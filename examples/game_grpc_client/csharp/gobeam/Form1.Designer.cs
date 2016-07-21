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
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.txtProcess = new System.Windows.Forms.TextBox();
            this.lblProcess = new System.Windows.Forms.Label();
            this.btnAttach = new System.Windows.Forms.Button();
            this.txtAddr = new System.Windows.Forms.TextBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grpControls = new System.Windows.Forms.GroupBox();
            this.btnTestJoystick = new System.Windows.Forms.Button();
            this.btnTestControls = new System.Windows.Forms.Button();
            this.grdControls = new System.Windows.Forms.DataGridView();
            this.Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Label = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tmrTestControls = new System.Windows.Forms.Timer(this.components);
            this.tmrTestJoystick = new System.Windows.Forms.Timer(this.components);
            this.statusStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.grpControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdControls)).BeginInit();
            this.SuspendLayout();
            // 
            // refreshTimer
            // 
            this.refreshTimer.Enabled = true;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            // 
            // txtProcess
            // 
            this.txtProcess.Location = new System.Drawing.Point(187, 29);
            this.txtProcess.Name = "txtProcess";
            this.txtProcess.Size = new System.Drawing.Size(115, 20);
            this.txtProcess.TabIndex = 3;
            this.txtProcess.Text = "project64";
            // 
            // lblProcess
            // 
            this.lblProcess.AutoSize = true;
            this.lblProcess.Location = new System.Drawing.Point(15, 32);
            this.lblProcess.Name = "lblProcess";
            this.lblProcess.Size = new System.Drawing.Size(79, 13);
            this.lblProcess.TabIndex = 4;
            this.lblProcess.Text = "Process Name:";
            // 
            // btnAttach
            // 
            this.btnAttach.Location = new System.Drawing.Point(12, 55);
            this.btnAttach.Name = "btnAttach";
            this.btnAttach.Size = new System.Drawing.Size(290, 23);
            this.btnAttach.TabIndex = 5;
            this.btnAttach.Text = "Attach";
            this.btnAttach.UseVisualStyleBackColor = true;
            this.btnAttach.Click += new System.EventHandler(this.btnAttach_Click);
            // 
            // txtAddr
            // 
            this.txtAddr.Location = new System.Drawing.Point(187, 312);
            this.txtAddr.Name = "txtAddr";
            this.txtAddr.Size = new System.Drawing.Size(115, 20);
            this.txtAddr.TabIndex = 6;
            this.txtAddr.Text = "127.0.0.1:50051";
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(15, 315);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(48, 13);
            this.lblAddress.TabIndex = 7;
            this.lblAddress.Text = "Address:";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(12, 338);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(290, 23);
            this.btnConnect.TabIndex = 8;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 420);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(310, 22);
            this.statusStrip.TabIndex = 9;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(29, 17);
            this.lblStatus.Text = "Idle.";
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
            this.loadKeysToolStripMenuItem,
            this.saveConfigurationToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // loadKeysToolStripMenuItem
            // 
            this.loadKeysToolStripMenuItem.Name = "loadKeysToolStripMenuItem";
            this.loadKeysToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.loadKeysToolStripMenuItem.Text = "&Load Keys...";
            // 
            // saveConfigurationToolStripMenuItem
            // 
            this.saveConfigurationToolStripMenuItem.Name = "saveConfigurationToolStripMenuItem";
            this.saveConfigurationToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.saveConfigurationToolStripMenuItem.Text = "&Save Configuration";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
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
            // 
            // grpControls
            // 
            this.grpControls.Controls.Add(this.btnTestJoystick);
            this.grpControls.Controls.Add(this.btnTestControls);
            this.grpControls.Controls.Add(this.grdControls);
            this.grpControls.Location = new System.Drawing.Point(18, 84);
            this.grpControls.Name = "grpControls";
            this.grpControls.Size = new System.Drawing.Size(284, 228);
            this.grpControls.TabIndex = 13;
            this.grpControls.TabStop = false;
            this.grpControls.Text = "Controls";
            // 
            // btnTestJoystick
            // 
            this.btnTestJoystick.Enabled = false;
            this.btnTestJoystick.Location = new System.Drawing.Point(143, 201);
            this.btnTestJoystick.Name = "btnTestJoystick";
            this.btnTestJoystick.Size = new System.Drawing.Size(135, 23);
            this.btnTestJoystick.TabIndex = 15;
            this.btnTestJoystick.Text = "Test Joystick";
            this.btnTestJoystick.UseVisualStyleBackColor = true;
            this.btnTestJoystick.Click += new System.EventHandler(this.btnTestJoystick_Click);
            // 
            // btnTestControls
            // 
            this.btnTestControls.Enabled = false;
            this.btnTestControls.Location = new System.Drawing.Point(6, 201);
            this.btnTestControls.Name = "btnTestControls";
            this.btnTestControls.Size = new System.Drawing.Size(131, 23);
            this.btnTestControls.TabIndex = 14;
            this.btnTestControls.Text = "Test Tactile";
            this.btnTestControls.UseVisualStyleBackColor = true;
            this.btnTestControls.Click += new System.EventHandler(this.btnTestControls_Click);
            // 
            // grdControls
            // 
            this.grdControls.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdControls.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Index,
            this.Label,
            this.Type,
            this.Key});
            this.grdControls.Location = new System.Drawing.Point(6, 19);
            this.grdControls.Name = "grdControls";
            this.grdControls.Size = new System.Drawing.Size(272, 176);
            this.grdControls.TabIndex = 12;
            this.grdControls.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdControls_CellContentClick);
            this.grdControls.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdControls_CellDoubleClick);
            // 
            // Index
            // 
            this.Index.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Index.HeaderText = "Index";
            this.Index.Name = "Index";
            this.Index.ReadOnly = true;
            this.Index.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Label
            // 
            this.Label.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Label.HeaderText = "Label";
            this.Label.Name = "Label";
            this.Label.ReadOnly = true;
            // 
            // Type
            // 
            this.Type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            // 
            // Key
            // 
            this.Key.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
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
            this.tmrTestJoystick.Interval = 250;
            this.tmrTestJoystick.Tick += new System.EventHandler(this.tmrTestJoystick_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 442);
            this.Controls.Add(this.grpControls);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.lblAddress);
            this.Controls.Add(this.txtAddr);
            this.Controls.Add(this.btnAttach);
            this.Controls.Add(this.lblProcess);
            this.Controls.Add(this.txtProcess);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "GoBeam Client";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.grpControls.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdControls)).EndInit();
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
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        private System.Windows.Forms.DataGridViewTextBoxColumn Label;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Key;
        private System.Windows.Forms.Button btnTestJoystick;
        private System.Windows.Forms.Timer tmrTestJoystick;
    }
}

