using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
//using System.Timers;

namespace Quickstarts.TetrisServer
{
    // Contains two classes: the first is an actual game, the second one a simulated version of a game
    public class TetrisGame
    {
        #region private fields

        #region instance members
        
        //Define constants for different colors of the tetris field squares
        private const int WHITE = 0;
        private const int GREY = 10;
        private const int G = 10;
        private const int LINEFULL = 8;
        private const int X = 9;
        
        //The tetrisfield is represented by a matrix of int's (the int determines the color of the element (= square in visualization)
        private int[,] backField;
        private int[,] dynamicField;
        private bool newBlockNeeded;
        private int score;
        private const int M = 1;
        private int locX;
        private int locY;
        private int locX0;
        private int locY0;
        private const int XMAX = 9;
        private const int YMAX = 18;
        private int width;
        private int height;
        private int currentRot;
        private bool removalNeeded = false;
        Random rnd = new Random();
        private int[, ,] shapeNow;
        private bool gameOver;
        private bool paused;
        private object m_lock;

        private const int NRSQUARESINWIDTH = 10 + 4;
        private const int NRSQUARESINHEIGHT = 20 + 4;

        private static Timer turntimer;
        private uint secondsTillUnpause;

        #endregion

        #region fill in shapes
        //These matrices are representations of the different tetris block shapes
        //private int[,,] shape1 = new int[1, 2, 2];
        private int[, ,] shape1 = { { { 1, 1 }, { 1, 1 } } };
        //private int[,,] shape2 = new int[2, 3, 3];
        private int[, ,] shape2 = { { {0,0,0}, {0,2,2}, {2,2,0} },
                                { {0,2,0}, {0,2,2}, {0,0,2} } };
        //private int[,,] shape3 = new int[2, 3, 3];
        private int[, ,] shape3 = { { {0,0,0}, {3,3,0}, {0,3,3} },
                                { {0,3,0}, {3,3,0}, {3,0,0} } };
        //private int[,,] shape4 = new int[4, 3, 3];
        private int[, ,] shape4 = { { {0,0,0}, {4,4,4}, {0,0,4} },
                                { {0,4,4}, {0,4,0}, {0,4,0} },
                                { {0,0,0}, {4,0,0}, {4,4,4} },
                                { {0,4,0}, {0,4,0}, {4,4,0} }};
        //private int[,,] shape5 = new int[4, 3, 3];
        private int[, ,] shape5 = { { {0,0,0}, {5,5,5}, {5,0,0} },
                                { {0,5,0}, {0,5,0}, {0,5,5} },
                                { {0,0,0}, {0,0,5}, {5,5,5} },
                                { {5,5,0}, {0,5,0}, {0,5,0} }};
        //private int[,,] shape6 = new int[2, 4, 4];
        private int[, ,] shape6 = { { {0,0,0,0}, {6,6,6,6}, {0,0,0,0}, {0,0,0,0} },
                                { {0,6,0,0}, {0,6,0,0}, {0,6,0,0}, {0,6,0,0} } };
        //private int[,,] shape7 = new int[4, 3, 3];
        private int[, ,] shape7 = { { {0,0,0}, {0,7,0}, {7,7,7} },
                                { {0,0,7}, {0,7,7}, {0,0,7} },
                                { {0,0,0}, {7,7,7}, {0,7,0} },
                                { {7,0,0}, {7,7,0}, {7,0,0} }};

        #endregion

        #region "game over" shape definition
        // The status of the field when the games is over:
        private int[,] gameOverField = {    {G, G, G, G, G, G, G, G, G, G, G, G, G, G, G, G, G, G, G, G, G, G, G, G},
                                        {G, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, G},
                                        {G, 0, 0, X, X, X, X, 0, 0, X, X, X, 0, 0, X, X, 0, X, X, X, X, X, 0, G},
                                        {G, 0, X, 0, 0, 0, X, 0, X, 0, X, 0, 0, X, 0, 0, 0, X, 0, X, 0, X, 0, G},
                                        {G, 0, X, 0, X, 0, X, 0, X, 0, X, 0, 0, 0, X, 0, 0, X, 0, 0, 0, X, 0, G},
                                        {G, 0, X, 0, X, X, X, 0, 0, X, X, X, 0, X, 0, 0, 0, X, 0, 0, 0, X, 0, G},
                                        {G, 0, 0, 0, X, 0, 0, 0, 0, 0, 0, 0, 0, 0, X, X, 0, 0, 0, 0, 0, 0, 0, G},
                                        {G, 0, 0, 0, 0, 0, 0, X, X, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, G},
                                        {G, 0, X, X, X, X, 0, 0, 0, X, 0, 0, X, X, X, X, X, 0, X, X, X, X, X, G},
                                        {G, 0, X, 0, 0, X, 0, 0, 0, 0, X, 0, X, 0, X, 0, X, 0, X, 0, X, 0, 0, G},
                                        {G, 0, X, 0, 0, X, 0, 0, 0, X, 0, 0, X, 0, 0, 0, X, 0, X, 0, X, 0, 0, G},
                                        {G, 0, X, X, X, X, 0, X, X, 0, 0, 0, X, 0, 0, 0, X, 0, X, X, 0, X, X, G},
                                        {G, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, G},
                                        {G, G, G, G, G, G, G, G, G, G, G, G, G, G, G, G, G, G, G, G, G, G, G, G}};

