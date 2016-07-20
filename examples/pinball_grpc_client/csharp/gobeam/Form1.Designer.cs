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
            this.lblScore = new System.Windows.Forms.Label();
            this.lblBall = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
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
            // lblScore
            // 
            this.lblScore.AutoSize = true;
            this.lblScore.Location = new System.Drawing.Point(12, 9);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(41, 13);
            this.lblScore.TabIndex = 0;
            this.lblScore.Text = "Score: ";
            // 
            // lblBall
            // 
            this.lblBall.AutoSize = true;
            this.lblBall.Location = new System.Drawing.Point(12, 34);
            this.lblBall.Name = "lblBall";
            this.lblBall.Size = new System.Drawing.Size(30, 13);
            this.lblBall.TabIndex = 1;
            this.lblBall.Text = "Ball: ";
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(249, 92);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblBall);
            this.Controls.Add(this.lblScore);
            this.Name = "Form1";
            this.Text = "Pinball Bot";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer refreshTimer;
        private System.Windows.Forms.Timer newRoundTimer;
        private System.Windows.Forms.Label lblScore;
        private System.Windows.Forms.Label lblBall;
        private System.Windows.Forms.Label lblStatus;
    }
}

