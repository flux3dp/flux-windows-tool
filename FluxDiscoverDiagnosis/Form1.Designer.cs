namespace FluxDiscoverDiagnosis
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.TEXT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Firmware = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IPAddr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ST1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ST2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Src = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UUID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_scan = new System.Windows.Forms.Button();
            this.btn_ping = new System.Windows.Forms.Button();
            this.txt_ip = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_reconnect = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.txt_network = new System.Windows.Forms.ToolStripStatusLabel();
            this.txt_status = new System.Windows.Forms.ToolStripStatusLabel();
            this.pgbar = new System.Windows.Forms.ToolStripProgressBar();
            this.btn_getip = new System.Windows.Forms.Button();
            this.form1BindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.multicastStBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.form1BindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.multicastStBindingSource)).BeginInit();
            this.SuspendLayout();
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
            this.dataGridView1.Location = new System.Drawing.Point(12, 38);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(819, 372);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
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
            // btn_scan
            // 
            this.btn_scan.Location = new System.Drawing.Point(12, 9);
            this.btn_scan.Name = "btn_scan";
            this.btn_scan.Size = new System.Drawing.Size(75, 23);
            this.btn_scan.TabIndex = 3;
            this.btn_scan.Text = "Scan";
            this.btn_scan.UseVisualStyleBackColor = true;
            this.btn_scan.Click += new System.EventHandler(this.btn_scan_click);
            // 
            // btn_ping
            // 
            this.btn_ping.Location = new System.Drawing.Point(760, 8);
            this.btn_ping.Name = "btn_ping";
            this.btn_ping.Size = new System.Drawing.Size(71, 24);
            this.btn_ping.TabIndex = 4;
            this.btn_ping.Text = "Ping";
            this.btn_ping.UseVisualStyleBackColor = true;
            this.btn_ping.Click += new System.EventHandler(this.btn_ping_click);
            // 
            // txt_ip
            // 
            this.txt_ip.Location = new System.Drawing.Point(654, 9);
            this.txt_ip.Name = "txt_ip";
            this.txt_ip.Size = new System.Drawing.Size(100, 22);
            this.txt_ip.TabIndex = 5;
            this.txt_ip.Tag = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(405, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(243, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "*Require device firmware version 1.0b14 or newer";
            // 
            // btn_reconnect
            // 
            this.btn_reconnect.Location = new System.Drawing.Point(93, 9);
            this.btn_reconnect.Name = "btn_reconnect";
            this.btn_reconnect.Size = new System.Drawing.Size(95, 23);
            this.btn_reconnect.TabIndex = 8;
            this.btn_reconnect.Text = "Refresh Network";
            this.btn_reconnect.UseVisualStyleBackColor = true;
            this.btn_reconnect.Click += new System.EventHandler(this.btn_reconnect_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.txt_network,
            this.txt_status,
            this.pgbar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 423);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(843, 22);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // txt_network
            // 
            this.txt_network.Name = "txt_network";
            this.txt_network.Size = new System.Drawing.Size(54, 17);
            this.txt_network.Text = "Ethernet";
            // 
            // txt_status
            // 
            this.txt_status.Name = "txt_status";
            this.txt_status.Size = new System.Drawing.Size(133, 17);
            this.txt_status.Text = "Discovering machine...";
            // 
            // pgbar
            // 
            this.pgbar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.pgbar.Enabled = false;
            this.pgbar.Name = "pgbar";
            this.pgbar.Size = new System.Drawing.Size(100, 16);
            // 
            // btn_getip
            // 
            this.btn_getip.Location = new System.Drawing.Point(194, 9);
            this.btn_getip.Name = "btn_getip";
            this.btn_getip.Size = new System.Drawing.Size(95, 23);
            this.btn_getip.TabIndex = 10;
            this.btn_getip.Text = "Get IP by USB";
            this.btn_getip.UseVisualStyleBackColor = true;
            this.btn_getip.Click += new System.EventHandler(this.btn_getip_Click);
            // 
            // form1BindingSource
            // 
            this.form1BindingSource.DataSource = typeof(FluxDiscoverDiagnosis.Form1);
            // 
            // multicastStBindingSource
            // 
            this.multicastStBindingSource.DataSource = typeof(FluxDiscoverDiagnosis.UdpStatus);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 445);
            this.Controls.Add(this.btn_getip);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btn_reconnect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_ip);
            this.Controls.Add(this.btn_ping);
            this.Controls.Add(this.btn_scan);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = " FLUX Delta Diagnosis Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.form1BindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.multicastStBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource form1BindingSource;
        private System.Windows.Forms.BindingSource multicastStBindingSource;
        private System.Windows.Forms.Button btn_scan;
        private System.Windows.Forms.Button btn_ping;
        private System.Windows.Forms.TextBox txt_ip;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_reconnect;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel txt_status;
        private System.Windows.Forms.ToolStripProgressBar pgbar;
        private System.Windows.Forms.ToolStripStatusLabel txt_network;
        private System.Windows.Forms.DataGridViewTextBoxColumn TEXT;
        private System.Windows.Forms.DataGridViewTextBoxColumn Firmware;
        private System.Windows.Forms.DataGridViewTextBoxColumn IPAddr;
        private System.Windows.Forms.DataGridViewTextBoxColumn ST1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ST2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Src;
        private System.Windows.Forms.DataGridViewTextBoxColumn UUID;
        private System.Windows.Forms.Button btn_getip;
    }
}

