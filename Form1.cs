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
            foreach (var item in paths)
            {
                checkedListBox1.Items.Add(item.ToString(), item.Selected);
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
            var paths = DataConfigProvider.DataConfig.DelPaths.Where(x => x.Selected);

            var allow = DataConfigProvider.DataConfig.Config.Allow.Where(x => x.Selected);

            foreach (var delPath in paths)
            {
                if (!Directory.Exists(delPath.Path))
                {
                    Logger.WriteLog($"[目录不存在] {delPath.Path}", LogLevel.ERROR);
                    continue;
                }

                foreach (var item in allow)
                {
                    if (string.IsNullOrWhiteSpace(item.Extension))
                    {
                        continue;
                    }

                    var files = Directory.GetFiles(delPath.Path, $"*{item.Extension}");
                    foreach (var file in files)
                    {
                        if (!File.Exists(file))
                        {
                            continue;
                        }

                        var fileInfo = new FileInfo(file);

                        // 检查文件创建时间是否在指定范围内
                        if (DateTime.TryParse(fileInfo.LastWriteTime.ToShortDateString(), out DateTime creationTime))
                        {
                            var startTime = DateTime.Parse(delPath.Start);
                            var endTime = DateTime.Parse(delPath.End);
                            if (creationTime >= startTime && creationTime <= endTime)
                            {
                                // 删除文件
                                try
                                {
                                    FileHelper.DelFile(file);
                                    Logger.WriteLog($"[删除成功] {file}");
                                }
                                catch (Exception ex)
                                {
                                    Logger.WriteLog($"[删除失败] {file} - {ex.Message}", LogLevel.ERROR);
                                }
                            }
                        }
                        else
                        {
                            Logger.WriteLog($"[时间解析失败] {fileInfo.CreationTime}", LogLevel.ERROR);
                        }
                    }
                }
            }

            // 如果需要更新UI，使用Invoke
            //this.Invoke((MethodInvoker)delegate
            //{
            //    label1.Text = currentTime;
            //});
        }

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

            //var confirm = MessageBox.Show("确定要定时清理该目录吗？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //if (confirm == DialogResult.Yes)
            //{

            //}

            if (DataConfigProvider.PathContains(textBox1.Text))
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

                DataConfigProvider.AddPath(data);
                checkedListBox1.Items.Add(data.ToString(), true);

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

        private void checkedListBox2_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var allow = checkedListBox2.Items[e.Index].ToString();
            DataConfigProvider.SetAllowSelected(allow, e.NewValue == CheckState.Checked);
        }
    }
}
