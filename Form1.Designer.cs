namespace ScheduledCleanup
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            folderBrowserDialog1 = new FolderBrowserDialog();
            button1 = new Button();
            textBox1 = new TextBox();
            button2 = new Button();
            checkedListBox1 = new CheckedListBox();
            label1 = new Label();
            dateTimePicker1 = new DateTimePicker();
            button3 = new Button();
            button4 = new Button();
            label2 = new Label();
            button5 = new Button();
            textBox2 = new TextBox();
            dateTimePicker2 = new DateTimePicker();
            label3 = new Label();
            label4 = new Label();
            button6 = new Button();
            button7 = new Button();
            button8 = new Button();
            checkedListBox2 = new CheckedListBox();
            label5 = new Label();
            textBox3 = new TextBox();
            button9 = new Button();
            label6 = new Label();
            checkBox1 = new CheckBox();
            comboBox1 = new ComboBox();
            toolTip1 = new ToolTip(components);
            SuspendLayout();
            // 
            // folderBrowserDialog1
            // 
            folderBrowserDialog1.Description = "请选择目标文件夹";
            // 
            // button1
            // 
            button1.Location = new Point(874, 41);
            button1.Name = "button1";
            button1.Size = new Size(90, 23);
            button1.TabIndex = 0;
            button1.Text = "选择文件夹";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(95, 42);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(752, 23);
            textBox1.TabIndex = 1;
            // 
            // button2
            // 
            button2.Location = new Point(970, 41);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 2;
            button2.Text = "添加";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // checkedListBox1
            // 
            checkedListBox1.ForeColor = Color.Green;
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Location = new Point(29, 253);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(1001, 328);
            checkedListBox1.TabIndex = 3;
            checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;
            checkedListBox1.SelectedIndexChanged += checkedListBox1_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 44);
            label1.Name = "label1";
            label1.Size = new Size(68, 17);
            label1.TabIndex = 4;
            label1.Text = "清除目录：";
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Location = new Point(95, 13);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(157, 23);
            dateTimePicker1.TabIndex = 5;
            // 
            // button3
            // 
            button3.Location = new Point(359, 597);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 6;
            button3.Text = "启动";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(440, 597);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 7;
            button4.Text = "停止";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(27, 18);
            label2.Name = "label2";
            label2.Size = new Size(68, 17);
            label2.TabIndex = 8;
            label2.Text = "文件日期：";
            // 
            // button5
            // 
            button5.Location = new Point(710, 597);
            button5.Name = "button5";
            button5.Size = new Size(75, 23);
            button5.TabIndex = 9;
            button5.Text = "查看日志";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(119, 209);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(155, 23);
            textBox2.TabIndex = 11;
            textBox2.Text = "60";
            // 
            // dateTimePicker2
            // 
            dateTimePicker2.Location = new Point(291, 12);
            dateTimePicker2.Name = "dateTimePicker2";
            dateTimePicker2.Size = new Size(148, 23);
            dateTimePicker2.TabIndex = 12;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(29, 213);
            label3.Name = "label3";
            label3.Size = new Size(92, 17);
            label3.TabIndex = 13;
            label3.Text = "执行间隔时间：";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(262, 16);
            label4.Name = "label4";
            label4.Size = new Size(13, 17);
            label4.TabIndex = 14;
            label4.Text = "-";
            // 
            // button6
            // 
            button6.Location = new Point(515, 208);
            button6.Name = "button6";
            button6.Size = new Size(75, 23);
            button6.TabIndex = 10;
            button6.Text = "保存配置";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // button7
            // 
            button7.ForeColor = Color.Red;
            button7.Location = new Point(525, 597);
            button7.Name = "button7";
            button7.Size = new Size(98, 23);
            button7.TabIndex = 15;
            button7.Text = "删除选中路径";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // button8
            // 
            button8.Location = new Point(629, 597);
            button8.Name = "button8";
            button8.Size = new Size(75, 23);
            button8.TabIndex = 16;
            button8.Text = "浏览目录";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // checkedListBox2
            // 
            checkedListBox2.CheckOnClick = true;
            checkedListBox2.FormattingEnabled = true;
            checkedListBox2.Location = new Point(95, 115);
            checkedListBox2.MultiColumn = true;
            checkedListBox2.Name = "checkedListBox2";
            checkedListBox2.Size = new Size(752, 76);
            checkedListBox2.TabIndex = 17;
            checkedListBox2.ItemCheck += checkedListBox2_ItemCheck;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(27, 113);
            label5.Name = "label5";
            label5.Size = new Size(68, 17);
            label5.TabIndex = 18;
            label5.Text = "清除格式：";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(95, 77);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(157, 23);
            textBox3.TabIndex = 19;
            // 
            // button9
            // 
            button9.Location = new Point(262, 77);
            button9.Name = "button9";
            button9.Size = new Size(75, 23);
            button9.TabIndex = 20;
            button9.Text = "添加";
            button9.UseVisualStyleBackColor = true;
            button9.Click += button9_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(27, 80);
            label6.Name = "label6";
            label6.Size = new Size(68, 17);
            label6.TabIndex = 21;
            label6.Text = "添加格式：";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(400, 211);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(99, 21);
            checkBox1.TabIndex = 22;
            checkBox1.Text = "删除空文件夹";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(284, 208);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(64, 25);
            comboBox1.TabIndex = 23;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1054, 632);
            Controls.Add(comboBox1);
            Controls.Add(checkBox1);
            Controls.Add(label6);
            Controls.Add(button9);
            Controls.Add(textBox3);
            Controls.Add(label5);
            Controls.Add(checkedListBox2);
            Controls.Add(button8);
            Controls.Add(button7);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(dateTimePicker2);
            Controls.Add(textBox2);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(label2);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(dateTimePicker1);
            Controls.Add(label1);
            Controls.Add(checkedListBox1);
            Controls.Add(button2);
            Controls.Add(textBox1);
            Controls.Add(button1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "定时清理程序中上传的资源文件";
            Activated += Form1_Activated;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FolderBrowserDialog folderBrowserDialog1;
        private Button button1;
        private TextBox textBox1;
        private Button button2;
        private CheckedListBox checkedListBox1;
        private Label label1;
        private DateTimePicker dateTimePicker1;
        private Button button3;
        private Button button4;
        private Label label2;
        private Button button5;
        private TextBox textBox2;
        private DateTimePicker dateTimePicker2;
        private Label label3;
        private Label label4;
        private Button button6;
        private Button button7;
        private Button button8;
        private CheckedListBox checkedListBox2;
        private Label label5;
        private TextBox textBox3;
        private Button button9;
        private Label label6;
        private CheckBox checkBox1;
        private ComboBox comboBox1;
        private ToolTip toolTip1;
    }
}
