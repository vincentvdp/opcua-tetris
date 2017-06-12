using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Quickstarts.TetrisServer
{
    public partial class TetrisServerControlsForm : Form
    {
        #region private fields
        //Define constants
        private const int WHITE = 0;
        private const int YELLOW = 1;
        private const int ORANGE = 2;
        private const int RED = 3;
        private const int PURPLE = 4;
        private const int BLUE = 5;
        private const int BLUEGREEN = 6;
        private const int GREEN = 7;
        private const int LINEFULL = 8;
        private const int GREY = 10;

        private const string DOWN = "2";
        private const string LEFT = "1";
        private const string RIGHT = "3";
        private const string ROTATE = " ";

        private const int XUPLEFT = 12;
        private const int YUPLEFT = 135;
        private const int NRSQUARESINWIDTH = 10+4;
        private const int NRSQUARESINHEIGHT = 20+4;
        private int SQUAREPIXELWIDTH = 20;

        //The field to be displayed:
        private int[,] field = new int[NRSQUARESINWIDTH,NRSQUARESINHEIGHT];
        //The game to provide the data:
        private TetrisGame tetrisgame;
        //The timer to refresh the field etc.:
        private Timer refreshTimer;
        //------------

        //removed:
        //private static Timer myTimer = new Timer();
        private Button btnClose;
        private TextBox txtScore;
        private Button btnRotate;
        private Button btnLower;
        private Button btnLeft;
        private Button btnRight;
        private Label label1;
        private Button btnReset;
        private Label label2;
        private TextBox txtInput;
        private Button btnStart;

        #endregion

        
        #region Windows code
        /*
        private void InitializeComponent()
        {
            this.btnStart = new System.Windows.Forms.Button();
            this.cbxTurned = new System.Windows.Forms.CheckBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.txtScore = new System.Windows.Forms.TextBox();
            //this.cbxTimer = new System.Windows.Forms.CheckBox();
            this.btnRotate = new System.Windows.Forms.Button();
            this.btnLower = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "&Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // cbxTurned
            // 
            this.cbxTurned.AutoSize = true;
            this.cbxTurned.Location = new System.Drawing.Point(642, 12);
            this.cbxTurned.Name = "cbxTurned";
            this.cbxTurned.Size = new System.Drawing.Size(56, 17);
            this.cbxTurned.TabIndex = 1;
            this.cbxTurned.Text = "turned";
            this.cbxTurned.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(12, 41);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtScore
            // 
            this.txtScore.Location = new System.Drawing.Point(381, 44);
            this.txtScore.Name = "txtScore";
            this.txtScore.ReadOnly = true;
            this.txtScore.Size = new System.Drawing.Size(100, 20);
            this.txtScore.TabIndex = 4;
            // 
            // cbxTimer
            // 
            this.cbxTimer.AutoSize = true;
            this.cbxTimer.Location = new System.Drawing.Point(584, 12);
            this.cbxTimer.Name = "cbxTimer";
            this.cbxTimer.Size = new System.Drawing.Size(52, 17);
            this.cbxTimer.TabIndex = 6;
            this.cbxTimer.Text = "Timer";
            this.cbxTimer.UseVisualStyleBackColor = true;
            // 
            // btnRotate
            // 
            this.btnRotate.Location = new System.Drawing.Point(205, 12);
            this.btnRotate.Name = "btnRotate";
            this.btnRotate.Size = new System.Drawing.Size(75, 23);
            this.btnRotate.TabIndex = 7;
            this.btnRotate.Text = "Rotate";
            this.btnRotate.UseVisualStyleBackColor = true;
            this.btnRotate.Click += new System.EventHandler(this.btnRotate_Click);
            // 
            // btnLower
            // 
            this.btnLower.Location = new System.Drawing.Point(205, 41);
            this.btnLower.Name = "btnLower";
            this.btnLower.Size = new System.Drawing.Size(75, 23);
            this.btnLower.TabIndex = 8;
            this.btnLower.Text = "Lower";
            this.btnLower.UseVisualStyleBackColor = true;
            this.btnLower.Click += new System.EventHandler(this.btnLower_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(124, 41);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(75, 23);
            this.btnLeft.TabIndex = 9;
            this.btnLeft.Text = "Left";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(286, 41);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(75, 23);
            this.btnRight.TabIndex = 10;
            this.btnRight.Text = "Right";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(381, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 11;
            this.label1.Text = "Score";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(615, 42);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 12;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(487, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 15);
            this.label2.TabIndex = 13;
            this.label2.Text = "input";
            // 
            // txtInput
            // 
            this.txtInput.Location = new System.Drawing.Point(487, 44);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(100, 20);
            this.txtInput.TabIndex = 14;
            this.txtInput.TextChanged += new System.EventHandler(this.txtInput_TextChanged);
            // 
            // TetrisServerControlsForm
            // 
            this.ClientSize = new System.Drawing.Size(729, 608);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnLower);
            this.Controls.Add(this.btnRotate);
//            this.Controls.Add(this.cbxTimer);
            this.Controls.Add(this.txtScore);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.cbxTurned);
            this.Controls.Add(this.btnStart);
            this.Name = "TetrisServerControlsForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }*/
        #endregion
        
        //constructor:
        public TetrisServerControlsForm(TetrisGame TetrisGameInput)
        {
            InitializeComponent();
            fillInBorders();
            tetrisgame = TetrisGameInput;
            //prevent flickering:
            this.DoubleBuffered = true;
            this.FormClosed += new FormClosedEventHandler(TetrisServerControlsForm_FormClosed);
        }

        void TetrisServerControlsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MessageBox.Show("Restart the server to reinitialize the Tetris Form");
        }
     
        protected override void OnPaint(PaintEventArgs paintEvnt)
        {
            #region Paint the form
            // Get the graphics object
            Graphics gfx = paintEvnt.Graphics;

            //grid disabled (uncomment following code to enable black grid)
            #region draw grid
            ////--------------DRAW grid--------------------------
            //// Create a new pen that we shall use for drawing the line
            //Pen myPen = new Pen(Color.Black);
            //// Loop and create a new horizaontal line SQUAREPIXELWIDTH pixels below the last one
            //for (int yi = 0; yi <= NRSQUARESINHEIGHT; yi++)
            //{
            //    gfx.DrawLine(myPen, XUPLEFT,
            //                        YUPLEFT + yi * SQUAREPIXELWIDTH,
            //                        XUPLEFT + NRSQUARESINWIDTH * SQUAREPIXELWIDTH,
            //                        YUPLEFT + yi * SQUAREPIXELWIDTH);
            //}
            ////Loop and create a vertical line SQUAREPIXELWIDTH pixels next to the last one
            //for (int xi = 0; xi <= NRSQUARESINWIDTH; xi++)
            //{
            //    gfx.DrawLine(myPen, XUPLEFT + xi * SQUAREPIXELWIDTH,
            //                        YUPLEFT,
            //                        XUPLEFT + xi * SQUAREPIXELWIDTH,
            //                        YUPLEFT + NRSQUARESINHEIGHT * SQUAREPIXELWIDTH);
            //}
            #endregion

            //Not done anymore...:
            #region colour border
            /* This is nowdone in the step below!!!
            //------------COLOUR border---------------
            //choose a colour: a grey scale from 0 to 255:
            int greyscale = 120;
            Color brushColor = Color.FromArgb(greyscale, greyscale, greyscale);
            // The brush is solid because we want a solid rectangle
            SolidBrush myBrush = new SolidBrush(brushColor);
           
            // Colour the lower and upper sides
            for (int xi = 0; xi <= NRSQUARESINWIDTH-1; xi++)
            {
                // Actually colour the upper squares
                gfx.FillRectangle(myBrush,  XUPLEFT + 1 + xi * SQUAREPIXELWIDTH,
                                            YUPLEFT + 1,
                                            SQUAREPIXELWIDTH-1, 
                                            SQUAREPIXELWIDTH-1);
                // Actually colour the lower squares
                gfx.FillRectangle(myBrush,  XUPLEFT + 1 + xi * SQUAREPIXELWIDTH,
                                            YUPLEFT + 1 + (NRSQUARESINHEIGHT - 1) * SQUAREPIXELWIDTH,
                                            SQUAREPIXELWIDTH-1, 
                                            SQUAREPIXELWIDTH-1);
            }
            // Colour the left and right sides
            for (int yi = 0; yi <= NRSQUARESINHEIGHT - 1; yi++)
            {
                // Actually colour the upper squares
                gfx.FillRectangle(myBrush,  XUPLEFT + 1 ,
                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                            SQUAREPIXELWIDTH - 1,
                                            SQUAREPIXELWIDTH - 1);
                // Actually colour the lower squares
                gfx.FillRectangle(myBrush,  XUPLEFT + 1 + (NRSQUARESINWIDTH - 1) * SQUAREPIXELWIDTH,
                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                            SQUAREPIXELWIDTH - 1,
                                            SQUAREPIXELWIDTH - 1);
            }
            */
            #endregion
           

            #region display playing field

            //---------------- DISPLAY the playfield ------------------------
            int greyscale = 120;
            Color grey = Color.FromArgb(greyscale, greyscale, greyscale);
            Color white = Color.FromArgb(250, 250, 250);
            Color yellow = Color.FromArgb(240, 240, 0);
            Color orange = Color.FromArgb(238, 154, 0);
            Color red = Color.FromArgb(176, 23, 31);
            Color purple = Color.FromArgb(122, 55, 139);
            Color blue = Color.FromArgb(61,89,171);
            Color bluegreen = Color.FromArgb(0, 238, 118);
            Color green = Color.FromArgb(48, 128, 20);
            Color peach = Color.FromArgb(255, 218, 185);
            Color gold = Color.FromArgb(255, 193, 37);

            SolidBrush greyBrush = new SolidBrush(grey);
            SolidBrush whiteBrush = new SolidBrush(white);
            SolidBrush yellowBrush = new SolidBrush(yellow);
            SolidBrush orangeBrush = new SolidBrush(orange);
            SolidBrush redBrush = new SolidBrush(red);
            SolidBrush purpleBrush = new SolidBrush(purple);
            SolidBrush blueBrush = new SolidBrush(blue);
            SolidBrush bluegreenBrush = new SolidBrush(bluegreen);
            SolidBrush greenBrush = new SolidBrush(green);
            SolidBrush peachBrush = new SolidBrush(peach);
            SolidBrush goldBrush = new SolidBrush(gold);
            for (int xi = 0; xi < NRSQUARESINWIDTH; xi++)
            {
                for (int yi = 0; yi < NRSQUARESINHEIGHT; yi++)
                {
                    switch (tetrisgame.DynamicField[xi, yi])
                    {
                        case WHITE:
                            gfx.FillRectangle(whiteBrush,   XUPLEFT + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;

                        case YELLOW:
                            gfx.FillRectangle(yellowBrush, XUPLEFT + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;

                        case ORANGE:
                            gfx.FillRectangle(orangeBrush, XUPLEFT + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;

                        case RED:
                            gfx.FillRectangle(redBrush, XUPLEFT + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;

                        case PURPLE:
                            gfx.FillRectangle(purpleBrush, XUPLEFT + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;

                        case BLUE:
                            gfx.FillRectangle(blueBrush, XUPLEFT + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;

                        case BLUEGREEN:
                            gfx.FillRectangle(bluegreenBrush, XUPLEFT + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;

                        case GREEN:
                            gfx.FillRectangle(greenBrush, XUPLEFT + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;

                        case LINEFULL:
                            gfx.FillRectangle(peachBrush, XUPLEFT + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;

                        case GREY:
                            gfx.FillRectangle(greyBrush, XUPLEFT + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;

                        default:
                            //gold squares are errors!
                            gfx.FillRectangle(goldBrush,   XUPLEFT + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;
                    }
                }
            }

            #endregion

            //BACK field displayed is disabled (uncomment following code to enable it again)            
            #region display BACK field for testing purposes!!
            /*
            //---------------- DISPLAY the playfield ------------------------
            int xUpLeftNew = 400;
            
            for (int xi = 0; xi < NRSQUARESINWIDTH; xi++)
            {
                for (int yi = 0; yi < NRSQUARESINHEIGHT; yi++)
                {
                    switch (tetrisgame.BackField[xi, yi])
                    {
                        case WHITE:
                            gfx.FillRectangle(whiteBrush, xUpLeftNew + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;

                        case YELLOW:
                            gfx.FillRectangle(yellowBrush, xUpLeftNew + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;

                        case ORANGE:
                            gfx.FillRectangle(orangeBrush, xUpLeftNew + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;

                        case RED:
                            gfx.FillRectangle(redBrush, xUpLeftNew + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;

                        case PURPLE:
                            gfx.FillRectangle(purpleBrush, xUpLeftNew + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;

                        case BLUE:
                            gfx.FillRectangle(blueBrush, xUpLeftNew + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;

                        case BLUEGREEN:
                            gfx.FillRectangle(bluegreenBrush, xUpLeftNew + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;

                        case GREEN:
                            gfx.FillRectangle(greenBrush, xUpLeftNew + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;

                        case GREY:
                            gfx.FillRectangle(greyBrush, xUpLeftNew + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;

                        default:
                            //gold squares are errors!
                            gfx.FillRectangle(goldBrush, xUpLeftNew + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;
                    }
                }
            }
            */
            #endregion

            #endregion
        }
                
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        
        private void refreshForm(Object myObject, EventArgs myEventArgs)
        {
            txtScore.Text = tetrisgame.Score.ToString();
            txtSecsTillUnpause.Text = tetrisgame.SecondsTillUnpause.ToString();
            //update paint
            Invalidate();                
            
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            refreshTimer = new Timer();
            refreshTimer.Interval = 100;
            refreshTimer.Tick += new EventHandler(refreshForm);
            refreshTimer.Start();

            tetrisgame.start();
            txtInput.Focus();
        }
        
        private void fillInBorders()
        {
            for (int i = 0; i < NRSQUARESINWIDTH; i++)
            {
                field[i, 0] = GREY;
                field[i, 1] = GREY;
                field[i, NRSQUARESINHEIGHT - 2] = GREY;
                field[i, NRSQUARESINHEIGHT - 1] = GREY;
            }
            for (int j = 0; j < NRSQUARESINHEIGHT; j++)
            {
                field[0, j] = GREY;
                field[1, j] = GREY;
                field[NRSQUARESINWIDTH - 2, j] = GREY;
                field[NRSQUARESINWIDTH - 1, j] = GREY;
            }
        }

        private void btnRotate_Click(object sender, EventArgs e)
        {
            cbxTurned.Checked = !cbxTurned.Checked;
            tetrisgame.rotate();
            Invalidate();
            txtInput.Focus();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            tetrisgame.moveLeft();
            Invalidate();
            txtInput.Focus();
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            tetrisgame.moveRight();
            Invalidate();
            txtInput.Focus();
        }

        private void btnLower_Click(object sender, EventArgs e)
        {
            tetrisgame.lower();
            Invalidate();
            txtInput.Focus();
        }

        //needs work...
        private void btnReset_Click(object sender, EventArgs e)
        {
            tetrisgame.reset();
            txtInput.Focus();
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            string input = txtInput.Text;

            switch (input)
            {
                case DOWN:
                    tetrisgame.lower();
                    Invalidate();
                    break;
                case LEFT:
                    tetrisgame.moveLeft();
                    Invalidate();
                    break;
                case RIGHT:
                    tetrisgame.moveRight();
                    Invalidate();
                    break;
                case ROTATE:
                    tetrisgame.rotate();
                    Invalidate();
                    break;
                default:
                    break;
            }
            txtInput.Clear();
            txtInput.Focus();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            tetrisgame.pause();
            if (tetrisgame.Paused)
            {
                cbxPaused.Checked = true;
            }
            else
            {
                cbxPaused.Checked = false;
            }
            txtInput.Focus();
        }

        private void btnPauseFor_Click(object sender, EventArgs e)
        {
            uint secs;
            bool flag = uint.TryParse(txtSecs.Text, out secs);
            if (flag)
            {
                tetrisgame.pause(secs);
            }
            txtInput.Focus();
        }
    }
}
