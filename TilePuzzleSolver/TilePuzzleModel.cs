using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilePuzzleSolver
{
    class TilePuzzleModel
    {
        public int[,] tileColor;
        //0 represents Red, 1 Orange, 2 Yellow, 3 Green, 4 Blue, 5 Purple, 6 Pink
        public int rows;
        public int cols;

        public TilePuzzleModel(int r, int c, int[,] tileGridColors)
        {
            rows = r;
            cols = c;
            tileColor = tileGridColors;
        }

        public void resizePuzzle(int newRows, int newCols)
        {
            int[,] newSizePuzzle = new int[newRows, newCols];

            if (newRows >= rows)
            {
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        newSizePuzzle[r, c] = tileColor[r, c];
                    }
                    for (int c = cols; c < newCols; c++)
                    {
                        newSizePuzzle[r, c] = 6;
                    }
                }
                for (int r = rows; r < newRows; r++)
                {
                    for (int c = 0; c < newCols; c++)
                    {
                        newSizePuzzle[r, c] = 6;
                    }
                }
            }
            else
            {
                for(int r = 0; r < newRows; r++)
                {
                    for(int c = 0; c < newCols; c++)
                    {
                        newSizePuzzle[r, c] = tileColor[r, c];
                    }
                }
            }

            tileColor = newSizePuzzle;
            rows = newRows;
            cols = newCols;
        }

        public void solve()
        {
            bool isOrangeScented = false;

            //puzzleGraph, or nodeList or something? = buildGraph();
             
        }

        //public Node[,] buildGraph()
        //{

        //}

    }
}
