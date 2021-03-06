﻿using System;
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
        public int weight;
        public int row;
        public int col;
        public int color = -1; //0 represents red, 1 orange, 2 yellow, 3 green, 4 blue, 5 purple, 6 pink. Set to 7 in special case that this node is a dummy node
        public List<Edge> edges;
        
        /// <summary>
        /// Creates a new node with color pink by default.
        /// </summary>
        public Node()
        {
            color = 6;
            edges = new List<Edge>(4); //Most nodes won't have more than 4 edges, save on memory
            weight = 0;
        }

        /// <summary>
        /// Creates a new node with the specified color
        /// </summary>
        /// <param name="tileType">The color of the new node. Should be an int from 0 - 6 (0 - red, 1 - orange, 2 - yellow, 3 - green, 4 - blue, 5 - purple, 6 - pink). 7 is reserved for a special case (dummy node)</param>
        public Node(int tileType, int r, int c)
        {
            if(tileType < 0 || tileType > 7)
            {
                tileType = 6;
            }
            color = tileType;
            row = r;
            col = c;
            weight = 0;
            edges = new List<Edge>(4);
        }

        /// <summary>
        /// Resets the list of edges that go from this node to others.
        /// </summary>
        public void resetNodeRelations()
        {
            edges = new List<Edge>(4);
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
                if (anEdge.isScented)
                {
                    if (anEdge.isOrangeScented)
                    {
                        edgeList = edgeList + "Edge to " + anEdge.childNode.row + ", " + anEdge.childNode.col + " to tile with color " + anEdge.childNode.color + " with orange scent" + "\n";
                    }
                    else
                    {
                        if (anEdge.childNode.color == 7) //edge slides over purple tile(s) into yellow tile, which sends back (dummy node)
                        {
                            int returnRow = anEdge.childNode.edges[0].childNode.row;
                            int returnCol = anEdge.childNode.edges[0].childNode.col;

                            edgeList = edgeList + "Edge to " + anEdge.childNode.row + ", " + anEdge.childNode.col + " to tile with color 2 with lemon scent, which forces back to " + returnRow + ", " + returnCol + "\n";
                        }
                        else
                        {
                            edgeList = edgeList + "Edge to " + anEdge.childNode.row + ", " + anEdge.childNode.col + " to tile with color " + anEdge.childNode.color + " with lemon scent" + "\n";
                        }
                    }
                }
                else
                {
                    edgeList = edgeList + "Edge to " + anEdge.childNode.row + ", " + anEdge.childNode.col + " to tile with color " + anEdge.childNode.color + "\n";
                }
            }

            if(edgeList == "")
            {
                edgeList = "No edges";
            }

            return edgeList;
        }
    }
}
