using ScheduledCleanup.Helper;
using ScheduledCleanup.Model;

namespace ScheduledCleanup
{
    public partial class Form1 : Form
    {
        private System.Timers.Timer timer = new System.Timers.Timer();

        private static bool isRunning = false;

        public Form1()
        {
            InitializeComponent();
            InitializeTrayIcon();
            InitializeTimer();
            InitData();
        }

        private void InitializeTimer()
        {
            timer.Interval = DataConfigProvider.DataConfig.Config.Seconds; // 1秒
            timer.Elapsed += Timer2_Elapsed;
            timer.AutoReset = true; // 设置为false表示只触发一次
        }

        private void InitData()
        {
            var paths = DataConfigProvider.DataConfig.DelPaths;

            for (int i = 0; i < paths.Count; i++)
            {
                var item = paths[i];
                checkedListBox1.Items.Add($"{i + 1}. {item.ToString()}", item.Selected);
            }

            var config = DataConfigProvider.DataConfig.Config;
            if (config != null)
            {
                foreach (var item in config.Allow)
                {
                    checkedListBox2.Items.Add(item.Extension, item.Selected);
                }

                if (!string.IsNullOrEmpty(config.Start))
                {
                    dateTimePicker1.Text = config.Start;
                }

                if (!string.IsNullOrEmpty(config.End))
                {
                    dateTimePicker2.Text = config.End;
                }

                if (config.Interval <= 0)
                {
                    textBox2.Text = "60"; // 默认60秒
                }
                else
                {
                    textBox2.Text = config.Interval.ToString();
                }

                checkBox1.Checked = config.DelEmptyFolder;
            }


            button4.Enabled = false; // 默认禁用结束按钮

            button7.Enabled = false; // 默认禁用删除按钮

            button8.Enabled = false; // 默认禁用浏览按钮
        }

        /// <summary>
        /// 定时清理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer2_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var selectedPaths = DataConfigProvider.DataConfig.DelPaths
            .Where(x => x.Selected && Directory.Exists(x.Path))
            .ToList();

            var allowedExtensions = DataConfigProvider.DataConfig.Config.Allow
            .Where(x => x.Selected && !string.IsNullOrWhiteSpace(x.Extension))
            .Select(x => x.Extension.Trim().ToLowerInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);


            if (!selectedPaths.Any() || !allowedExtensions.Any())
            {
                return;
            }

            foreach (var delPath in selectedPaths)
            {
                if (!TryParseDateRange(delPath, out var startTime, out var endTime))
                {
                    //Logger.WriteLog($"[无效日期范围] {delPath.Path}", LogLevel.ERROR);
                    continue;
                }

                try
                {
                    var files = allowedExtensions
                        .SelectMany(ext => Directory.EnumerateFiles(delPath.Path, $"*.{ext.TrimStart('.')}", SearchOption.AllDirectories))
                        .Distinct()
                        .ToList();

                    foreach (var file in files)
                    {
                        ProcessFile(file, startTime, endTime);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLog($"[目录处理失败] {delPath.Path} - {ex.Message}", LogLevel.ERROR);
                }

                if (DataConfigProvider.DataConfig.Config.DelEmptyFolder)
                {
                    // 删除空目录
                    FileHelper.DeleteEmptyDirectories(delPath.Path);
                }
            }

            // 如果需要更新UI，使用Invoke
            //this.Invoke((MethodInvoker)delegate
            //{
            //    label1.Text = currentTime;
            //});
        }

        private static bool TryParseDateRange(DelPath delPath, out DateTime startTime, out DateTime endTime)
        {
            startTime = default;
            endTime = default;

            return !string.IsNullOrWhiteSpace(delPath.Start) &&
                   !string.IsNullOrWhiteSpace(delPath.End) &&
                   DateTime.TryParse(delPath.Start, out startTime) &&
                   DateTime.TryParse(delPath.End, out endTime) &&
                   startTime <= endTime;
        }

