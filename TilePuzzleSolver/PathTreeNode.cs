using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilePuzzleSolver
{
    class PathTreeNode
    {
        public int row;
        public int col;
        public int height;
        public bool isOrangeStep;

        public PathTreeNode parent;
        public List<PathTreeNode> children;

        public PathTreeNode(int r, int c)
        {
            row = r;
            col = c;
            parent = null;
            height = 0;
            children = new List<PathTreeNode>(4); //Set initial capacity to 4 because most nodes won't have more than 4 edges, save memory space
            isOrangeStep = false;
        }

        public PathTreeNode(int r, int c, PathTreeNode parentOfNode)
        {
            row = r;
            col = c;
            parent = parentOfNode;
            children = new List<PathTreeNode>(4); //Set initial capacity to 4 because most nodes won't have more than 4 edges, save memory space

            height = 0;
            PathTreeNode child = this;
            while(child.parent != null)
            {
                height++;
                child = child.parent;
            }

            isOrangeStep = false;
        }

        public PathTreeNode(int r, int c, PathTreeNode parentOfNode, bool orangeStep)
        {
            row = r;
            col = c;
            parent = parentOfNode;
            children = new List<PathTreeNode>(4); //Set initial capacity to 4 because most nodes won't have more than 4 edges, save memory space

            height = 0;
            PathTreeNode child = this;
            while (child.parent != null)
            {
                height++;
                child = child.parent;
            }

            isOrangeStep = orangeStep;
        }

    }
}
