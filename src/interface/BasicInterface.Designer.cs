namespace WinDHCP
{
    partial class BasicInterface
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BasicInterface));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cb_interface = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lb_mac = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lb_raspberry_address = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.cb_run_on_startup = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.loggerText = new System.Windows.Forms.RichTextBox();
            this.timer_check_ipaddr = new System.Windows.Forms.Timer(this.components);
            this.timer_check_interface = new System.Windows.Forms.Timer(this.components);
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.TEXT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Firmware = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IPAddr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ST1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ST2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Src = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UUID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(771, 415);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(763, 383);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Information";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Size = new System.Drawing.Size(757, 377);
            this.splitContainer1.SplitterDistance = 251;
            this.splitContainer1.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cb_interface);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(251, 377);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Network";
            // 
            // cb_interface
            // 
            this.cb_interface.Dock = System.Windows.Forms.DockStyle.Top;
            this.cb_interface.FormattingEnabled = true;
            this.cb_interface.Location = new System.Drawing.Point(3, 43);
            this.cb_interface.Name = "cb_interface";
            this.cb_interface.Size = new System.Drawing.Size(245, 26);
            this.cb_interface.TabIndex = 3;
            this.cb_interface.SelectedIndexChanged += new System.EventHandler(this.cb_interface_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Top;
            this.label5.Location = new System.Drawing.Point(3, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 18);
            this.label5.TabIndex = 4;
            this.label5.Text = "Interface:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 18);
            this.label4.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lb_mac);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.lb_raspberry_address);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(502, 377);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Machine";
            // 
            // lb_mac
            // 
            this.lb_mac.AutoSize = true;
            this.lb_mac.Location = new System.Drawing.Point(88, 66);
            this.lb_mac.Name = "lb_mac";
            this.lb_mac.Size = new System.Drawing.Size(14, 18);
            this.lb_mac.TabIndex = 4;
            this.lb_mac.Text = "-";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(26, 66);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 18);
            this.label6.TabIndex = 3;
            this.label6.Text = "MAC: ";
            // 
            // lb_raspberry_address
            // 
            this.lb_raspberry_address.AutoSize = true;
            this.lb_raspberry_address.Location = new System.Drawing.Point(88, 41);
            this.lb_raspberry_address.Name = "lb_raspberry_address";
            this.lb_raspberry_address.Size = new System.Drawing.Size(14, 18);
            this.lb_raspberry_address.TabIndex = 2;
            this.lb_raspberry_address.Text = "-";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 18);
            this.label3.TabIndex = 1;
            this.label3.Text = "IP: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 18);
            this.label2.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 18);
            this.label1.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.cb_run_on_startup);
            this.tabPage3.Location = new System.Drawing.Point(4, 28);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(622, 314);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Settings";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // cb_run_on_startup
            // 
            this.cb_run_on_startup.AutoSize = true;
            this.cb_run_on_startup.Location = new System.Drawing.Point(22, 21);
            this.cb_run_on_startup.Name = "cb_run_on_startup";
            this.cb_run_on_startup.Size = new System.Drawing.Size(134, 22);
            this.cb_run_on_startup.TabIndex = 0;
            this.cb_run_on_startup.Text = "Run on startup";
            this.cb_run_on_startup.UseVisualStyleBackColor = true;
            this.cb_run_on_startup.CheckedChanged += new System.EventHandler(this.cb_run_on_startup_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.loggerText);
            this.tabPage2.Location = new System.Drawing.Point(4, 28);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(622, 314);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Console Log";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // loggerText
            // 
            this.loggerText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loggerText.Location = new System.Drawing.Point(3, 3);
            this.loggerText.Name = "loggerText";
            this.loggerText.Size = new System.Drawing.Size(616, 308);
            this.loggerText.TabIndex = 0;
            this.loggerText.Text = "";
            // 
            // timer_check_ipaddr
            // 
            this.timer_check_ipaddr.Enabled = true;
            this.timer_check_ipaddr.Interval = 3000;
            this.timer_check_ipaddr.Tick += new System.EventHandler(this.timer_check_ipaddr_Tick);
            // 
            // timer_check_interface
            // 
            this.timer_check_interface.Enabled = true;
            this.timer_check_interface.Interval = 10000;
            this.timer_check_interface.Tick += new System.EventHandler(this.timer_check_interface_Tick);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.dataGridView1);
            this.tabPage4.Location = new System.Drawing.Point(4, 28);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(763, 383);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Machine Listing";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TEXT,
            this.Firmware,
            this.IPAddr,
            this.ST1,
            this.ST2,
            this.Src,
            this.UUID});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(757, 377);
            this.dataGridView1.TabIndex = 3;
            // 
            // TEXT
            // 
            this.TEXT.DataPropertyName = "Name";
            this.TEXT.HeaderText = "Device Name";
            this.TEXT.Name = "TEXT";
            this.TEXT.ReadOnly = true;
            // 
            // Firmware
            // 
            this.Firmware.DataPropertyName = "FirmwareVersion";
            this.Firmware.HeaderText = "Firmware";
            this.Firmware.Name = "Firmware";
            this.Firmware.Width = 70;
            // 
            // IPAddr
            // 
            this.IPAddr.DataPropertyName = "IPAddr";
            this.IPAddr.FillWeight = 130F;
            this.IPAddr.HeaderText = "IP Address";
            this.IPAddr.Name = "IPAddr";
            this.IPAddr.Width = 130;
            // 
            // ST1
            // 
            this.ST1.DataPropertyName = "ST1";
            this.ST1.HeaderText = "ST1";
            this.ST1.Name = "ST1";
            this.ST1.ReadOnly = true;
            this.ST1.Width = 40;
            // 
            // ST2
            // 
            this.ST2.DataPropertyName = "ST2";
            dataGridViewCellStyle1.NullValue = "N/A";
            this.ST2.DefaultCellStyle = dataGridViewCellStyle1;
            this.ST2.HeaderText = "ST2";
            this.ST2.Name = "ST2";
            this.ST2.Width = 40;
            // 
            // Src
            // 
            this.Src.DataPropertyName = "Src";
            this.Src.HeaderText = "Source";
            this.Src.Name = "Src";
            // 
            // UUID
            // 
            this.UUID.DataPropertyName = "UUID";
            this.UUID.FillWeight = 80F;
            this.UUID.HeaderText = "UUID";
            this.UUID.MinimumWidth = 300;
            this.UUID.Name = "UUID";
            this.UUID.ReadOnly = true;
            this.UUID.Width = 300;
            // 
            // BasicInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 415);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BasicInterface";
            this.Text = "FLUX Delta Connection Tool";
            this.Load += new System.EventHandler(this.BasicInterface_Load);
            this.Resize += new System.EventHandler(this.BasicInterface_Resize);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RichTextBox loggerText;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cb_interface;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Timer timer_check_ipaddr;
        private System.Windows.Forms.Label lb_raspberry_address;
        private System.Windows.Forms.Timer timer_check_interface;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.CheckBox cb_run_on_startup;
        private System.Windows.Forms.Label lb_mac;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn TEXT;
        private System.Windows.Forms.DataGridViewTextBoxColumn Firmware;
        private System.Windows.Forms.DataGridViewTextBoxColumn IPAddr;
        private System.Windows.Forms.DataGridViewTextBoxColumn ST1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ST2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Src;
        private System.Windows.Forms.DataGridViewTextBoxColumn UUID;
    }
}