        private static void ProcessFile(string file, DateTime startTime, DateTime endTime)
        {
            try
            {
                if (!File.Exists(file))
                {
                    return;
                }

                var start = startTime.ToString("yyyy/MM/dd 00:00:00");
                var end = endTime.ToString("yyyy/MM/dd 23:59:59");


                var fileInfo = new FileInfo(file);
                if (fileInfo.CreationTime >= Convert.ToDateTime(start) && fileInfo.CreationTime <= Convert.ToDateTime(end))
                {
                    var result = FileHelper.DelFile(file);
                    if (result.Success)
                    {
                        Logger.WriteLog($"[删除成功] - [{fileInfo.CreationTime}] {file}");
                    }
                    else
                    {
                        Logger.WriteLog($"[删除失败] {file} - {result.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog($"[删除失败] {file} - {ex.Message}", LogLevel.ERROR);
            }
        }

        #region 系统托盘

        private NotifyIcon notifyIcon;
        private ContextMenuStrip trayMenu;

        private void InitializeTrayIcon()
        {
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            // 创建托盘图标
            notifyIcon = new NotifyIcon
            {
                Icon = (Icon)resources.GetObject("$this.Icon"), // 使用项目资源中的图标
                Text = "定时清理程序中上传的资源文件",
                Visible = false
            };

            // 创建托盘菜单
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("显示主窗口", null, (s, e) => RestoreFromTray());
            trayMenu.Items.Add("退出", null, (s, e) => ExitApplication());

            notifyIcon.ContextMenuStrip = trayMenu;

            // 双击托盘图标恢复窗口
            notifyIcon.DoubleClick += (s, e) => RestoreFromTray();
        }

        private void RestoreFromTray()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
            this.Activate(); // 激活窗口
        }

        private void ExitApplication()
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // 询问用户操作
                var result = MessageBox.Show(
                    "需要最小化到系统托盘吗？\n点击【是】最小化到托盘，点击【否】退出程序。",
                    "提示",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    e.Cancel = true; // 取消关闭事件
                    MinimizeToTray();
                }
                else
                {
                    ExitApplication();
                }
            }

            base.OnFormClosing(e);
        }

        private void MinimizeToTray()
        {
            this.Hide();
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(1000, "提示", "程序已最小化到托盘", ToolTipIcon.Info);
        }