       #endregion

        #endregion

        #region Constructor

        public TetrisGame()
        {
            //some initialization
            width = NRSQUARESINWIDTH;
            height = NRSQUARESINHEIGHT;
            backField = new int[width, height];
            dynamicField = new int[width, height];
            newBlockNeeded = true;
            score = 0;
            locX0 = width / 2 - 1;
            locY0 = 2;
            locX = locX0;
            locY = locY0;
            currentRot = 0;
            fillInBackFieldBorders();
            dynamicField = (int[,])backField.Clone();
            gameOver = false;
            paused = true;
            secondsTillUnpause = 0;
            m_lock = new object();
        }
        #endregion

        #region getters and setters
        //------------getters and setter--------
        public int[,] DynamicField { get { return dynamicField; } }
        public int[,] BackField { get { return backField; } }
        public int Score { get { return score; } }
        public bool GameOver { get { return gameOver; } }
        public bool Paused { get { return paused; } }
        public uint SecondsTillUnpause { get { return secondsTillUnpause; } }

        //------------general methods---------------
        #endregion

        #region Public Methods

        public void start()
        {
            secondsTillUnpause = 0;
            paused = false;
            if (turntimer == null)
            {
                turntimer = new Timer(updateField, null, 1000, 1000);
            }
        }

        public void reset()
        {
            lock (m_lock)
            {
                backField = new int[width, height];
                dynamicField = new int[width, height];
                newBlockNeeded = true;
                score = 0;
                locX0 = width / 2 - 1;
                locY0 = 2;
                locX = locX0;
                locY = locY0;
                currentRot = 0;
                fillInBackFieldBorders();
                dynamicField = (int[,])backField.Clone();
                gameOver = false;
                paused = false;                
                turntimer.Dispose();
                turntimer = null;
                secondsTillUnpause = 0;
                start();
            }
        }

        public void pause()
        {
            secondsTillUnpause = 0;
            paused = !paused;
        }

        public void pause(uint secsToPause)
        {
            if (secsToPause == 0) return;

            if (!(secondsTillUnpause == 0 && paused == true))
            {
                secondsTillUnpause = secsToPause;
                paused = true;
            }
        }

        public void lower()
        {
            if (gameOver == false && Paused == false)
            {
                if (removalNeeded == true)
                {
                    removalNeeded = false;
                    removeLines();
                }
                else
                {
                    if (newBlockNeeded)
                    {
                        newBlockNeeded = false;
                        locX = locX0;
                        locY = locY0;
                        currentRot = 0;
                        getNewRandomShape();

                        if (checkCollision() == true)
                        {
                            dynamicField = (int[,])gameOverField.Clone();
                            gameOver = true;
                            return;
                        }

                        //update dynamicField
                        addBlockToDynamicField();


                    }
                    else // just lower the block if possible:
                    {
                        //lower the block
                        locY++;
                        //check for collission
                        if (checkCollision() == true)
                        {
                            //move the block back up...
                            locY--;
                            //fix the block to the field
                            addBlockToBackField();
                            newBlockNeeded = true;
                            if (checkAndMarkLines() == true)
                            {
                                removalNeeded = true;
                            }
                        }
                        else
                        {
                            addBlockToDynamicField();
                        }
                    }
                }
            }
        }

        public void moveRight()
        {
            if (gameOver == false && Paused == false)
            {
                locX++;
                if (checkCollision() == true)
                {
                    locX--;
                }
                else
                {
                    addBlockToDynamicField();
                }
            }
        }

        public void moveLeft()
        {
            if (gameOver == false && Paused == false)
            {
                locX--;
                if (checkCollision() == true)
                {
                    locX++;
                }
                else
                {
                    addBlockToDynamicField();
                }
            }

        }

