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

            //Until code to dynamically alter graph edges between nodes as the puzzle is editted is written (if at all realistically possible) must rebuild whole thing at once:
            resetGraphEdges(); 

            buildGraph();
            
            //A*-based pathfinding algorithm

            //return something to represent path, maybe ordered list of tuples? that the mainwindow.xaml.cs can use to draw/highlight the path to the user.
        }

        public void buildGraph()
        {
            for(int r = 0; r < rows; r++)
            {
                checkEdgeLeft(startNode, r, -1); //-1 as column so it will check node at col = 0
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
            if (nodeCol + 1 == cols)
            {
                return;
            }
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
                    if (isWaterElectrified(nodeRow, i))
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
                        else
                        {
                            return;
                        }
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
            if(nodeRow + 1 == rows)
            {
                return;
            }
            if (nodes[nodeRow + 1, nodeCol].color == 0 || nodes[nodeRow + 1, nodeCol].color == 2)
            {
                return;
            }
            else if (nodes[nodeRow + 1, nodeCol].color == 3 || nodes[nodeRow + 1, nodeCol].color == 6)
            {
                nodes[nodeRow + 1, nodeCol].edges[2] = checkedNode;
                //checkedNode.edges.Add(nodes[nodeRow + 1,nodeCol]);
            }
            else if (nodes[nodeRow + 1, nodeCol].color == 1)
            {
                nodes[nodeRow + 1, nodeCol].edges[2] = checkedNode;
                //checkedNode.edges.Add(nodes[nodeRow + 1,nodeCol]);
                //checkedNode.however this specific edge is signified.isEdgeScented = true;
                //checkedNode.however this specific edge is signified.isEdgeOrangeScented = true;
                //nodes[nodeRow + 1,nodeCol].however this specific edge is signified.isEdgeScented = true;
                //nodes[nodeRow + 1,nodeCol].however this specific edge is signified.isEdgeOrangeScented = true;
            }
            else if (nodes[nodeRow + 1, nodeCol].color == 4)
            {
                if (isWaterElectrified(nodeRow + 1, nodeCol))
                {
                    return;
                }
                else
                {
                    nodes[nodeRow + 1, nodeCol].edges[2] = checkedNode;
                    //checkedNode.edges.Add(nodes[nodeRow + 1,nodeCol]);
                }
            }
            else if (nodes[nodeRow + 1, nodeCol].color == 5)
            {
                if (rows == nodeRow + 2)
                {
                    //nodes[nodeRow + 1, nodeCol].edges.Add(checkedNode);
                    //checkedNode.edges.Add(nodes[nodeRow + 1, nodeCol]);
                    return;
                }

                int i = nodeRow + 1;

                while (i < rows && nodes[i, nodeCol].color == 5)
                {
                    i++;
                }

                if (nodes[i, nodeCol].color == 3 || nodes[i, nodeCol].color == 6)
                {
                    nodes[i, nodeCol].edges[2] = checkedNode;
                    //checkedNode.edges.Add(nodes[i, nodeCol]);
                }
                else if (nodes[i, nodeCol].color == 0)
                {
                    return;
                }
                else if (nodes[i, nodeCol].color == 2)
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
                else if (nodes[i, nodeCol].color == 4)
                {
                    if (isWaterElectrified(i, nodeCol))
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
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        nodes[i, nodeCol].edges[2] = checkedNode;
                        //checkedNode.edges.Add(nodes[i, nodeCol]);
                    }
                }
                else if (nodes[i, nodeCol].color == 1)
                {
                    nodes[i, nodeCol].edges[2] = checkedNode;
                    //checkedNode.edges.Add(nodes[i, nodeCol]);
                    //checkedNode.however this specific edge is signified.isEdgeScented = true;
                    //checkedNode.however this specific edge is signified.isEdgeOrangeScented = true;
                    //nodes[i, nodeCol].however this specific edge is signified.isEdgeScented = true;
                    //nodes[i, nodeCol].however this specific edge is signified.isEdgeOrangeScented = true;
                }
            }
        }

        public void resetGraphEdges()
        {
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    nodes[r, c].resetNodeRelations();
                }
            }

            startNode.resetNodeRelations();
            endNode.resetNodeRelations();
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