        // 窗体最小化按钮点击事件
        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                MinimizeToTray();
            }
        }

        #endregion

        /// <summary>
        /// 选择目录目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            string selectedPath = folderBrowserDialog1.SelectedPath;
            if (selectedPath != null)
            {
                try
                {
                    textBox1.Text = selectedPath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"选择目录异常: {ex.Message}", "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("未选择目录", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 添加目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("请选择需要清理目录", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 目录是否合法
            if (!Directory.Exists(textBox1.Text))
            {
                MessageBox.Show("所选目录不存在或不是一个有效的目录", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show($"确定要定时清理该目录下【{dateTimePicker1.Value.ToShortDateString()} - {dateTimePicker2.Value.ToShortDateString()}】创建的文件 ？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            if (DataConfigProvider.PathExists(textBox1.Text))
            {
                MessageBox.Show("目录已存在", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var data = new DelPath
                {
                    Path = textBox1.Text,
                    Selected = true,
                    Start = dateTimePicker1.Value.ToShortDateString(),
                    End = dateTimePicker2.Value.ToShortDateString()
                };

                var count = DataConfigProvider.DataConfig.DelPaths.Count;

                DataConfigProvider.AddPath(data);
                checkedListBox1.Items.Add($"{count + 1}. {data.ToString()}", true);

                var config = DataConfigProvider.DataConfig.Config;
                config.Start = dateTimePicker1.Value.ToShortDateString();
                config.End = dateTimePicker2.Value.ToShortDateString();

                DataConfigProvider.SaveConfig(config);

                textBox1.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加目录异常: {ex.Message}", "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 查看日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            var logPath = Path.Combine(AppContext.BaseDirectory, "logs");
            FileHelper.OpenPath(logPath);
        }

        /// <summary>
        /// 开启删除服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.Items.Count == 0)
            {
                MessageBox.Show("请添加要清理的目录", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            timer.Start();
            isRunning = true;

            button3.Enabled = false;
            button4.Enabled = true;
        }

        /// <summary>
        /// 结束删除服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            timer.Stop();
            isRunning = false;

            button3.Enabled = true;
            button4.Enabled = false;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text) || !int.TryParse(textBox2.Text, out int interval) || interval <= 0)
            {
                MessageBox.Show("请输入有效的删除间隔时间（正整数）", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dateTimePicker1.Value == null || dateTimePicker2.Value == null)
            {
                MessageBox.Show("请选择开始和结束时间", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (Convert.ToDateTime(dateTimePicker1.Value.ToShortDateString()) > Convert.ToDateTime(dateTimePicker2.Value.ToShortDateString()))
            {
                MessageBox.Show("开始时间必须早于或等于结束时间", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var list = new List<Allow>();
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                var item = checkedListBox2.Items[i];
                var extension = checkedListBox2.GetItemText(item);
                var selected = checkedListBox2.GetItemChecked(i);
                list.Add(new Allow
                {
                    Extension = extension,
                    Selected = selected
                });
            }


            DataConfigProvider.SaveConfig(new Config
            {
                Interval = Convert.ToInt32(textBox2.Text),
                Start = dateTimePicker1.Value.ToShortDateString(),
                End = dateTimePicker2.Value.ToShortDateString(),
                DelEmptyFolder = checkBox1.Checked,
                Allow = list
            });

            if (isRunning)
            {
                timer.Stop();
            }

            timer.Interval = DataConfigProvider.DataConfig.Config.Seconds; // 更新定时器间隔
            timer.Start();

            // 保存成功
            MessageBox.Show("配置信息保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 更新选中状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var path = DataConfigProvider.GetPath(e.Index);
            DataConfigProvider.SetPathSelected(path, e.NewValue == CheckState.Checked);
        }

        /// <summary>
        /// 删除选中目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            // 获取选中(勾选)项的数量
            int selectedCount = checkedListBox1.SelectedItems.Count;

            if (selectedCount == 0)
            {
                MessageBox.Show("请先选中要删除的目录", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 确认删除对话框
            string message = selectedCount > 1 ?
                $"确定要删除选中的 {selectedCount} 个目录吗？" :
                "确定要删除选中的目录吗？";

            DialogResult result = MessageBox.Show(message, "确认删除",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                checkedListBox1.BeginUpdate();
                try
                {
                    // 从后向前删除选中项
                    for (int i = checkedListBox1.Items.Count - 1; i >= 0; i--)
                    {
                        if (checkedListBox1.SelectedIndices.Contains(i))
                        {
                            var path = checkedListBox1.Items[i].ToString();

                            // 从数据配置中删除目录
                            //DataConfigHelper.RemovePath(path);
                            DataConfigProvider.RemovePath(i);

                            checkedListBox1.Items.RemoveAt(i);
                        }
                    }
                }
                finally
                {
                    checkedListBox1.EndUpdate();
                    // 删除后更新按钮状态
                    button7.Enabled = false;
                }
            }
        }

        /// <summary>
        /// 鼠标选中项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button7.Enabled = checkedListBox1.SelectedItems.Count > 0;
            button8.Enabled = checkedListBox1.SelectedItems.Count > 0;
        }

        /// <summary>
        /// 浏览目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            // 获取选中(勾选)项的数量
            int selectedCount = checkedListBox1.SelectedItems.Count;

            if (selectedCount == 0)
            {
                MessageBox.Show("请先选中要打开的目录", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 从后向前删除选中项
            for (int i = checkedListBox1.Items.Count - 1; i >= 0; i--)
            {
                if (checkedListBox1.SelectedIndices.Contains(i))
                {
                    var path = DataConfigProvider.GetPath(i);
                    FileHelper.OpenPath(path);
                }
            }

        }

        /// <summary>
        /// 添加格式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            var allow = textBox3.Text;
            if (string.IsNullOrWhiteSpace(allow))
            {
                MessageBox.Show("请输入需要添加的格式", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DataConfigProvider.DataConfig.Config.Allow.Select(x => x.Extension).Contains(allow))
            {
                MessageBox.Show("格式已存在", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataConfigProvider.AddAllowExtension(allow);
                checkedListBox2.Items.Add(allow, true);
                textBox3.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加格式异常: {ex.Message}", "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 获取焦点读取剪贴板内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Activated(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
            {
                string unicodeText = Clipboard.GetText(TextDataFormat.UnicodeText);
                if (Directory.Exists(unicodeText))
                {
                    textBox1.Text = unicodeText;
                }
            }
        }

        /// <summary>
        /// 选中允许删除的格式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedListBox2_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var allow = checkedListBox2.Items[e.Index].ToString();
            DataConfigProvider.SetAllowSelected(allow, e.NewValue == CheckState.Checked);
        }
    }
}
