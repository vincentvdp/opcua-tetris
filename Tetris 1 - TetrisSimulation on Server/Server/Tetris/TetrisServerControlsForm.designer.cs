namespace Quickstarts.TetrisServer
{
    partial class TetrisServerControlsForm
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
            this.btnStart = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.txtScore = new System.Windows.Forms.TextBox();
            this.btnRotate = new System.Windows.Forms.Button();
            this.btnLower = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.cbxPaused = new System.Windows.Forms.CheckBox();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnPauseFor = new System.Windows.Forms.Button();
            this.txtSecs = new System.Windows.Forms.TextBox();
            this.txtSecsTillUnpause = new System.Windows.Forms.TextBox();
            this.cbxTurned = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(47, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "&Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(65, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(47, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtScore
            // 
            this.txtScore.Location = new System.Drawing.Point(217, 14);
            this.txtScore.Name = "txtScore";
            this.txtScore.ReadOnly = true;
            this.txtScore.Size = new System.Drawing.Size(76, 20);
            this.txtScore.TabIndex = 4;
            // 
            // btnRotate
            // 
            this.btnRotate.Location = new System.Drawing.Point(127, 69);
            this.btnRotate.Name = "btnRotate";
            this.btnRotate.Size = new System.Drawing.Size(47, 23);
            this.btnRotate.TabIndex = 7;
            this.btnRotate.Text = "Rotate";
            this.btnRotate.UseVisualStyleBackColor = true;
            this.btnRotate.Click += new System.EventHandler(this.btnRotate_Click);
            // 
            // btnLower
            // 
            this.btnLower.Location = new System.Drawing.Point(127, 98);
            this.btnLower.Name = "btnLower";
            this.btnLower.Size = new System.Drawing.Size(47, 23);
            this.btnLower.TabIndex = 8;
            this.btnLower.Text = "Lower";
            this.btnLower.UseVisualStyleBackColor = true;
            this.btnLower.Click += new System.EventHandler(this.btnLower_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(74, 98);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(47, 23);
            this.btnLeft.TabIndex = 9;
            this.btnLeft.Text = "Left";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(180, 98);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(47, 23);
            this.btnRight.TabIndex = 10;
            this.btnRight.Text = "Right";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(171, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 20);
            this.label1.TabIndex = 11;
            this.label1.Text = "Score:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(118, 12);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(47, 23);
            this.btnReset.TabIndex = 12;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(12, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 20);
            this.label2.TabIndex = 13;
            this.label2.Text = "input";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtInput
            // 
            this.txtInput.Location = new System.Drawing.Point(74, 72);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(47, 20);
            this.txtInput.TabIndex = 14;
            this.txtInput.TextChanged += new System.EventHandler(this.txtInput_TextChanged);
            // 
            // cbxPaused
            // 
            this.cbxPaused.AutoSize = true;
            this.cbxPaused.Location = new System.Drawing.Point(176, 45);
            this.cbxPaused.Name = "cbxPaused";
            this.cbxPaused.Size = new System.Drawing.Size(62, 17);
            this.cbxPaused.TabIndex = 15;
            this.cbxPaused.Text = "Paused";
            this.cbxPaused.UseVisualStyleBackColor = true;
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(244, 41);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(49, 23);
            this.btnPause.TabIndex = 16;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnPauseFor
            // 
            this.btnPauseFor.Location = new System.Drawing.Point(12, 41);
            this.btnPauseFor.Name = "btnPauseFor";
            this.btnPauseFor.Size = new System.Drawing.Size(84, 23);
            this.btnPauseFor.TabIndex = 17;
            this.btnPauseFor.Text = "Pause ... secs";
            this.btnPauseFor.UseVisualStyleBackColor = true;
            this.btnPauseFor.Click += new System.EventHandler(this.btnPauseFor_Click);
            // 
            // txtSecs
            // 
            this.txtSecs.Location = new System.Drawing.Point(102, 43);
            this.txtSecs.Name = "txtSecs";
            this.txtSecs.Size = new System.Drawing.Size(31, 20);
            this.txtSecs.TabIndex = 18;
            // 
            // txtSecsTillUnpause
            // 
            this.txtSecsTillUnpause.Location = new System.Drawing.Point(139, 43);
            this.txtSecsTillUnpause.Name = "txtSecsTillUnpause";
            this.txtSecsTillUnpause.ReadOnly = true;
            this.txtSecsTillUnpause.Size = new System.Drawing.Size(31, 20);
            this.txtSecsTillUnpause.TabIndex = 19;
            // 
            // cbxTurned
            // 
            this.cbxTurned.AutoSize = true;
            this.cbxTurned.Location = new System.Drawing.Point(12, 102);
            this.cbxTurned.Name = "cbxTurned";
            this.cbxTurned.Size = new System.Drawing.Size(56, 17);
            this.cbxTurned.TabIndex = 1;
            this.cbxTurned.Text = "turned";
            this.cbxTurned.UseVisualStyleBackColor = true;
            this.cbxTurned.Visible = false;
            // 
            // TetrisServerControlsForm
            // 
            this.ClientSize = new System.Drawing.Size(305, 631);
            this.Controls.Add(this.txtSecsTillUnpause);
            this.Controls.Add(this.txtSecs);
            this.Controls.Add(this.btnPauseFor);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.cbxPaused);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnLower);
            this.Controls.Add(this.btnRotate);
            this.Controls.Add(this.txtScore);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.cbxTurned);
            this.Controls.Add(this.btnStart);
            this.MaximumSize = new System.Drawing.Size(321, 669);
            this.MinimumSize = new System.Drawing.Size(321, 669);
            this.Name = "TetrisServerControlsForm";
            this.Text = "Tetris Server Controls";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbxPaused;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnPauseFor;
        private System.Windows.Forms.TextBox txtSecs;
        private System.Windows.Forms.TextBox txtSecsTillUnpause;
        private System.Windows.Forms.CheckBox cbxTurned;
    }
}