using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilePuzzleSolver
{
    class TilePuzzleModel
    {
        public Node[,] nodes;
        public Node startNode, endNode;

        public int rows;
        public int cols;

        public TilePuzzleModel(int r, int c, int[,] tileGridColors)
        {
            rows = r;
            cols = c;

            startNode = new Node(6);
            endNode = new Node(6);
            nodes = new Node[rows, cols];
            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < cols; j++)
                {
                    nodes[i, j] = new Node(tileGridColors[i, j]);
                }
            }
        }

        public void resizePuzzle(int newRows, int newCols)
        {
            Node[,] newSizePuzzle = new Node[newRows, newCols];

            if (newRows >= rows)
            {
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        newSizePuzzle[r, c] = nodes[r, c];
                    }
                    for (int c = cols; c < newCols; c++)
                    {
                        newSizePuzzle[r, c] = new Node(6);
                    }
                }
                for (int r = rows; r < newRows; r++)
                {
                    for (int c = 0; c < newCols; c++)
                    {
                        newSizePuzzle[r, c] = new Node(6);
                    }
                }
            }
            else
            {
                for(int r = 0; r < newRows; r++)
                {
                    for(int c = 0; c < newCols; c++)
                    {
                        newSizePuzzle[r, c] = nodes[r, c];
                    }
                }
            }

            nodes = newSizePuzzle;
            rows = newRows;
            cols = newCols;
        }

        public void solve()
        {
            bool isPlayerOrangeScented = false;

            //method to loop through 2d array and reset all nodes' edges?

            buildGraph();
             
        }

        public void buildGraph()
        {
            for(int r = 0; r < rows; r++)
            {
                checkEdgeLeft(startNode, r, -1); //-1 as column so it will check node at col = 0

                //OLD CODE, refactored into checkEdgeLeft(node, int, int) for simplicity. Remove soon if no problems.
                //if(nodes[r,0].color == 0 || nodes[r, 0].color == 2)
                //{
                //    continue;
                //}
                //if(nodes[r, 0].color == 3 || nodes[r, 0].color == 6)
                //{
                //    nodes[r, 0].edges[3] = startNode;
                //    //startNode.edges.Add(nodes[r,0]);
                //}
                //if (nodes[r, 1].color == 1)
                //{
                //    nodes[r, 1].edges[3] = startNode;
                //    //startNode.edges.Add(nodes[r,1]);
                //    //startNode.however this specific edge is signified.isEdgeScented = true;
                //    //startNode.however this specific edge is signified.isEdgeOrangeScented = true;
                //}
                //else if(nodes[r, 0].color == 4)
                //{
                //    if((r - 1 >= 0 && nodes[r - 1, 0].color == 2) || (r + 1 < rows && nodes[r + 1, 0].color == 2) || (cols > 1 && nodes[r, 1].color == 2))
                //    {
                //        continue;
                //    }
                //    else
                //    {
                //        nodes[r, 0].edges[3] = startNode;
                //        //startNode.edges.Add(nodes[r,0]);
                //    }
                //}
                //else if(nodes[r, 0].color == 5)
                //{
                //    if(cols == 1)
                //    {
                //        endNode.edges[3] = startNode;
                //        //startNode.edges.Add(endNode);
                //        continue;
                //    }
                //    if(nodes[r, 0].color == 5)
                //    {
                //        int i = 1;

                //        while (i < cols && nodes[r,i].color == 5)
                //        {
                //            i++;
                //        }

                //        if(nodes[r, i].color == 3 || nodes[r, i].color == 6)
                //        {
                //            nodes[r, i].edges[3] = startNode;
                //            //startNode.edges.Add(nodes[r,i]);
                //        }
                //        else if(nodes[r, i].color == 0 || nodes[r, i].color == 2)
                //        {
                //            continue;
                //        }
                //        else if(nodes[r, i].color == 4)
                //        {
                //            if ((r - 1 >= 0 && nodes[r - 1, i].color == 2) || (r + 1 < rows && nodes[r + 1, i].color == 2) || (cols > 1 && nodes[r, i + 1].color == 2))
                //            {
                //                continue;
                //            }
                //            else
                //            {
                //                nodes[r, i].edges[3] = startNode;
                //                //startNode.edges.Add(nodes[r,i]);
                //            }
                //        }
                //        else if(nodes[r, i].color == 1)
                //        {
                //            nodes[r, i].edges[3] = startNode;
                //            //startNode.edges.Add(nodes[r,i]);
                //            //startNode.however this specific edge is signified.isEdgeScented = true;
                //            //startNode.however this specific edge is signified.isEdgeOrangeScented = true;
                //        }
                //    }

                //}
            }

            for(int r = 0; r < rows - 1; r++)
            {
                for(int c = 0; c < cols - 1; c++)
                {
                    //Check Nodes to the right and below the current node rather than in all 4 directions (doing so would perform duplicate checks)
                    checkEdgeLeft(nodes[r, c], r, c);
                    checkEdgeDown(nodes[r, c], r, c);
                }
            }

            for(int c = 0; c < cols - 1; c++)
            {
                checkEdgeLeft(nodes[rows - 1, c], rows -1, c);
            }

            for (int r = 0; r < rows - 1; r++)
            {
                //nodes[r, cols - 1].edges.Add(endNode); //Even if this node shouldnt connect to end, i.e. if node is red, but in any of these cases, the node is inaccessible otherwise, so it doesn't matter.
                checkEdgeDown(nodes[r, cols - 1], r, cols - 1);
            }
            //nodes[r, cols - 1].edges.Add(endNode);
        }

        public void checkEdgeLeft(Node checkedNode, int nodeRow, int nodeCol)
        {
            if (nodes[nodeRow, nodeCol + 1].color == 0 || nodes[nodeRow, nodeCol + 1].color == 2)
            {
                return;
            }
            else if (nodes[nodeRow, nodeCol + 1].color == 3 || nodes[nodeRow, nodeCol + 1].color == 6)
            {
                nodes[nodeRow, nodeCol + 1].edges[3] = checkedNode;
                //checkedNode.edges.Add(nodes[nodeRow,nodeCol + 1]);
            }
            else if (nodes[nodeRow, nodeCol + 1].color == 1)
            {
                nodes[nodeRow, nodeCol + 1].edges[3] = checkedNode;
                //checkedNode.edges.Add(nodes[nodeRow,nodeCol + 1]);
                //checkedNode.however this specific edge is signified.isEdgeScented = true;
                //checkedNode.however this specific edge is signified.isEdgeOrangeScented = true;
                //nodes[nodeRow,nodeCol+1].however this specific edge is signified.isEdgeScented = true;
                //nodes[nodeRow,nodeCol + 1].however this specific edge is signified.isEdgeOrangeScented = true;
            }
            else if (nodes[nodeRow, nodeCol + 1].color == 4)
            {
                if (isWaterElectrified(nodeRow, nodeCol + 1))
                {
                    return;
                }
                else
                {
                    nodes[nodeRow, nodeCol + 1].edges[3] = checkedNode;
                    //checkedNode.edges.Add(nodes[nodeRow,nodeCol + 1]);
                }
            }
            else if (nodes[nodeRow, nodeCol + 1].color == 5)
            {
                if (cols == nodeCol + 2)
                {
                    //endNode.edges.Add(checkedNode);
                    //checkedNode.edges.Add(endNode);
                    return;
                }
                
                int i = nodeCol + 1;

                while (i < cols && nodes[nodeRow, i].color == 5)
                {
                    i++;
                }

                if (nodes[nodeRow, i].color == 3 || nodes[nodeRow, i].color == 6)
                {
                    nodes[nodeRow, i].edges[3] = checkedNode;
                    //checkedNode.edges.Add(nodes[nodeRow,i]);
                }
                else if (nodes[nodeRow, i].color == 0)
                {
                    return;
                }
                else if(nodes[nodeRow, i].color == 2)
                {
                    if (checkedNode.color != 1)
                    {
                        //dummyNode = new Node(6);
                        //checkedNode.edges.Add(dummyNode);
                        //dummyNode.edges.Add(checkedNode);
                        //checkedNode.however this specific edge is signified.isEdgeScented = true;
                        //checkedNode.however this specific edge is signified.isEdgeOrangeScented = false;

                        //dummyNode.however this specific edge is signified.isEdgeScented = true;
                        //dummyNode.however this specific edge is signified.isEdgeOrangeScented = false;
                    }
                }
                else if (nodes[nodeRow, i].color == 4)
                {
                    if (isWaterElectrified(nodeRow, nodeCol + 1))
                    {
                        return;
                    }
                    else
                    {
                        nodes[nodeRow, i].edges[3] = checkedNode;
                        //checkedNode.edges.Add(nodes[nodeRow,i]);
                    }
                }
                else if (nodes[nodeRow, i].color == 1)
                {
                    nodes[nodeRow, i].edges[3] = checkedNode;
                    //checkedNode.edges.Add(nodes[nodeRow,i]);
                    //checkedNode.however this specific edge is signified.isEdgeScented = true;
                    //checkedNode.however this specific edge is signified.isEdgeOrangeScented = true;
                    //nodes[nodeRow,i].however this specific edge is signified.isEdgeScented = true;
                    //nodes[nodeRow,i].however this specific edge is signified.isEdgeOrangeScented = true;
                }

            }
        }

        public void checkEdgeDown(Node checkedNode, int nodeRow, int nodeCol)
        {
            
        }

        public bool isWaterElectrified(int waterRow, int waterCol)
        {
            if ((waterRow - 1 >= 0 && nodes[waterRow - 1, waterCol + 1].color == 2) || (waterRow + 1 < rows && nodes[waterRow + 1, waterCol + 1].color == 2) || (cols > waterCol + 2 && nodes[waterRow, waterCol + 2].color == 2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
