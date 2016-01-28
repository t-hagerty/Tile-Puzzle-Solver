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
        public List<Edge> edges;
        

        public Node()
        {
            color = 6;
            edges = new List<Edge>(4);
        }

        public Node(int tileType)
        {
            color = tileType;
            edges = new List<Edge>(4);
        }

        public void resetNodeRelations()
        {
            edges = new List<Edge>(4);
            parent = null;
        }
    }
}
