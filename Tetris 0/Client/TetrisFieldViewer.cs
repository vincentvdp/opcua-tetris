using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Quickstarts.TetrisClient
{
    public partial class TetrisFieldViewer : Form
    {
        #region private fields
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
                
        private const int XUPLEFT = 12;
        private const int YUPLEFT = 90;
        private const int NRSQUARESINWIDTH = 10 + 4;
        private const int NRSQUARESINHEIGHT = 20 + 4;
        private int SQUAREPIXELWIDTH = 20;

        private string myMessage;
        private int score;
        private bool gameactive;
        private uint secsRemaining;
        private int[,] field;

        private PauseMethodClickedEventHandler onPauseClicked;

        private Timer refreshTimer;        

        #endregion

        #region getters and setters

        public PauseMethodClickedEventHandler OnPauseClicked
        {
            get
            {
                return onPauseClicked;
            }
            set
            {
                onPauseClicked = value;
            }
        }

        public string MyMessage
        {
            get
            {
                return myMessage;
            }
            set
            {
                myMessage = value;
            }
        }

        public int[,] Field
        {
            get
            {
                return field;
            }
            set
            {
                field = value;
            }
        }

        public int Score
        {
            get
            {
                return score;
            }
            set
            {
                score = value;
            }
        }

        public bool GameActive
        {
            get
            {
                return gameactive;
            }
            set
            {
                gameactive = value;
            }
        }

        public uint SecsRemaining
        {
            get
            {
                return secsRemaining;
            }
            set
            {
                secsRemaining = value;
            }
        }

        #endregion

        #region constructors

        public TetrisFieldViewer()
        {
            InitializeComponent();
            score = 0;
            field = new int[NRSQUARESINWIDTH, NRSQUARESINHEIGHT];
            fillInBorders();
            this.DoubleBuffered = true;

            //enable timer:
            refreshTimer = new Timer();
            refreshTimer.Tick += new EventHandler(refreshForm);
            refreshTimer.Interval = 500;
            refreshTimer.Start();

            //this.Shown = new EventHandler(refreshForm);
        }

        #endregion

        #region private methods

        private void startRefreshTimer(Object myObject, EventArgs myEventArgs)
        {
            refreshTimer = new Timer();
            refreshTimer.Tick += new EventHandler(refreshForm);
            refreshTimer.Interval = 500;
            refreshTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs paintEvnt)
        {
            // Get the graphics object
            Graphics gfx = paintEvnt.Graphics;
            
            #region display playing field

            //---------------- DISPLAY the playfield ------------------------
            int greyscale = 120;
            Color grey = Color.FromArgb(greyscale, greyscale, greyscale);
            Color white = Color.FromArgb(250, 250, 250);
            Color yellow = Color.FromArgb(240, 240, 0);
            Color orange = Color.FromArgb(238, 154, 0);
            Color red = Color.FromArgb(176, 23, 31);
            Color purple = Color.FromArgb(122, 55, 139);
            Color blue = Color.FromArgb(61, 89, 171);
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
                    switch (field[xi, yi])
                    {
                        case WHITE:
                            gfx.FillRectangle(whiteBrush, XUPLEFT + 1 + xi * SQUAREPIXELWIDTH,
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
                            gfx.FillRectangle(goldBrush, XUPLEFT + 1 + xi * SQUAREPIXELWIDTH,
                                                            YUPLEFT + 1 + yi * SQUAREPIXELWIDTH,
                                                            SQUAREPIXELWIDTH - 1,
                                                            SQUAREPIXELWIDTH - 1);
                            break;
                    }
                }
            }

            #endregion

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void refreshForm(Object myObject, EventArgs myEventArgs)
        {
            //only update the form if it is shown, no need for updates otherwise...
            if(this.Visible)
            {
                //update the games status
                switch (gameactive)
                {
                    case true:
                        lblGameActive.Text = "Game Active: yes";
                        break;
                    case false:
                        lblGameActive.Text = "Game Active: NO!";
                        break;
                    default:
                        lblGameActive.Text = "Game Active: ???!";
                        break;
                }                

                //update the score
                lblScore.Text = "Score: " + score.ToString();

                //update seconds remaining till unpause
                lblSecsRemaining.Text = secsRemaining.ToString();

                //update paint
                Invalidate();

                //update my message:
                txtMyMessage.Text = myMessage;
            }
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

        #endregion

        public delegate string PauseMethodClickedEventHandler(uint secondsToPause);

        private void btnMyPause_Click(object sender, EventArgs e)
        {
            if (onPauseClicked == null)
            {
                myMessage = "EventHandler OnPauseClicked NOT initialized...";
            }
            else
            {                
                uint secs;
                if (uint.TryParse(txtSecondsToPause.Text, out secs)) myMessage = onPauseClicked(secs);
                else myMessage = "seconds not correct Format";
            }
        }

        private void txtMyMessage_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }        
}
