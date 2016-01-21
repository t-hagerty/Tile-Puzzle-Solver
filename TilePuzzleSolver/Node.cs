using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilePuzzleSolver
{
    class Node
    {
        public int f = int.MaxValue; //The overall value f(x) = g + h, where g is the min distance from starting node, h the value of the heuristic function. Lower f value nodes are tested first because it's predicted they'll be the best move.
        public int g = int.MaxValue; //Min distance from starting node to this node.
        public int h = int.MaxValue; //Value of the heuristic function, the underestimated or exact (NEVER overestimated) distance it will take to get from this node to goal node.

        //public int row = -1;
        //public int col = -1;
        public int color = -1; //0 represents red, 1 orange, 2 yellow, 3 green, 4 blue, 5 purple, 6 pink.

        public Node parent = null; //Used to store sequential steps in the path, the start node will have no parent, the first step's parent will be start node, etc.
        public Node[] edges = new Node[4]; //0 index corresponds to edge in direction above the node (-1 row direction), 1 to the edge to the right, 2 to below, 3 to left. null if no edge in said direction.
        public bool[] isEdgeScented;
        public bool[] isEdgeOrangeScented;

        //public bool isScented = false; //true if the node gives either an orange scent (orange tile) or a lemon scent (purple tile).
        //public bool isScentOrange = false; //true if the space is orange scented, false if lemon scented (or no scent, but vaiable only checked when isScented is true)

        public Node()
        {

        }

        public Node(int tileType)
        {
            //row = r;
            //col = c;
            color = tileType;

            isEdgeScented = new bool[4];
            isEdgeOrangeScented = new bool[4];

            //if(color == 1 || color == 5)
            //{
                //isScented = true;

                //if(color == 1)
                //{
                    //isScentOrange = true;
                //}
            //}
        }

        public void resetNodeRelations()
        {
            edges = new Node[4];
            isEdgeScented = new bool[4];
            isEdgeOrangeScented = new bool[4];
            parent = null;
        }
    }
}
