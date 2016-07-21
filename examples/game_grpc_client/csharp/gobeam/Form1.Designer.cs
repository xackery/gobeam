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
            this.newRoundTimer = new System.Windows.Forms.Timer(this.components);
            this.lblStatus = new System.Windows.Forms.Label();
            this.txtProcess = new System.Windows.Forms.TextBox();
            this.lblProcess = new System.Windows.Forms.Label();
            this.btnAttach = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // refreshTimer
            // 
            this.refreshTimer.Enabled = true;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            // 
            // newRoundTimer
            // 
            this.newRoundTimer.Interval = 5000;
            this.newRoundTimer.Tick += new System.EventHandler(this.newRoundTimer_Tick);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 58);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(40, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Status:";
            // 
            // txtProcess
            // 
            this.txtProcess.Location = new System.Drawing.Point(100, 78);
            this.txtProcess.Name = "txtProcess";
            this.txtProcess.Size = new System.Drawing.Size(100, 20);
            this.txtProcess.TabIndex = 3;
            // 
            // lblProcess
            // 
            this.lblProcess.AutoSize = true;
            this.lblProcess.Location = new System.Drawing.Point(15, 81);
            this.lblProcess.Name = "lblProcess";
            this.lblProcess.Size = new System.Drawing.Size(79, 13);
            this.lblProcess.TabIndex = 4;
            this.lblProcess.Text = "Process Name:";
            // 
            // btnAttach
            // 
            this.btnAttach.Location = new System.Drawing.Point(12, 104);
            this.btnAttach.Name = "btnAttach";
            this.btnAttach.Size = new System.Drawing.Size(225, 23);
            this.btnAttach.TabIndex = 5;
            this.btnAttach.Text = "Attach";
            this.btnAttach.UseVisualStyleBackColor = true;
            this.btnAttach.Click += new System.EventHandler(this.btnAttach_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(249, 135);
            this.Controls.Add(this.btnAttach);
            this.Controls.Add(this.lblProcess);
            this.Controls.Add(this.txtProcess);
            this.Controls.Add(this.lblStatus);
            this.Name = "Form1";
            this.Text = "GoBeam Client";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer refreshTimer;
        private System.Windows.Forms.Timer newRoundTimer;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtProcess;
        private System.Windows.Forms.Label lblProcess;
        private System.Windows.Forms.Button btnAttach;
    }
}