        public void rotate()
        {
            if (gameOver == false && Paused == false)
            {
                currentRot = (currentRot + 1) % shapeNow.GetLength(0);
                if (checkCollision() == true)
                {
                    //try to move the block left or right, one or two spaces:
                    locX++;
                    if (locX >= 1 && locX <= 10 && checkCollision() == false)
                    {
                        return; // this position is OK
                    }
                    else
                    {
                        locX++;
                        if (locX >= 1 && locX <= 10 && checkCollision() == false)
                        {
                            return; // this position is OK
                        }
                        else
                        {
                            locX -= 3;
                            if (locX >= 1 && locX <= 10 && checkCollision() == false)
                            {
                                return; // this position is OK
                            }
                            else
                            {
                                locX--;
                                if (locX >= 1 && locX <= 10 && checkCollision() == false)
                                {
                                    return; // this position is OK
                                }
                                else
                                {
                                    //Tried 4 other positions, but still can't rotate!
                                    currentRot = (currentRot + shapeNow.GetLength(0) - 1) % shapeNow.GetLength(0);
                                    //...so go back to previous rotation position and initial location:
                                    locX += 2;
                                }
                            }
                        }
                    }
                }
                addBlockToDynamicField();
            }
        }

        #endregion

        #region Private Methods

        //--------------helper methods----------------
        
        //this method is called whenever a timer interval elapse = the rate at which tetris blocks go down (each second).
        private void updateField(object state)
        {
            lock (m_lock)
            {
                //check if game should be Unpaused:
                switch (secondsTillUnpause)
                {
                    case 0:
                        break;
                    case 1:

                        secondsTillUnpause = 0;
                        paused = false;
                        break;
                    default: //secondsTillUnpause < 0
                        secondsTillUnpause--;
                        break;
                }
                //lower the active tetris block
                lower();
            }
        }

        //Called when one or more tetris lines are made: remove the lines, drop the field above and increment the score
        private void removeLines()
        {
            int nrLinesMade = 0;
            for (int yi = 21; yi > 1; yi--) // check each line whether it was marked as full
            {
                if (backField[2, yi] == LINEFULL) // check each line whether it was marked as full
                {
                    nrLinesMade++;
                    //if so, move all the squares above one down:
                    for (int k = yi; k > 2; k--)
                    {
                        for (int xi = 2; xi < 12; xi++)
                        {
                            //move line one down
                            backField[xi, k] = backField[xi, k - 1];
                        }
                    }
                    //above line should be made zero = WHITE:
                    for (int xi = 2; xi < 12; xi++)
                    {
                        backField[xi, 2] = WHITE;
                    }
                    yi++;
                }
                //same line has to be checked again since line above was lowered and is also possibly a full line            
            }
            switch (nrLinesMade)
            {
                case 1:
                    score += 10;
                    break;
                case 2:
                    score += 30;
                    break;
                case 3:
                    score += 60;
                    break;
                case 4:
                    score += 100;
                    break;
                default:
                    score = 999999;
                    break;
            }
        }

        // Check whether lines are made and mark these lines (change the color)
        private bool checkAndMarkLines()
        {
            bool returnValue = false;
            bool lineIsFull = false;
            for (int yi = 21; yi > 1; yi--)
            {
                if (lineIsFull)
                {
                    returnValue = true;
                    //mark the line as full = fill in number 8 = LINEFULL
                    for (int xi = 2; xi < 12; xi++)
                    {
                        backField[xi, yi] = LINEFULL;
                        dynamicField[xi, yi] = LINEFULL;
                    }
                    lineIsFull = false;
                }
                else
                {
                    lineIsFull = true; //assume this and only change if you find out it's not full
                    yi++; // same as above, assume line full, if so, the line just looked at needs to be done again, hence yi++;
                    for (int xi = 2; xi < 12; xi++)
                    {
                        if (backField[xi, yi - 1] == 0) // a zero occured so definately not a full line...
                        {
                            lineIsFull = false;
                            yi--; // assumption was wrong, line is not full, get to next line by undoing the yi++.
                            break; // brea from loop, remaining squares don't have to be checked...
                        }
                    }
                }
            }
            return returnValue;
        }

        // Called when the block enters an obstacle when lowered and will be add to the backfield, the static field on which one tetris block moves
        //(a new block after this katest has been added to the backfield)
        private void addBlockToBackField()
        {
            for (int xi = locX; xi < locX + shapeNow.GetLength(2); xi++)
            {
                for (int yi = locY; yi < locY + shapeNow.GetLength(1); yi++)
                {
                    backField[xi, yi] += shapeNow[currentRot, yi - locY, xi - locX];
                }
            }
        }

