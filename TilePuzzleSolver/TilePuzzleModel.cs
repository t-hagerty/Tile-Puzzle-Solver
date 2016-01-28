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
            if (checkedNode.color == 0)
            {
                return;
            }
            if (nodeCol + 1 == cols)
            {
                return;
            }
            if (checkedNode.color == 4 && isWaterElectrified(nodeRow, nodeCol) && nodes[nodeRow, nodeCol + 1].color != 5)
            {
                return;
            }
            if (checkedNode.color == 2 && nodes[nodeRow, nodeCol + 1].color != 5)
            {
                return;
            }
            if (nodes[nodeRow, nodeCol + 1].color == 0 || nodes[nodeRow, nodeCol + 1].color == 2)
            {
                return;
            }
            else if (nodes[nodeRow, nodeCol + 1].color == 3 || nodes[nodeRow, nodeCol + 1].color == 6)
            {
                addEdges(nodeRow, nodeCol, nodeRow, nodeCol + 1, checkedNode, nodes[nodeRow, nodeCol + 1], false);
            }
            else if (nodes[nodeRow, nodeCol + 1].color == 1)
            {
                addEdges(nodeRow, nodeCol, nodeRow, nodeCol + 1, checkedNode, nodes[nodeRow, nodeCol + 1], false);
            }
            else if (nodes[nodeRow, nodeCol + 1].color == 4)
            {
                if (isWaterElectrified(nodeRow, nodeCol + 1))
                {
                    return;
                }
                else
                {
                    addEdges(nodeRow, nodeCol, nodeRow, nodeCol + 1, checkedNode, nodes[nodeRow, nodeCol + 1], false);
                }
            }
            else if (nodes[nodeRow, nodeCol + 1].color == 5)
            {
                int i = nodeCol + 1;

                while (i < cols && nodes[nodeRow, i].color == 5)
                {
                    i++;
                }
                if (cols == i + 1)
                {
                    addEdges(nodeRow, nodeCol, nodeRow, i, checkedNode, endNode, true);
                    return;
                }
                if (checkedNode.color == 2 || (checkedNode.color == 4 && isWaterElectrified(nodeRow,nodeCol)))
                {
                    Node dummyNode = new Node(6);
                    addEdges(nodeRow, i, -1, -1, nodes[nodeRow, i], dummyNode, true);
                }

                if (nodes[nodeRow, i].color == 3 || nodes[nodeRow, i].color == 6)
                {
                    addEdges(nodeRow, nodeCol, nodeRow, i, checkedNode, nodes[nodeRow, i], true);
                }
                else if (nodes[nodeRow, i].color == 0)
                {
                    return;
                }
                else if(nodes[nodeRow, i].color == 2)
                {
                    if (checkedNode.color != 1) //No reason to make the dummy node/edge here if the tile we'd return back to will re-give orange scent
                    {
                        Node dummyNode = new Node(6);
                        addEdges(nodeRow, nodeCol, -1, -1, checkedNode, dummyNode, true);
                    }
                }
                else if (nodes[nodeRow, i].color == 4)
                {
                    if (isWaterElectrified(nodeRow, i))
                    {
                        if (checkedNode.color != 1) //No reason to make the dummy node/edge here if the tile we'd return back to will re-give orange scent
                        {
                            Node dummyNode = new Node(6);
                            addEdges(nodeRow, nodeCol, -1, -1, checkedNode, dummyNode, true);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        addEdges(nodeRow, nodeCol, nodeRow, i, checkedNode, nodes[nodeRow, i], true);
                    }
                }
                else if (nodes[nodeRow, i].color == 1)
                {
                    addEdges(nodeRow, nodeCol, nodeRow, i, checkedNode, nodes[nodeRow, i], true);
                }

            }
        }

        public void checkEdgeDown(Node checkedNode, int nodeRow, int nodeCol)
        {
            if(checkedNode.color == 0)
            {
                return;
            }
            if (nodeRow + 1 == rows)
            {
                return;
            }
            if (checkedNode.color == 4 && isWaterElectrified(nodeRow, nodeCol) && nodes[nodeRow + 1, nodeCol].color != 5)
            {
                return;
            }
            if (checkedNode.color == 2 && nodes[nodeRow + 1, nodeCol].color != 5)
            {
                return;
            }
            if (nodes[nodeRow + 1, nodeCol].color == 0 || nodes[nodeRow + 1, nodeCol].color == 2)
            {
                return;
            }
            else if (nodes[nodeRow + 1, nodeCol].color == 3 || nodes[nodeRow + 1, nodeCol].color == 6)
            {
                addEdges(nodeRow, nodeCol, nodeRow + 1, nodeCol, checkedNode, nodes[nodeRow + 1, nodeCol], false);
            }
            else if (nodes[nodeRow + 1, nodeCol].color == 1)
            {
                addEdges(nodeRow, nodeCol, nodeRow + 1, nodeCol, checkedNode, nodes[nodeRow + 1, nodeCol], false);
            }
            else if (nodes[nodeRow + 1, nodeCol].color == 4)
            {
                if (isWaterElectrified(nodeRow + 1, nodeCol))
                {
                    return;
                }
                else
                {
                    addEdges(nodeRow, nodeCol, nodeRow + 1, nodeCol, checkedNode, nodes[nodeRow + 1, nodeCol], false);
                }
            }
            else if (nodes[nodeRow + 1, nodeCol].color == 5)
            {
                int i = nodeRow + 1;

                while (i < rows && nodes[i, nodeCol].color == 5)
                {
                    i++;
                }
                if (rows == i)
                {
                    addEdges(nodeRow, nodeCol, i - 1, nodeCol, checkedNode, nodes[i - 1, nodeCol], true);
                    return;
                }
                else if (checkedNode.color == 2 || (checkedNode.color == 4 && isWaterElectrified(nodeRow,nodeCol)))
                {
                    Node dummyNode = new Node(6);
                    addEdges(i, nodeCol, -1, -1, nodes[i, nodeCol], dummyNode, true);
                }
                else if (nodes[i, nodeCol].color == 3 || nodes[i, nodeCol].color == 6)
                {
                    addEdges(nodeRow, nodeCol, i, nodeCol, checkedNode, nodes[i, nodeCol], true);
                }
                else if (nodes[i, nodeCol].color == 0)
                {
                    return;
                }
                else if (nodes[i, nodeCol].color == 2)
                {
                    if (checkedNode.color != 1)
                    {
                        Node dummyNode = new Node(6);
                        addEdges(nodeRow, nodeCol, -1, -1, checkedNode, dummyNode, true);
                    }
                }
                else if (nodes[i, nodeCol].color == 4)
                {
                    if (isWaterElectrified(i, nodeCol))
                    {
                        if (checkedNode.color != 1)
                        {
                            Node dummyNode = new Node(6);
                            addEdges(nodeRow, nodeCol, -1, -1, checkedNode, dummyNode, true);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        addEdges(nodeRow, nodeCol, i, nodeCol, checkedNode, nodes[i, nodeCol], true);
                    }
                }
                else if (nodes[i, nodeCol].color == 1)
                {
                    addEdges(nodeRow, nodeCol, i, nodeCol, checkedNode, nodes[i, nodeCol], true);
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
        
        public void addEdges(int row1, int col1, int row2, int col2, Node node1, Node node2, bool goesOverPurple)
        {
            if(node1.color == 1)
            {
                node2.edges.Add(new Edge(row1, col1, row2, col2, node1, "orange"));
            }
            else if(goesOverPurple)
            {
                node2.edges.Add(new Edge(row1, col1, row2, col2, node1, "lemon"));
            }
            else
            {
                node2.edges.Add(new Edge(row1, col1, row2, col2, node1, ""));
            }
            if (node2.color == 1)
            {
                node1.edges.Add(new Edge(row2, col2, row1, col1, node2, "orange"));
            }
            else if(goesOverPurple)
            {
                node1.edges.Add(new Edge(row2, col2, row1, col1, node2, "lemon"));
            }
            else
            {
                node1.edges.Add(new Edge(row2, col2, row1, col1, node2, ""));
            }

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
