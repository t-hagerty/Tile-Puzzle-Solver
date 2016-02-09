using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilePuzzleSolver
{
    /// <summary>
    /// Represents an individual tile in the color tile puzzle. Tiles in the puzzle are represented as nodes with edges to each other for use in the
    /// path-finding algorithm.
    /// 
    /// Node contains color info represented as an int (0 - red, 1 - orange, 2 - yellow, 3 - green, 4 - blue, 5 - purple, 6 - pink), a list of all edges to other
    /// nodes, and information to be used to calculate the path through the puzzle (parent node to store the previous step if this node is on the path, f, g, h
    /// for use in the A*-based path-finding algorithm).
    /// </summary>
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
        
        /// <summary>
        /// Creates a new node with color pink by default.
        /// </summary>
        public Node()
        {
            color = 6;
            edges = new List<Edge>(4);
        }

        /// <summary>
        /// Creates a new node with the specified color
        /// </summary>
        /// <param name="tileType">The color of the new node. Should be an int from 0 - 6 (0 - red, 1 - orange, 2 - yellow, 3 - green, 4 - blue, 5 - purple, 6 - pink)</param>
        public Node(int tileType)
        {
            if(tileType < 0 || tileType > 6)
            {
                tileType = 6;
            }
            color = tileType;
            edges = new List<Edge>(4);
        }

        /// <summary>
        /// Resets the list of edges that go from this node to others.
        /// </summary>
        public void resetNodeRelations()
        {
            edges = new List<Edge>(4);
            parent = null;
        }

        /// <summary>
        /// Returns a list of all the edges that go from this node to others including row/columns and color of ther others, in a string
        /// </summary>
        /// <returns>A String list of all the edges that go from this node to others along with teir rows/cloumns and colors.</returns>
        public String edgesToString()
        {
            String edgeList = "";

            foreach(Edge anEdge in edges)
            {
                if(anEdge.childRow == -2 && anEdge.childCol == -2)
                {
                    if (anEdge.childNode.edges.Count > 0)
                    {
                        edgeList = edgeList + "Edge to dummy node which leads to " + anEdge.childNode.edges[0].childRow + ", " + anEdge.childNode.edges[0].childCol + " with color " + anEdge.childNode.edges[0].childNode.color + "\n";
                    }
                    else
                    {
                        edgeList = edgeList + "Edge to dummy node which has nowhere valid/useful to go back to \n";
                    }
                }
                else
                {
                    edgeList = edgeList + "Edge to " + anEdge.childRow + ", " + anEdge.childCol + " to tile with color " + anEdge.childNode.color + "\n";
                }
            }

            return edgeList;
        }
    }
}
