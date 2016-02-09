using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilePuzzleSolver
{
    /// <summary>
    /// Represents a directional, scented edge between two nodes. The parent node is the node that the edge goes from, to the child node.
    /// A player in the puzzle could not use this edge to go from the child node to the parent.
    /// 
    /// Scent refers to the rules of the puzzle concerning orange/purple tiles. See TilePuzzleModel for full explanation of rules.
    /// </summary>
    class Edge
    {
        public Node childNode;
        //in this class, parent will refer to the Node that contains this edge object. child will refer to the nod that the edge connects the parent to.
        //Note that there will likely be an equivalent edge going in the reverse direction, in which the parent in this edge is the child in the other, and vice versa.
        public int parentRow = -1;
        public int parentCol = -1;
        public int childRow = -1;
        public int childCol = -1;

        public bool isScented = false;
        public bool isOrangeScented = false;

        /// <summary>
        /// Constructor for an edge object that goes from one node to another (but not the other way) and can give a scent when traveling across it.
        /// </summary>
        /// <param name="rowOfParent">Row of the node that the edge goes from</param>
        /// <param name="colOfParent">Column of the node that the edge goes from</param>
        /// <param name="rowOfChild">Row of the node that the edge goes to</param>
        /// <param name="colOfChild">Column of the node that the edge goes to</param>
        /// <param name="child">The node that the edge goes to.</param>
        /// <param name="scentType">The scent that crossing this edge will give a player, if any. Input "Orange" or "orange" or "O" or "o" for orange scent,
        /// "Lemon" or "lemon" or "L" or "l" for lemon scent, or anything else for no scent.</param>
        public Edge(int rowOfParent, int colOfParent, int rowOfChild, int colOfChild, Node child, String scentType)
        {
            childNode = child;
            parentRow = rowOfParent;
            parentCol = colOfParent;
            childRow = rowOfChild;
            childCol = colOfChild;

            if(scentType.Equals("Orange") || scentType.Equals("orange") || scentType.Equals("O") || scentType.Equals("o"))
            {
                isScented = true;
                isOrangeScented = true;
            }
            else if(scentType.Equals("Lemon") || scentType.Equals("lemon") || scentType.Equals("L") || scentType.Equals("l"))
            {
                isScented = true;
                isOrangeScented = false;
            }

        }
    }
}
