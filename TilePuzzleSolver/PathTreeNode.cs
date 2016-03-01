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
        //public List<PathTreeNode> children;

        /// <summary>
        /// Constructor for a step that only has set row/column position info, no parent/previous step
        /// </summary>
        /// <param name="r">Row of the tile this step represents</param>
        /// <param name="c">Column of the tile this step represents</param>
        public PathTreeNode(int r, int c)
        {
            row = r;
            col = c;
            parent = null;
            height = 0; //With no parent, height = 0
            //children = new List<PathTreeNode>(4); //Set initial capacity to 4 because most nodes won't have more than 4 edges, save memory space
            isOrangeStep = false;
        }

        /// <summary>
        /// Constructor for a step that has a parent in a tree of all possible paths (the step previous to this one)
        /// </summary>
        /// <param name="r">Row of the tile this step represents</param>
        /// <param name="c">Column of the tile this step represents</param>
        /// <param name="parentOfNode">The previous step taken in this possible path, the parent of this node in a tree of all possible paths</param>
        public PathTreeNode(int r, int c, PathTreeNode parentOfNode)
        {
            row = r;
            col = c;
            parent = parentOfNode;
            //children = new List<PathTreeNode>(4); //Set initial capacity to 4 because most nodes won't have more than 4 edges, save memory space

            height = 0;
            PathTreeNode child = this;
            while(child.parent != null)
            {
                height++;
                child = child.parent;
            }

            isOrangeStep = false;
        }

        /// <summary>
        /// Constructor for a step that can be set if it was made inside of an orange loop in the pathfinding algorithm or not
        /// </summary>
        /// <param name="r">Row of the tile this step represents</param>
        /// <param name="c">Column of the tile this step represents</param>
        /// <param name="parentOfNode">The previous step taken in this possible path, the parent of this node in a tree of all possible paths</param>
        /// <param name="orangeStep">true if this step was made while the player is orange-scented/step was made in an orange loop in the path-finding algorithm</param>
        public PathTreeNode(int r, int c, PathTreeNode parentOfNode, bool orangeStep)
        {
            row = r;
            col = c;
            parent = parentOfNode;
            //children = new List<PathTreeNode>(4); //Set initial capacity to 4 because most nodes won't have more than 4 edges, save memory space

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
