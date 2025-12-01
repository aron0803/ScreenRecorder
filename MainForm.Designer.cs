namespace ScreenRecorder
{
    partial class MainForm
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
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.grpSettings = new System.Windows.Forms.GroupBox();
            this.lblFps = new System.Windows.Forms.Label();
            this.numFps = new System.Windows.Forms.NumericUpDown();
            this.lblBitrate = new System.Windows.Forms.Label();
            this.cmbBitrate = new System.Windows.Forms.ComboBox();
            this.rbFullscreen = new System.Windows.Forms.RadioButton();
            this.rbRegion = new System.Windows.Forms.RadioButton();
            this.btnSelectRegion = new System.Windows.Forms.Button();
            this.chkShowKeys = new System.Windows.Forms.CheckBox();
            this.chkShowMouse = new System.Windows.Forms.CheckBox();
            this.chkAudio = new System.Windows.Forms.CheckBox();
            this.lstFiles = new System.Windows.Forms.ListBox();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.grpSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFps)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(50, 210);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(120, 40);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "開始錄製";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(200, 210);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(120, 40);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "停止錄製";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // grpSettings
            // 
            this.grpSettings.Controls.Add(this.btnSelectRegion);
            this.grpSettings.Controls.Add(this.rbRegion);
            this.grpSettings.Controls.Add(this.rbFullscreen);
            this.grpSettings.Controls.Add(this.cmbBitrate);
            this.grpSettings.Controls.Add(this.lblBitrate);
            this.grpSettings.Controls.Add(this.numFps);
            this.grpSettings.Controls.Add(this.lblFps);
            this.grpSettings.Controls.Add(this.chkShowKeys);
            this.grpSettings.Controls.Add(this.chkShowMouse);
            this.grpSettings.Controls.Add(this.chkAudio);
            this.grpSettings.Location = new System.Drawing.Point(12, 12);
            this.grpSettings.Name = "grpSettings";
            this.grpSettings.Size = new System.Drawing.Size(360, 180);
            this.grpSettings.TabIndex = 3;
            this.grpSettings.TabStop = false;
            this.grpSettings.Text = "錄影設定";
            // 
            // lblFps
            // 
            this.lblFps.AutoSize = true;
            this.lblFps.Location = new System.Drawing.Point(20, 30);
            this.lblFps.Name = "lblFps";
            this.lblFps.Size = new System.Drawing.Size(27, 13);
            this.lblFps.TabIndex = 0;
            this.lblFps.Text = "FPS:";
            // 
            // numFps
            // 
            this.numFps.Location = new System.Drawing.Point(80, 28);
            this.numFps.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numFps.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numFps.Name = "numFps";
            this.numFps.Size = new System.Drawing.Size(60, 20);
            this.numFps.TabIndex = 1;
            this.numFps.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // lblBitrate
            // 
            this.lblBitrate.AutoSize = true;
            this.lblBitrate.Location = new System.Drawing.Point(180, 30);
            this.lblBitrate.Name = "lblBitrate";
            this.lblBitrate.Size = new System.Drawing.Size(41, 13);
            this.lblBitrate.TabIndex = 2;
            this.lblBitrate.Text = "位元率:";
            // 
            // cmbBitrate
            // 
            this.cmbBitrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBitrate.FormattingEnabled = true;
            this.cmbBitrate.Items.AddRange(new object[] {
            "低 (1.5 Mbps)",
            "中 (4 Mbps)",
            "高 (8 Mbps)"});
            this.cmbBitrate.Location = new System.Drawing.Point(230, 27);
            this.cmbBitrate.Name = "cmbBitrate";
            this.cmbBitrate.Size = new System.Drawing.Size(100, 21);
            this.cmbBitrate.TabIndex = 3;
            // 
            // rbFullscreen
            // 
            this.rbFullscreen.AutoSize = true;
            this.rbFullscreen.Checked = true;
            this.rbFullscreen.Location = new System.Drawing.Point(23, 70);
            this.rbFullscreen.Name = "rbFullscreen";
            this.rbFullscreen.Size = new System.Drawing.Size(73, 17);
            this.rbFullscreen.TabIndex = 4;
            this.rbFullscreen.TabStop = true;
            this.rbFullscreen.Text = "全螢幕";
            this.rbFullscreen.UseVisualStyleBackColor = true;
            this.rbFullscreen.CheckedChanged += new System.EventHandler(this.rbSource_CheckedChanged);
            // 
            // rbRegion
            // 
            this.rbRegion.AutoSize = true;
            this.rbRegion.Location = new System.Drawing.Point(120, 70);
            this.rbRegion.Name = "rbRegion";
            this.rbRegion.Size = new System.Drawing.Size(73, 17);
            this.rbRegion.TabIndex = 5;
            this.rbRegion.Text = "選取範圍";
            this.rbRegion.UseVisualStyleBackColor = true;
            this.rbRegion.CheckedChanged += new System.EventHandler(this.rbSource_CheckedChanged);
            // 
            // btnSelectRegion
            // 
            this.btnSelectRegion.Enabled = false;
            this.btnSelectRegion.Location = new System.Drawing.Point(200, 65);
            this.btnSelectRegion.Name = "btnSelectRegion";
            this.btnSelectRegion.Size = new System.Drawing.Size(130, 25);
            this.btnSelectRegion.TabIndex = 6;
            this.btnSelectRegion.Text = "選取範圍...";
            this.btnSelectRegion.UseVisualStyleBackColor = true;
            this.btnSelectRegion.Click += new System.EventHandler(this.btnSelectRegion_Click);
            // 
            // chkShowKeys
            // 
            this.chkShowKeys.AutoSize = true;
            this.chkShowKeys.Checked = true;
            this.chkShowKeys.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowKeys.Location = new System.Drawing.Point(23, 110);
            this.chkShowKeys.Name = "chkShowKeys";
            this.chkShowKeys.Size = new System.Drawing.Size(98, 17);
            this.chkShowKeys.TabIndex = 7;
            this.chkShowKeys.Text = "顯示按鍵";
            this.chkShowKeys.UseVisualStyleBackColor = true;
            this.chkShowKeys.CheckedChanged += new System.EventHandler(this.chkShowKeys_CheckedChanged);
            // 
            // chkShowMouse
            // 
            this.chkShowMouse.AutoSize = true;
            this.chkShowMouse.Checked = true;
            this.chkShowMouse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowMouse.Location = new System.Drawing.Point(23, 140);
            this.chkShowMouse.Name = "chkShowMouse";
            this.chkShowMouse.Size = new System.Drawing.Size(98, 17);
            this.chkShowMouse.TabIndex = 8;
            this.chkShowMouse.Text = "顯示滑鼠";
            this.chkShowMouse.UseVisualStyleBackColor = true;
            this.chkShowMouse.CheckedChanged += new System.EventHandler(this.chkShowMouse_CheckedChanged);
            // 
            // chkAudio
            // 
            this.chkAudio.AutoSize = true;
            this.chkAudio.Checked = true;
            this.chkAudio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAudio.Location = new System.Drawing.Point(120, 110);
            this.chkAudio.Name = "chkAudio";
            this.chkAudio.Size = new System.Drawing.Size(98, 17);
            this.chkAudio.TabIndex = 9;
            this.chkAudio.Text = "錄製聲音";
            this.chkAudio.UseVisualStyleBackColor = true;
            // 
            // lstFiles
            // 
            this.lstFiles.FormattingEnabled = true;
            this.lstFiles.Location = new System.Drawing.Point(400, 12);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(280, 216);
            this.lstFiles.TabIndex = 4;
            this.lstFiles.DoubleClick += new System.EventHandler(this.lstFiles_DoubleClick);
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.Location = new System.Drawing.Point(400, 238);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(280, 25);
            this.btnOpenFolder.TabIndex = 5;
            this.btnOpenFolder.Text = "開啟資料夾";
            this.btnOpenFolder.UseVisualStyleBackColor = true;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 269);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(700, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(31, 17);
            this.lblStatus.Text = "就緒";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 291);
            this.Controls.Add(this.btnOpenFolder);
            this.Controls.Add(this.lstFiles);
            this.Controls.Add(this.grpSettings);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Name = "MainForm";
            this.Text = "Screen Recorder";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.grpSettings.ResumeLayout(false);
            this.grpSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFps)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.GroupBox grpSettings;
        private System.Windows.Forms.Label lblFps;
        private System.Windows.Forms.NumericUpDown numFps;
        private System.Windows.Forms.Label lblBitrate;
        private System.Windows.Forms.ComboBox cmbBitrate;
        private System.Windows.Forms.RadioButton rbFullscreen;
        private System.Windows.Forms.RadioButton rbRegion;
        private System.Windows.Forms.Button btnSelectRegion;
        private System.Windows.Forms.CheckBox chkShowKeys;
        private System.Windows.Forms.CheckBox chkShowMouse;
        private System.Windows.Forms.CheckBox chkAudio;
        private System.Windows.Forms.ListBox lstFiles;
        private System.Windows.Forms.Button btnOpenFolder;

        #endregion
    }
}
