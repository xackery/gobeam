namespace gobeam
{
    partial class InputKey
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputKey));
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblPressKey = new System.Windows.Forms.Label();
            this.cmbInputType = new System.Windows.Forms.ComboBox();
            this.cmbKey = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.cmbJoystickID = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(12, 143);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(145, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            this.btnCancel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnCancel_KeyDown);
            // 
            // lblPressKey
            // 
            this.lblPressKey.AutoSize = true;
            this.lblPressKey.Location = new System.Drawing.Point(35, 9);
            this.lblPressKey.Name = "lblPressKey";
            this.lblPressKey.Size = new System.Drawing.Size(97, 13);
            this.lblPressKey.TabIndex = 1;
            this.lblPressKey.Text = "Press a key to map";
            // 
            // cmbInputType
            // 
            this.cmbInputType.FormattingEnabled = true;
            this.cmbInputType.Items.AddRange(new object[] {
            "Tactile",
            "JoystickKey"});
            this.cmbInputType.Location = new System.Drawing.Point(77, 25);
            this.cmbInputType.Name = "cmbInputType";
            this.cmbInputType.Size = new System.Drawing.Size(80, 21);
            this.cmbInputType.TabIndex = 2;
            this.cmbInputType.Text = "Tactile";
            this.cmbInputType.SelectedIndexChanged += new System.EventHandler(this.cmbInputType_SelectedIndexChanged);
            this.cmbInputType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbInputType_KeyDown);
            this.cmbInputType.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbInputType_KeyPress);
            // 
            // cmbKey
            // 
            this.cmbKey.Enabled = false;
            this.cmbKey.FormattingEnabled = true;
            this.cmbKey.Items.AddRange(new object[] {
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
            this.cmbKey.Location = new System.Drawing.Point(76, 79);
            this.cmbKey.Name = "cmbKey";
            this.cmbKey.Size = new System.Drawing.Size(81, 21);
            this.cmbKey.TabIndex = 3;
            this.cmbKey.Text = "1";
            this.cmbKey.SelectedIndexChanged += new System.EventHandler(this.cmbKey_SelectedIndexChanged);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(12, 114);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(145, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cmbJoystickID
            // 
            this.cmbJoystickID.Enabled = false;
            this.cmbJoystickID.FormattingEnabled = true;
            this.cmbJoystickID.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.cmbJoystickID.Location = new System.Drawing.Point(77, 52);
            this.cmbJoystickID.Name = "cmbJoystickID";
            this.cmbJoystickID.Size = new System.Drawing.Size(81, 21);
            this.cmbJoystickID.TabIndex = 5;
            this.cmbJoystickID.Text = "1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Type:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Joystick ID:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Key ID:";
            // 
            // InputKey
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(170, 178);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbJoystickID);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cmbKey);
            this.Controls.Add(this.cmbInputType);
            this.Controls.Add(this.lblPressKey);
            this.Controls.Add(this.btnCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputKey";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InputKey #";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InputKey_FormClosing);
            this.Load += new System.EventHandler(this.InputKey_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InputKey_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblPressKey;
        private System.Windows.Forms.ComboBox cmbInputType;
        private System.Windows.Forms.ComboBox cmbKey;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cmbJoystickID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}