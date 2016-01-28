using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilePuzzleSolver
{
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
