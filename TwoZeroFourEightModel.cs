﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twozerofoureight
{
    class TwoZeroFourEightModel : Model
    {
        protected int boardSize; // default is 4
        protected int[,] board;
        protected Random rand;
        protected int[] range;

        public TwoZeroFourEightModel() : this(4)
        {
            // default board size is 4 
        }

        public TwoZeroFourEightModel(int size)
        {
            boardSize = size;
            board = new int[boardSize, boardSize];
            range = Enumerable.Range(0, boardSize).ToArray();
            foreach (int i in range)
            {
                foreach (int j in range)
                {
                    board[i, j] = 0;
                }
            }
            rand = new Random();
            // initialize board
            HandleChanges();
        }

        public int[,] GetBoard()
        {
            return board;
        }

        private void AddRandomSlot()
        {
            while (true)
            {
                int x = rand.Next(boardSize);
                int y = rand.Next(boardSize);
                if (board[x, y] == 0)
                {
                    board[x, y] = 2;
                    return;
                }
            }
        }

        public bool checkGameOver()
        {
            var range = Enumerable.Range(0, boardSize);

            bool Check = false;
            foreach (int i in range)
            {
                foreach (int j in range)
                {
                    if (board[i, j] == 2048)
                    {
                        Check = true;
                    }

                }
            }
            return Check;
        }

        public int getScore() //method that return sumation of score
        {
            var range = Enumerable.Range(0, boardSize);
            int sum = 0;

            foreach (int i in range)
            {
                foreach (int j in range)
                {

                    sum = sum + board[i, j];
                }
            }
            return sum;
        }

        // Perform shift and merge to the left of the given array.
        protected bool ShiftAndMerge(int[] buffer)
        {
            bool changed = false; // whether the array has changed
            int pos = 0; // next available slot index
            int lastMergedSlot = -1; // last slot that resulted from merging
            foreach (int k in range)
            {
                if (buffer[k] != 0) // nonempty slot
                {
                    // check if we can merge with the previous slot
                    if (pos > 0 && pos - 1 > lastMergedSlot && buffer[pos - 1] == buffer[k])
                    {
                        // merge
                        buffer[pos - 1] *= 2;
                        buffer[k] = 0;
                        lastMergedSlot = pos - 1;
                        changed = true;
                    }
                    else
                    {
                        // shift to the next available slot
                        buffer[pos] = buffer[k];
                        if (pos != k)
                        {
                            buffer[k] = 0;
                            changed = true;
                        }
                        // move the next available slot
                        pos++;
                    }
                }
            }
            return changed;
        }

        protected void HandleChanges(bool changed = true)
        {
            // if the board has changed, add a new number
            // and notify all views
            if (changed)
            {
                AddRandomSlot();
                NotifyAll();
            }
        }

        public void PerformDown()
        {
            bool changed = false; // whether the board has changed
            foreach (int i in range)
            {
                int[] buffer = new int[boardSize];
                // extract the current column from bottom to top
                foreach (int j in range)
                {
                    buffer[boardSize - j - 1] = board[j, i];
                }
                // process the extracted array
                // also track changes
                changed = ShiftAndMerge(buffer) || changed;
                // copy back
                foreach (int j in range)
                {
                    board[j, i] = buffer[boardSize - j - 1];
                }
            }
            HandleChanges(changed);
        }




        public bool CheckGameOverFull()
        {
            int count = 0;


            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (board[i, j] != 0)
                    {
                        count++;
                    }
                }
            }

            if (count == 16)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        //First row
                        if (i == 0 && j == 0) //#1
                        {
                            if (board[i, j] == board[i, j + 1] || board[i, j] == board[i + 1, j])
                            {
                                return false;
                            }
                        }
                        else if (i == 0 && j != 0 && j != boardSize - 1) //#2,3
                        {
                            if (board[i, j] == board[i, j - 1] || board[i, j] == board[i, j + 1] || board[i, j] == board[i + 1, j])
                            {
                                return false;
                            }
                        }
                        else if (i == 0 && j == boardSize - 1) //#4
                        {
                            if (board[i, j] == board[i, j - 1] || board[i, j] == board[i + 1, j])
                            {
                                return false;
                            }
                        }
                        //Second Row and Third Row
                        else if (j == 0 && i != 0 && i != boardSize - 1) //#1
                        {
                            if (board[i, j] == board[i + 1, j] || board[i, j] == board[i, j + 1] || board[i, j] == board[i - 1, j])
                            {
                                return false;
                            }
                        }
                        //Second Row and Third Row
                        else if (j == boardSize - 1 && i != 0 && i != boardSize - 1) //#4
                        {
                            if (board[i, j] == board[i + 1, j] || board[i, j] == board[i, j - 1] || board[i, j] == board[i - 1, j])
                            {
                                return false;
                            }
                        }
                        //Forth Row
                        else if (i == boardSize - 1 && j == 0)
                        {
                            if (board[i, j] == board[i, j + 1] || board[i, j] == board[i - 1, j])
                            {
                                return false;
                            }
                        }
                        else if (i == boardSize - 1 && j != 0 && j != boardSize - 1) //#2,3
                        {
                            if (board[i, j] == board[i, j - 1] || board[i, j] == board[i, j + 1] || board[i, j] == board[i - 1, j])
                            {
                                return false;
                            }
                        }
                        else if (i == boardSize - 1 && j == boardSize - 1) //#1
                        {
                            if (board[i, j] == board[i, j - 1] || board[i, j] == board[i - 1, j])
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (board[i, j] == board[i, j - 1] || board[i, j] == board[i, j + 1] || board[i, j] == board[i + 1, j] || board[i, j] == board[i - 1, j])
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;

            }
            return false;
        }


        public void PerformUp()
        {
            if (checkGameOver() == false)
            {
                bool changed = false; // whether the board has changed
                foreach (int i in range)
                {
                    int[] buffer = new int[boardSize];
                    // extract the current column from top to bottom
                    foreach (int j in range)
                    {
                        buffer[j] = board[j, i];
                    }
                    // process the extracted array
                    // also track changes
                    changed = ShiftAndMerge(buffer) || changed;
                    // copy back
                    foreach (int j in range)
                    {
                        board[j, i] = buffer[j];
                    }
                }
                HandleChanges(changed);
            }
        }

        public void PerformRight()
        {
            if (checkGameOver() == false)
            {
                bool changed = false; // whether the board has changed
                foreach (int i in range)
                {
                    int[] buffer = new int[boardSize];
                    // extract the current column from right to left
                    foreach (int j in range)
                    {
                        buffer[boardSize - j - 1] = board[i, j];
                    }
                    // process the extracted array
                    // also track changes
                    changed = ShiftAndMerge(buffer) || changed;
                    // copy back
                    foreach (int j in range)
                    {
                        board[i, j] = buffer[boardSize - j - 1];
                    }
                }
                HandleChanges(changed);
            }
        }

        public void PerformLeft()
        {
            if (checkGameOver() == false)
            {
                bool changed = false; // whether the board has changed
                foreach (int i in range)
                {
                    int[] buffer = new int[boardSize];
                    // extract the current column from left to right
                    foreach (int j in range)
                    {
                        buffer[j] = board[i, j];
                    }
                    // process the extracted array
                    // also track changes
                    changed = ShiftAndMerge(buffer) || changed;
                    // copy back
                    foreach (int j in range)
                    {
                        board[i, j] = buffer[j];
                    }
                }
                HandleChanges(changed);
            }
        }
    }
}