        //Add the active block to the dynamic field:
        // the dynamic field is a snapshot of the current status of the game (backfield + active tetris block)
        private void addBlockToDynamicField()
        {
            dynamicField = (int[,])backField.Clone();
            for (int xi = locX; xi < locX + shapeNow.GetLength(2); xi++)
            {
                for (int yi = locY; yi < locY + shapeNow.GetLength(1); yi++)
                {
                    dynamicField[xi, yi] = backField[xi, yi] + shapeNow[currentRot, yi - locY, xi - locX];
                }
            }
        }

        //Checks whether the current position of the block is legitimate (not the case if active block overlaps with backfield)
        private bool checkCollision()
        {
            if (locX >= 1 && locX <= 10 && locY >= 2 && locY <= 20)
            {
                for (int xi = locX; xi < locX + shapeNow.GetLength(2); xi++)
                {
                    for (int yi = locY; yi < locY + shapeNow.GetLength(1); yi++)
                    {
                        if (backField[xi, yi] != 0 && shapeNow[currentRot, yi - locY, xi - locX] != 0)
                        {
                            return true; //two squares both have a non-zero value
                        }
                    }
                }
            }
            else
            {
                //a collision occured since box already out of allowed zone:
                return true;
            }
            return false;
        }

        //Function called just once (at start or at reset) to set the borders of the field
        private void fillInBackFieldBorders()
        {
            for (int i = 0; i < width; i++)
            {
                backField[i, 0] = GREY;
                backField[i, 1] = GREY;
                backField[i, height - 2] = GREY;
                backField[i, height - 1] = GREY;
            }
            for (int j = 0; j < height; j++)
            {
                backField[0, j] = GREY;
                backField[1, j] = GREY;
                backField[width - 2, j] = GREY;
                backField[width - 1, j] = GREY;
            }
        }

        //Called when asked for a new block
        private void getNewRandomShape()
        {
            int newRandom = rnd.Next(1, 8);
            switch (newRandom)
            {
                case 1:
                    shapeNow = shape1;
                    break;
                case 2:
                    shapeNow = shape2;
                    break;
                case 3:
                    shapeNow = shape3;
                    break;
                case 4:
                    shapeNow = shape4;
                    break;
                case 5:
                    shapeNow = shape5;
                    break;
                case 6:
                    shapeNow = shape6;
                    break;
                case 7:
                    shapeNow = shape7;
                    break;
                default:
                    //will remain as previous! Should not oocur!
                    break;
            }
        }
        #endregion
    }

    //----------------------------------------------------------
    //----------------- Below: SIMULATED class------------------
    //----------------------------------------------------------

    public class TetrisSimulation    {

        #region private fields

        private static Timer turntimer;
        private int score;
        private bool gameover;
        private bool paused;
        private uint secondsTillUnpause;

        #endregion

        //-------------------------------------------------
        
        #region constructors

        public TetrisSimulation()
        {
            score = 0;
            gameover = false;
            turntimer = new Timer(myTurnTimerFunction, null, 1000, 2000);
            //turntimer.Tick += new EventHandler(lower);
            //turntimer.Interval = 2000;
            //turntimer.Start();
            paused = false;
            secondsTillUnpause = 0;
        }

        #endregion

        //-------------------------------------------------

        #region getters and setters

        public bool GameOver { get { return gameover; } }
        public int Score { get { return score; } }
        public bool Paused
        {
            get { return paused; }
            set { paused = value; }
        }
        public int[,] DynamicField { get { return null; } }
        public uint SecondsTillUnpause { get { return secondsTillUnpause; } }
        #endregion

        //-------------------------------------------------
        
        #region public methods
        
        public void lower()
        {            
            if(gameover)
            {
                gameover = false;
                score = 0;
            }
            else
            {
                Random rnd = new Random();

                int increment = rnd.Next(0,3); //increase score once out of four times
                if(increment == 0)
                {
                    int amount = rnd.Next(1,4)*10; // increase with a random amount 10, 20, 30 or 40
                    score += amount;
                }

                int rndGameOver = rnd.Next(0,20); // game over occurs at a random moment (just in this simulation of course)
                if(rndGameOver == 0)
                {
                    gameover = true;
                    score = 0;
                }
            }
        }

        #endregion

        //-------------------------------------------------

        #region private methods

        private void myTurnTimerFunction(object state)
        {
            if (!paused) lower();
            return;
        }

        #endregion
    }
}
