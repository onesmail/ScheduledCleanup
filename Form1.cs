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
            timer.Interval = DataConfigProvider.DataConfig.Config.Seconds; // 1��
            timer.Elapsed += Timer2_Elapsed;
            timer.AutoReset = true; // ����Ϊfalse��ʾֻ����һ��
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
                    textBox2.Text = "60"; // Ĭ��60��
                }
                else
                {
                    textBox2.Text = config.Interval.ToString();
                }

                checkBox1.Checked = config.DelEmptyFolder;
            }


            button4.Enabled = false; // Ĭ�Ͻ��ý�����ť

            button7.Enabled = false; // Ĭ�Ͻ���ɾ����ť

            button8.Enabled = false; // Ĭ�Ͻ��������ť
        }

        /// <summary>
        /// ��ʱ����
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
                    //Logger.WriteLog($"[��Ч���ڷ�Χ] {delPath.Path}", LogLevel.ERROR);
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
                    Logger.WriteLog($"[Ŀ¼����ʧ��] {delPath.Path} - {ex.Message}", LogLevel.ERROR);
                }

                if (DataConfigProvider.DataConfig.Config.DelEmptyFolder)
                {
                    // ɾ����Ŀ¼
                    FileHelper.DeleteEmptyDirectories(delPath.Path);
                }
            }

            // �����Ҫ����UI��ʹ��Invoke
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
                        Logger.WriteLog($"[ɾ���ɹ�] - [{fileInfo.CreationTime}] {file}");
                    }
                    else
                    {
                        Logger.WriteLog($"[ɾ��ʧ��] {file} - {result.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog($"[ɾ��ʧ��] {file} - {ex.Message}", LogLevel.ERROR);
            }
        }

        #region ϵͳ����

        private NotifyIcon notifyIcon;
        private ContextMenuStrip trayMenu;

        private void InitializeTrayIcon()
        {
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            // ��������ͼ��
            notifyIcon = new NotifyIcon
            {
                Icon = (Icon)resources.GetObject("$this.Icon"), // ʹ����Ŀ��Դ�е�ͼ��
                Text = "��ʱ����������ϴ�����Դ�ļ�",
                Visible = false
            };

            // �������̲˵�
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("��ʾ������", null, (s, e) => RestoreFromTray());
            trayMenu.Items.Add("�˳�", null, (s, e) => ExitApplication());

            notifyIcon.ContextMenuStrip = trayMenu;

            // ˫������ͼ��ָ�����
            notifyIcon.DoubleClick += (s, e) => RestoreFromTray();
        }

        private void RestoreFromTray()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
            this.Activate(); // �����
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
                // ѯ���û�����
                var result = MessageBox.Show(
                    "��Ҫ��С����ϵͳ������\n������ǡ���С�������̣���������˳�����",
                    "��ʾ",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    e.Cancel = true; // ȡ���ر��¼�
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
            notifyIcon.ShowBalloonTip(1000, "��ʾ", "��������С��������", ToolTipIcon.Info);
        }

        // ������С����ť����¼�
        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                MinimizeToTray();
            }
        }

        #endregion

        /// <summary>
        /// ѡ��Ŀ¼Ŀ¼
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
                    MessageBox.Show($"ѡ��Ŀ¼�쳣: {ex.Message}", "�쳣", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("δѡ��Ŀ¼", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// ���Ŀ¼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("��ѡ����Ҫ����Ŀ¼", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Ŀ¼�Ƿ�Ϸ�
            if (!Directory.Exists(textBox1.Text))
            {
                MessageBox.Show("��ѡĿ¼�����ڻ���һ����Ч��Ŀ¼", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show($"ȷ��Ҫ��ʱ�����Ŀ¼�¡�{dateTimePicker1.Value.ToShortDateString()} - {dateTimePicker2.Value.ToShortDateString()}���������ļ� ��", "ȷ��", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            if (DataConfigProvider.PathExists(textBox1.Text))
            {
                MessageBox.Show("Ŀ¼�Ѵ���", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show($"���Ŀ¼�쳣: {ex.Message}", "�쳣", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// �鿴��־
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            var logPath = Path.Combine(AppContext.BaseDirectory, "logs");
            FileHelper.OpenPath(logPath);
        }

        /// <summary>
        /// ����ɾ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.Items.Count == 0)
            {
                MessageBox.Show("�����Ҫ�����Ŀ¼", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            timer.Start();
            isRunning = true;

            button3.Enabled = false;
            button4.Enabled = true;
        }

        /// <summary>
        /// ����ɾ������
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
        /// ��������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text) || !int.TryParse(textBox2.Text, out int interval) || interval <= 0)
            {
                MessageBox.Show("��������Ч��ɾ�����ʱ�䣨��������", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dateTimePicker1.Value == null || dateTimePicker2.Value == null)
            {
                MessageBox.Show("��ѡ��ʼ�ͽ���ʱ��", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (Convert.ToDateTime(dateTimePicker1.Value.ToShortDateString()) > Convert.ToDateTime(dateTimePicker2.Value.ToShortDateString()))
            {
                MessageBox.Show("��ʼʱ��������ڻ���ڽ���ʱ��", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            timer.Interval = DataConfigProvider.DataConfig.Config.Seconds; // ���¶�ʱ�����
            timer.Start();

            // ����ɹ�
            MessageBox.Show("������Ϣ����ɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// ����ѡ��״̬
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var path = DataConfigProvider.GetPath(e.Index);
            DataConfigProvider.SetPathSelected(path, e.NewValue == CheckState.Checked);
        }

        /// <summary>
        /// ɾ��ѡ��Ŀ¼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            // ��ȡѡ��(��ѡ)�������
            int selectedCount = checkedListBox1.SelectedItems.Count;

            if (selectedCount == 0)
            {
                MessageBox.Show("����ѡ��Ҫɾ����Ŀ¼", "��ʾ",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // ȷ��ɾ���Ի���
            string message = selectedCount > 1 ?
                $"ȷ��Ҫɾ��ѡ�е� {selectedCount} ��Ŀ¼��" :
                "ȷ��Ҫɾ��ѡ�е�Ŀ¼��";

            DialogResult result = MessageBox.Show(message, "ȷ��ɾ��",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                checkedListBox1.BeginUpdate();
                try
                {
                    // �Ӻ���ǰɾ��ѡ����
                    for (int i = checkedListBox1.Items.Count - 1; i >= 0; i--)
                    {
                        if (checkedListBox1.SelectedIndices.Contains(i))
                        {
                            var path = checkedListBox1.Items[i].ToString();

                            // ������������ɾ��Ŀ¼
                            //DataConfigHelper.RemovePath(path);
                            DataConfigProvider.RemovePath(i);

                            checkedListBox1.Items.RemoveAt(i);
                        }
                    }
                }
                finally
                {
                    checkedListBox1.EndUpdate();
                    // ɾ������°�ť״̬
                    button7.Enabled = false;
                }
            }
        }

        /// <summary>
        /// ���ѡ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button7.Enabled = checkedListBox1.SelectedItems.Count > 0;
            button8.Enabled = checkedListBox1.SelectedItems.Count > 0;
        }

        /// <summary>
        /// ���Ŀ¼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            // ��ȡѡ��(��ѡ)�������
            int selectedCount = checkedListBox1.SelectedItems.Count;

            if (selectedCount == 0)
            {
                MessageBox.Show("����ѡ��Ҫ�򿪵�Ŀ¼", "��ʾ",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // �Ӻ���ǰɾ��ѡ����
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
        /// ��Ӹ�ʽ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            var allow = textBox3.Text;
            if (string.IsNullOrWhiteSpace(allow))
            {
                MessageBox.Show("��������Ҫ��ӵĸ�ʽ", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DataConfigProvider.DataConfig.Config.Allow.Select(x => x.Extension).Contains(allow))
            {
                MessageBox.Show("��ʽ�Ѵ���", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show($"��Ӹ�ʽ�쳣: {ex.Message}", "�쳣", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ��ȡ�����ȡ����������
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
        /// ѡ������ɾ���ĸ�ʽ
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
