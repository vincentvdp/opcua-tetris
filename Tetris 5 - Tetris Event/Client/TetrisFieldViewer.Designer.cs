namespace Quickstarts.TetrisClient
{
    partial class TetrisFieldViewer
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
            this.btnClose = new System.Windows.Forms.Button();
            this.lblGameActive = new System.Windows.Forms.Label();
            this.lblScore = new System.Windows.Forms.Label();
            this.txtMyMessage = new System.Windows.Forms.TextBox();
            this.btnMyPause = new System.Windows.Forms.Button();
            this.txtSecondsToPause = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSecsRemaining = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtEvents = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(13, 13);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblGameActive
            // 
            this.lblGameActive.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblGameActive.Location = new System.Drawing.Point(177, 13);
            this.lblGameActive.Name = "lblGameActive";
            this.lblGameActive.Size = new System.Drawing.Size(117, 23);
            this.lblGameActive.TabIndex = 1;
            this.lblGameActive.Text = "Game Active: ...";
            this.lblGameActive.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblScore
            // 
            this.lblScore.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblScore.Location = new System.Drawing.Point(94, 13);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(77, 23);
            this.lblScore.TabIndex = 2;
            this.lblScore.Text = "Score: ...";
            this.lblScore.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtMyMessage
            // 
            this.txtMyMessage.Location = new System.Drawing.Point(13, 39);
            this.txtMyMessage.Multiline = true;
            this.txtMyMessage.Name = "txtMyMessage";
            this.txtMyMessage.ReadOnly = true;
            this.txtMyMessage.Size = new System.Drawing.Size(107, 46);
            this.txtMyMessage.TabIndex = 3;
            this.txtMyMessage.TextChanged += new System.EventHandler(this.txtMyMessage_TextChanged);
            // 
            // btnMyPause
            // 
            this.btnMyPause.Location = new System.Drawing.Point(126, 39);
            this.btnMyPause.Name = "btnMyPause";
            this.btnMyPause.Size = new System.Drawing.Size(75, 23);
            this.btnMyPause.TabIndex = 4;
            this.btnMyPause.Text = "Pause for ...";
            this.btnMyPause.UseVisualStyleBackColor = true;
            this.btnMyPause.Click += new System.EventHandler(this.btnMyPause_Click);
            // 
            // txtSecondsToPause
            // 
            this.txtSecondsToPause.Location = new System.Drawing.Point(207, 41);
            this.txtSecondsToPause.Name = "txtSecondsToPause";
            this.txtSecondsToPause.Size = new System.Drawing.Size(32, 20);
            this.txtSecondsToPause.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(246, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "seconds";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // lblSecsRemaining
            // 
            this.lblSecsRemaining.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSecsRemaining.Location = new System.Drawing.Point(126, 65);
            this.lblSecsRemaining.Name = "lblSecsRemaining";
            this.lblSecsRemaining.Size = new System.Drawing.Size(32, 21);
            this.lblSecsRemaining.TabIndex = 7;
            this.lblSecsRemaining.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(164, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(131, 22);
            this.label2.TabIndex = 8;
            this.label2.Text = "seconds remaining";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtEvents
            // 
            this.txtEvents.Location = new System.Drawing.Point(302, 44);
            this.txtEvents.Multiline = true;
            this.txtEvents.Name = "txtEvents";
            this.txtEvents.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtEvents.Size = new System.Drawing.Size(290, 525);
            this.txtEvents.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(302, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(290, 23);
            this.label3.TabIndex = 12;
            this.label3.Text = "Events";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TetrisFieldViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 582);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtEvents);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblSecsRemaining);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSecondsToPause);
            this.Controls.Add(this.btnMyPause);
            this.Controls.Add(this.txtMyMessage);
            this.Controls.Add(this.lblScore);
            this.Controls.Add(this.lblGameActive);
            this.Controls.Add(this.btnClose);
            this.MaximumSize = new System.Drawing.Size(620, 620);
            this.MinimumSize = new System.Drawing.Size(323, 620);
            this.Name = "TetrisFieldViewer";
            this.Text = "Tetris Client FieldViewer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblGameActive;
        private System.Windows.Forms.Label lblScore;
        private System.Windows.Forms.TextBox txtMyMessage;
        private System.Windows.Forms.Button btnMyPause;
        private System.Windows.Forms.TextBox txtSecondsToPause;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSecsRemaining;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtEvents;
        private System.Windows.Forms.Label label3;
    }
}