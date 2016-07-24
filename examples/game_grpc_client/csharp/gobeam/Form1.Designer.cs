namespace gobeam
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.txtProcess = new System.Windows.Forms.TextBox();
            this.lblProcess = new System.Windows.Forms.Label();
            this.btnAttach = new System.Windows.Forms.Button();
            this.txtAddr = new System.Windows.Forms.TextBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.prgLeft = new System.Windows.Forms.ToolStripProgressBar();
            this.prgRight = new System.Windows.Forms.ToolStripProgressBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grpControls = new System.Windows.Forms.GroupBox();
            this.grpJoystick = new System.Windows.Forms.GroupBox();
            this.cmbJoystick = new System.Windows.Forms.ComboBox();
            this.chkJoyUp = new System.Windows.Forms.CheckBox();
            this.btnTestJoystick = new System.Windows.Forms.Button();
            this.btnTestControls = new System.Windows.Forms.Button();
            this.grdControls = new System.Windows.Forms.DataGridView();
            this.Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Label = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tmrTestControls = new System.Windows.Forms.Timer(this.components);
            this.tmrTestJoystick = new System.Windows.Forms.Timer(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnTestgRPC = new System.Windows.Forms.Button();
            this.grpAttach = new System.Windows.Forms.GroupBox();
            this.grpConnect = new System.Windows.Forms.GroupBox();
            this.tmrJoyPress = new System.Windows.Forms.Timer(this.components);
            this.tmrProgressUpdate = new System.Windows.Forms.Timer(this.components);
            this.statusStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.grpControls.SuspendLayout();
            this.grpJoystick.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdControls)).BeginInit();
            this.grpAttach.SuspendLayout();
            this.grpConnect.SuspendLayout();
            this.SuspendLayout();
            // 
            // refreshTimer
            // 
            this.refreshTimer.Enabled = true;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            // 
            // txtProcess
            // 
            this.txtProcess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProcess.Location = new System.Drawing.Point(57, 13);
            this.txtProcess.Name = "txtProcess";
            this.txtProcess.Size = new System.Drawing.Size(227, 20);
            this.txtProcess.TabIndex = 3;
            this.txtProcess.Tag = "Which process requires focus in order for keys to press";
            this.txtProcess.Text = "project64";
            this.txtProcess.TextChanged += new System.EventHandler(this.txtProcess_TextChanged);
            // 
            // lblProcess
            // 
            this.lblProcess.AutoSize = true;
            this.lblProcess.Location = new System.Drawing.Point(3, 16);
            this.lblProcess.Name = "lblProcess";
            this.lblProcess.Size = new System.Drawing.Size(38, 13);
            this.lblProcess.TabIndex = 4;
            this.lblProcess.Tag = "Which process requires focus in order for keys to press";
            this.lblProcess.Text = "Name:";
            // 
            // btnAttach
            // 
            this.btnAttach.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAttach.Location = new System.Drawing.Point(6, 39);
            this.btnAttach.Name = "btnAttach";
            this.btnAttach.Size = new System.Drawing.Size(278, 23);
            this.btnAttach.TabIndex = 5;
            this.btnAttach.Tag = "Attach to Process";
            this.btnAttach.Text = "Attach";
            this.btnAttach.UseVisualStyleBackColor = true;
            this.btnAttach.Click += new System.EventHandler(this.btnAttach_Click);
            // 
            // txtAddr
            // 
            this.txtAddr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAddr.Location = new System.Drawing.Point(57, 13);
            this.txtAddr.Name = "txtAddr";
            this.txtAddr.Size = new System.Drawing.Size(227, 20);
            this.txtAddr.TabIndex = 6;
            this.txtAddr.Tag = "What address the gRPC server at";
            this.txtAddr.Text = "127.0.0.1:50051";
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(3, 16);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(48, 13);
            this.lblAddress.TabIndex = 7;
            this.lblAddress.Tag = "What address the gRPC server at";
            this.lblAddress.Text = "Address:";
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(6, 35);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(278, 23);
            this.btnConnect.TabIndex = 8;
            this.btnConnect.Tag = "Connect to gRPC server";
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.prgLeft,
            this.prgRight});
            this.statusStrip.Location = new System.Drawing.Point(0, 470);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(310, 22);
            this.statusStrip.TabIndex = 9;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = false;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(250, 17);
            this.lblStatus.Text = "Idle.";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // prgLeft
            // 
            this.prgLeft.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.prgLeft.Maximum = 1;
            this.prgLeft.Name = "prgLeft";
            this.prgLeft.Size = new System.Drawing.Size(20, 16);
            this.prgLeft.Click += new System.EventHandler(this.toolStripProgressBar1_Click);
            // 
            // prgRight
            // 
            this.prgRight.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.prgRight.AutoSize = false;
            this.prgRight.BackColor = System.Drawing.SystemColors.Control;
            this.prgRight.Maximum = 1;
            this.prgRight.Name = "prgRight";
            this.prgRight.Size = new System.Drawing.Size(20, 16);
            this.prgRight.Click += new System.EventHandler(this.toolStripProgressBar2_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(310, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newConfigurationToolStripMenuItem,
            this.loadKeysToolStripMenuItem,
            this.saveConfigurationToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newConfigurationToolStripMenuItem
            // 
            this.newConfigurationToolStripMenuItem.Name = "newConfigurationToolStripMenuItem";
            this.newConfigurationToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.newConfigurationToolStripMenuItem.Text = "&New Configuration";
            this.newConfigurationToolStripMenuItem.Click += new System.EventHandler(this.newConfigurationToolStripMenuItem_Click);
            // 
            // loadKeysToolStripMenuItem
            // 
            this.loadKeysToolStripMenuItem.Name = "loadKeysToolStripMenuItem";
            this.loadKeysToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.loadKeysToolStripMenuItem.Text = "&Load Configuration...";
            this.loadKeysToolStripMenuItem.Click += new System.EventHandler(this.loadKeysToolStripMenuItem_Click);
            // 
            // saveConfigurationToolStripMenuItem
            // 
            this.saveConfigurationToolStripMenuItem.Name = "saveConfigurationToolStripMenuItem";
            this.saveConfigurationToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.saveConfigurationToolStripMenuItem.Text = "&Save Configuration...";
            this.saveConfigurationToolStripMenuItem.Click += new System.EventHandler(this.saveConfigurationToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // grpControls
            // 
            this.grpControls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpControls.Controls.Add(this.grpJoystick);
            this.grpControls.Controls.Add(this.btnTestJoystick);
            this.grpControls.Controls.Add(this.btnTestControls);
            this.grpControls.Controls.Add(this.grdControls);
            this.grpControls.Location = new System.Drawing.Point(12, 101);
            this.grpControls.Name = "grpControls";
            this.grpControls.Size = new System.Drawing.Size(290, 267);
            this.grpControls.TabIndex = 13;
            this.grpControls.TabStop = false;
            this.grpControls.Text = "Controls";
            // 
            // grpJoystick
            // 
            this.grpJoystick.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpJoystick.Controls.Add(this.cmbJoystick);
            this.grpJoystick.Controls.Add(this.chkJoyUp);
            this.grpJoystick.Location = new System.Drawing.Point(7, 207);
            this.grpJoystick.Name = "grpJoystick";
            this.grpJoystick.Size = new System.Drawing.Size(277, 54);
            this.grpJoystick.TabIndex = 18;
            this.grpJoystick.TabStop = false;
            this.grpJoystick.Text = "Joystick Control Emulator";
            // 
            // cmbJoystick
            // 
            this.cmbJoystick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmbJoystick.FormattingEnabled = true;
            this.cmbJoystick.Items.AddRange(new object[] {
            "Up",
            "Left",
            "Right",
            "Down",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32"});
            this.cmbJoystick.Location = new System.Drawing.Point(6, 19);
            this.cmbJoystick.Name = "cmbJoystick";
            this.cmbJoystick.Size = new System.Drawing.Size(121, 21);
            this.cmbJoystick.TabIndex = 17;
            this.cmbJoystick.Text = "Up";
            this.cmbJoystick.SelectedIndexChanged += new System.EventHandler(this.cmbJoystick_SelectedIndexChanged);
            // 
            // chkJoyUp
            // 
            this.chkJoyUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkJoyUp.AutoSize = true;
            this.chkJoyUp.Enabled = false;
            this.chkJoyUp.Location = new System.Drawing.Point(137, 21);
            this.chkJoyUp.Name = "chkJoyUp";
            this.chkJoyUp.Size = new System.Drawing.Size(134, 17);
            this.chkJoyUp.TabIndex = 16;
            this.chkJoyUp.Text = "Hold Joystick Direction";
            this.chkJoyUp.UseVisualStyleBackColor = true;
            this.chkJoyUp.CheckedChanged += new System.EventHandler(this.chkJoyUp_CheckedChanged);
            // 
            // btnTestJoystick
            // 
            this.btnTestJoystick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestJoystick.Enabled = false;
            this.btnTestJoystick.Location = new System.Drawing.Point(148, 178);
            this.btnTestJoystick.Name = "btnTestJoystick";
            this.btnTestJoystick.Size = new System.Drawing.Size(135, 23);
            this.btnTestJoystick.TabIndex = 15;
            this.btnTestJoystick.Tag = "Detect and Test Joystick";
            this.btnTestJoystick.Text = "Detect Joystick";
            this.btnTestJoystick.UseVisualStyleBackColor = true;
            this.btnTestJoystick.Click += new System.EventHandler(this.btnTestJoystick_Click);
            // 
            // btnTestControls
            // 
            this.btnTestControls.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTestControls.Enabled = false;
            this.btnTestControls.Location = new System.Drawing.Point(6, 178);
            this.btnTestControls.Name = "btnTestControls";
            this.btnTestControls.Size = new System.Drawing.Size(131, 23);
            this.btnTestControls.TabIndex = 14;
            this.btnTestControls.Tag = "Test Pressing Keys";
            this.btnTestControls.Text = "Test Tactile";
            this.btnTestControls.UseVisualStyleBackColor = true;
            this.btnTestControls.Click += new System.EventHandler(this.btnTestControls_Click);
            // 
            // grdControls
            // 
            this.grdControls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdControls.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdControls.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Index,
            this.Label,
            this.Type,
            this.Key});
            this.grdControls.Location = new System.Drawing.Point(6, 19);
            this.grdControls.Name = "grdControls";
            this.grdControls.RowHeadersWidth = 24;
            this.grdControls.Size = new System.Drawing.Size(278, 149);
            this.grdControls.TabIndex = 12;
            this.grdControls.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdControls_CellContentClick);
            this.grdControls.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdControls_CellDoubleClick);
            // 
            // Index
            // 
            this.Index.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Index.FillWeight = 79.18781F;
            this.Index.HeaderText = "Index";
            this.Index.Name = "Index";
            this.Index.ReadOnly = true;
            this.Index.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Label
            // 
            this.Label.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Label.FillWeight = 79.18781F;
            this.Label.HeaderText = "Label";
            this.Label.Name = "Label";
            this.Label.ReadOnly = true;
            // 
            // Type
            // 
            this.Type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Type.FillWeight = 162.4366F;
            this.Type.HeaderText = "Type";
            this.Type.MinimumWidth = 10;
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            this.Type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Key
            // 
            this.Key.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Key.FillWeight = 79.18781F;
            this.Key.HeaderText = "Key";
            this.Key.Name = "Key";
            this.Key.ReadOnly = true;
            // 
            // tmrTestControls
            // 
            this.tmrTestControls.Interval = 1000;
            this.tmrTestControls.Tick += new System.EventHandler(this.tmrTestControls_Tick);
            // 
            // tmrTestJoystick
            // 
            this.tmrTestJoystick.Interval = 500;
            this.tmrTestJoystick.Tick += new System.EventHandler(this.tmrTestJoystick_Tick);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnTestgRPC
            // 
            this.btnTestgRPC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTestgRPC.Enabled = false;
            this.btnTestgRPC.Location = new System.Drawing.Point(6, 64);
            this.btnTestgRPC.Name = "btnTestgRPC";
            this.btnTestgRPC.Size = new System.Drawing.Size(131, 23);
            this.btnTestgRPC.TabIndex = 14;
            this.btnTestgRPC.Text = "Test ProgressUpdate";
            this.btnTestgRPC.UseVisualStyleBackColor = true;
            this.btnTestgRPC.Click += new System.EventHandler(this.btnTestgRPC_Click);
            // 
            // grpAttach
            // 
            this.grpAttach.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpAttach.Controls.Add(this.txtProcess);
            this.grpAttach.Controls.Add(this.btnAttach);
            this.grpAttach.Controls.Add(this.lblProcess);
            this.grpAttach.Location = new System.Drawing.Point(12, 27);
            this.grpAttach.Name = "grpAttach";
            this.grpAttach.Size = new System.Drawing.Size(290, 68);
            this.grpAttach.TabIndex = 15;
            this.grpAttach.TabStop = false;
            this.grpAttach.Text = "Process";
            // 
            // grpConnect
            // 
            this.grpConnect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpConnect.Controls.Add(this.lblAddress);
            this.grpConnect.Controls.Add(this.btnConnect);
            this.grpConnect.Controls.Add(this.btnTestgRPC);
            this.grpConnect.Controls.Add(this.txtAddr);
            this.grpConnect.Location = new System.Drawing.Point(12, 374);
            this.grpConnect.Name = "grpConnect";
            this.grpConnect.Size = new System.Drawing.Size(290, 93);
            this.grpConnect.TabIndex = 16;
            this.grpConnect.TabStop = false;
            this.grpConnect.Text = "gRPC";
            // 
            // tmrJoyPress
            // 
            this.tmrJoyPress.Interval = 500;
            this.tmrJoyPress.Tick += new System.EventHandler(this.tmrJoyPress_Tick);
            // 
            // tmrProgressUpdate
            // 
            this.tmrProgressUpdate.Interval = 500;
            this.tmrProgressUpdate.Tick += new System.EventHandler(this.tmrProgressUpdate_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 492);
            this.Controls.Add(this.grpConnect);
            this.Controls.Add(this.grpAttach);
            this.Controls.Add(this.grpControls);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(326, 429);
            this.Name = "Form1";
            this.Text = "GoBeam v1.0.0.0";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.grpControls.ResumeLayout(false);
            this.grpJoystick.ResumeLayout(false);
            this.grpJoystick.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdControls)).EndInit();
            this.grpAttach.ResumeLayout(false);
            this.grpAttach.PerformLayout();
            this.grpConnect.ResumeLayout(false);
            this.grpConnect.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer refreshTimer;
        private System.Windows.Forms.TextBox txtProcess;
        private System.Windows.Forms.Label lblProcess;
        private System.Windows.Forms.Button btnAttach;
        private System.Windows.Forms.TextBox txtAddr;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadKeysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveConfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.GroupBox grpControls;
        private System.Windows.Forms.DataGridView grdControls;
        private System.Windows.Forms.Button btnTestControls;
        private System.Windows.Forms.Timer tmrTestControls;
        private System.Windows.Forms.Button btnTestJoystick;
        private System.Windows.Forms.Timer tmrTestJoystick;
        private System.Windows.Forms.ToolStripMenuItem newConfigurationToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnTestgRPC;
        private System.Windows.Forms.GroupBox grpAttach;
        private System.Windows.Forms.GroupBox grpConnect;
        private System.Windows.Forms.ComboBox cmbJoystick;
        private System.Windows.Forms.CheckBox chkJoyUp;
        private System.Windows.Forms.Timer tmrJoyPress;
        private System.Windows.Forms.GroupBox grpJoystick;
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        private System.Windows.Forms.DataGridViewTextBoxColumn Label;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Key;
        private System.Windows.Forms.ToolStripProgressBar prgLeft;
        private System.Windows.Forms.ToolStripProgressBar prgRight;
        private System.Windows.Forms.Timer tmrProgressUpdate;
    }
}

