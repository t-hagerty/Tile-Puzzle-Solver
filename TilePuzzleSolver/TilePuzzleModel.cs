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

            if (newRows > rows || newCols > cols)
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
                checkForEdgeInDirection(startNode, r, -1, 1, 0); //-1 as column so it will check node at col = 0
            }

            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    if((r == 1 || r == 2) && c == 24)
                    {
                        r = r;
                    }
                    //Check Nodes to the right and below the current node rather than in all 4 directions (doing so would perform duplicate checks)
                    findEdgesForNode(nodes[r, c], r, c);
                }
            }

            for (int r = 0; r < rows; r++)
            {
                nodes[r, cols - 1].edges.Add(new Edge(r, cols - 1, r, cols, endNode, "")); //Even if this node shouldnt connect to end, i.e. if node is red, but in any of these cases, the node is inaccessible otherwise, so it doesn't matter.
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

        public void findEdgesForNode(Node nodeBeingChecked, int row, int col)
        {
                switch (nodeBeingChecked.color)
            {
                case 0:
                    break;
                case 1:
                    checkForEdgeInDirection(nodeBeingChecked, row, col, 1, 0);
                    checkForEdgeInDirection(nodeBeingChecked, row, col, 0, 1);
                    break;
                case 2:
                    if (col + 1 < cols && nodes[row, col + 1].color == 5) //Need to check if a dummy node needs to be made for a leftward direction because we normally only check rightwards (which is sufficient except in special cases like this)
                    {
                        int i = col + 2;
                        while (i < cols && nodes[row, i].color == 5)
                        {
                            i++;
                        }
                        if (i == cols) { i--; }
                        checkForEdgeInDirection(nodes[row, i], row, i, 0, -1);
                    }
                    if (row + 1 < rows && nodes[row + 1, col].color == 5) //Need to check if a dummy node needs to be made for an upward direction because we normally only check downwards (which is sufficient except in special cases like this)
                    {
                        int i = row + 2;
                        while (i < rows && nodes[i, col].color == 5)
                        {
                            i++;
                        }
                        if (i == rows) { i--; }
                        checkForEdgeInDirection(nodes[i, col], i, col, -1, 0);
                    }
                    break;
                case 3:
                    checkForEdgeInDirection(nodeBeingChecked, row, col, 1, 0);
                    checkForEdgeInDirection(nodeBeingChecked, row, col, 0, 1);
                    break;
                case 4:
                    if (isWaterElectrified(row, col))
                    {
                        if (col + 1 < cols && nodes[row, col + 1].color == 5) //Need to check if a dummy node needs to be made for a leftward direction because we normally only check rightwards (which is sufficient except in special cases like this)
                        {
                            int i = col + 2;
                            while (i < cols && nodes[row, i].color == 5)
                            {
                                i++;
                            }
                            if (i == cols) { i--; }
                            checkForEdgeInDirection(nodes[row, i], row, i, 0, -1);
                        }
                        if (row + 1 < rows && nodes[row + 1, col].color == 5) //Need to check if a dummy node needs to be made for an upward direction because we normally only check downwards (which is sufficient except in special cases like this)
                        {
                            int i = row + 2;
                            while (i < rows && nodes[i, col].color == 5)
                            {
                                i++;
                            }
                            if (i == rows) { i--; }
                            checkForEdgeInDirection(nodes[i, col], i, col, -1, 0);
                        }
                    }
                    else
                    {
                        checkForEdgeInDirection(nodeBeingChecked, row, col, 1, 0);
                        checkForEdgeInDirection(nodeBeingChecked, row, col, 0, 1);
                    }
                    break;
                case 5:
                    /*
                    *Even if there's a electrified tile next to a purple tile, doesn't matter (don't have to worry about dummy nodes) because "player" already has lemon scent 
                    *if standing on purple, and any tile the dummy node would send them to is already accessible from the purple tile.
                    */

                    if (((row + 1 < rows && nodes[row + 1, col].color == 0) && 
                        (row - 1 >= 0 && !(nodes[row - 1, col].color == 0 || nodes[row - 1, col].color == 2 || (nodes[row - 1, col].color == 4 && isWaterElectrified(row - 1, col)))))
                        || ((row - 1 >= 0 && nodes[row - 1, col].color == 0) && 
                        (row + 1 < rows && !(nodes[row + 1, col].color == 0 || nodes[row + 1, col].color == 2 || (nodes[row + 1, col].color == 4 && isWaterElectrified(row + 1, col)))))
                        || (((col + 1 < cols && nodes[row, col + 1].color == 0) || col + 1 == cols) && 
                        (col - 1 >= 0 && !(nodes[row, col - 1].color == 0 || nodes[row, col - 1].color == 2 || (nodes[row, col - 1].color == 4 && isWaterElectrified(row, col - 1)))))
                        || (((col - 1 >= 0 && nodes[row, col - 1].color == 0) || col - 1 == -1) && 
                        (col + 1 < cols && !(nodes[row, col + 1].color == 0 || nodes[row, col + 1].color == 2 || (nodes[row, col + 1].color == 4 && isWaterElectrified(row, col + 1))))))
                    {
                        checkForEdgeInDirection(nodeBeingChecked, row, col, 1, 0);
                        checkForEdgeInDirection(nodeBeingChecked, row, col, 0, 1);
                        checkForEdgeInDirection(nodeBeingChecked, row, col, -1, 0);
                        checkForEdgeInDirection(nodeBeingChecked, row, col, 0, -1);
                    }
                    break;
                case 6:
                    checkForEdgeInDirection(nodeBeingChecked, row, col, 1, 0);
                    checkForEdgeInDirection(nodeBeingChecked, row, col, 0, 1);
                    break;
                default:
                    break;
            }
        }

        public void checkForEdgeInDirection(Node nodeBeingChecked, int row, int col, int dx, int dy)
        {
            if(row + dy < 0 || row + dy >= rows || col + dx < 0 || col + dx >= cols) { return; }

            Node adjacentNode = nodes[row + dy, col + dx];

            switch(adjacentNode.color)
            {
                case 0:
                    break;
                case 1:
                    addEdge(row, col, row + dy, col + dx, nodeBeingChecked, adjacentNode, false);
                    if (nodeBeingChecked.color == 5 && !(row - dy == 0 || row - dy == rows || col - dx == 0 || col - dx == cols || nodes[row -dy, col - dx].color == 0))
                    {
                        break;
                    }
                    else
                    {
                        addEdge(row + dy, col + dx, row, col, adjacentNode, nodeBeingChecked, false);
                    }
                    break;
                case 2:
                    break;
                case 3:
                    addEdge(row, col, row + dy, col + dx, nodeBeingChecked, adjacentNode, false);
                    if (nodeBeingChecked.color == 5 && !(row - dy == 0 || row - dy == rows || col - dx == 0 || col - dx == cols || nodes[row - dy, col - dx].color == 0))
                    {
                        break;
                    }
                    else
                    {
                        addEdge(row + dy, col + dx, row, col, adjacentNode, nodeBeingChecked, false);
                    }
                    break;
                case 4:
                    if(isWaterElectrified(row + dy, col + dx))
                    {
                        break;
                    }
                    else
                    {
                        addEdge(row, col, row + dy, col + dx, nodeBeingChecked, adjacentNode, false);
                        if (nodeBeingChecked.color == 5 && !(row - dy == 0 || row - dy == rows || col - dx == 0 || col - dx == cols || nodes[row - dy, col - dx].color == 0))
                        {
                            break;
                        }
                        else
                        {
                            addEdge(row + dy, col + dx, row, col, adjacentNode, nodeBeingChecked, false);
                        }
                        break;
                    }
                case 5:
                    int slideDX = dx + dx; //col + slideDX will always be the column of the row after the purple tile currently being looked at.
                    int slideDY = dy + dy;

                    while (row + slideDY >= 0 && row + slideDY < rows && col + slideDX >= 0 && col + slideDX < cols && adjacentNode.color == 5)
                    {
                        adjacentNode = nodes[row + slideDY, col + slideDX];
                        slideDX += dx;
                        slideDY += dy;
                    }

                    if(adjacentNode.color == 0)
                    {
                        nodes[row + slideDY - dy, col + slideDX - dx] = adjacentNode; //backtrack one, the purple tile before the red tile (wall/impassable) becomes where the edge stops
                    }

                    if(nodeBeingChecked.color != 1 && (adjacentNode.color == 2 || (adjacentNode.color == 4 && isWaterElectrified(row + slideDY - dy, col + slideDX - dx))))
                    {
                        Node dummyNode = new Node(6);
                        addEdge(row, col, -2, -2, nodeBeingChecked, dummyNode, true);
                        addEdge(-2, -2, row, col, dummyNode, nodeBeingChecked, true);
                    }
                    else
                    {
                        addEdge(row, col, row + slideDY - dy, col + slideDX - dx, nodeBeingChecked, adjacentNode, true);
                        if (nodeBeingChecked.color != 5)
                        {
                            addEdge(row + slideDY - dy, col + slideDX - dx, row, col, adjacentNode, nodeBeingChecked, true);
                        }
                        else if(nodes[row - dy, col - dx].color == 0 || (row - dy < 0 || row - dy >= rows || col - dx < 0 || col - dx >= cols))
                        {
                            addEdge(row + slideDY - dy, col + slideDX - dx, row, col, adjacentNode, nodeBeingChecked, true);
                        }
                    }
                    break;
                case 6:
                    addEdge(row, col, row + dy, col + dx, nodeBeingChecked, adjacentNode, false);
                    if (nodeBeingChecked.color == 5 && !(row - dy == 0 || row - dy == rows || col - dx == 0 || col - dx == cols || nodes[row - dy, col - dx].color == 0))
                    {
                        break;
                    }
                    else
                    {
                        addEdge(row + dy, col + dx, row, col, adjacentNode, nodeBeingChecked, false);
                    }
                    break;
                default:
                    break;
            }
        }

        public void addEdge(int rowFrom, int colFrom, int rowTo, int colTo, Node edgeGoesFrom, Node edgeGoesTo, bool goesOverPurple)
        {
            if(edgeGoesFrom.color == 0 || edgeGoesFrom.color == 2 || (edgeGoesFrom.color == 4 && isWaterElectrified(rowFrom, colFrom)) ||
                edgeGoesTo.color == 0 || edgeGoesTo.color == 2 || (edgeGoesTo.color == 4 && isWaterElectrified(rowTo, colTo)))
            {
                return;
            }

            if(edgeGoesTo.color == 1)
            {
                edgeGoesFrom.edges.Add(new Edge(rowFrom, colFrom, rowTo, colTo, edgeGoesTo, "orange"));
            }
            else if(goesOverPurple)
            {
                edgeGoesFrom.edges.Add(new Edge(rowFrom, colFrom, rowTo, colTo, edgeGoesTo, "lemon"));
            }
            else
            {
                edgeGoesFrom.edges.Add(new Edge(rowFrom, colFrom, rowTo, colTo, edgeGoesTo, ""));
            }
        }

        public bool isWaterElectrified(int waterRow, int waterCol)
        {
            if ((waterRow - 1 >= 0 && nodes[waterRow - 1, waterCol].color == 2) || (waterRow + 1 < rows && nodes[waterRow + 1, waterCol].color == 2) || 
                    (cols > waterCol + 1 && nodes[waterRow, waterCol + 1].color == 2) || (waterCol - 1 >= 0 && nodes[waterRow, waterCol - 1].color == 2))
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
