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
        /*
            startNode and endNode are for the purposes of this scenario arbitrary nodes that don't have a specific location, they're off the grid/2D array.
            This is because, in this scenario, we esentially have an "invisible platform" that a player could enter the puzzle on to any tile on the left
            (col = 0), and the goal is to cross the puzzle to the right side, where we could think another invisible platform is that can be reached from
            any tile on the right (col = cols - 1).

            If we wanted the player to start on a specific tile wthin the puzzle, we could set startNode = nodes[r,c], where r, c = the row/column (same for
            endNode) and make some minor modifications to the code (specifically, the code in buildGraph() that deals with adding edges for the two arbitrary
            start/endNodes).
        */

        public int rows;
        public int cols;


        public TilePuzzleModel(int r, int c, int[,] tileGridColors)
        {
            rows = r;
            cols = c;
            
            //Pink tiles (6) have no rules concerning moving onto them, so we make start/end "pink" as they're not on the puzzle.
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
                        //For all tiles in the new space of the larger puzzle, we have no information about their color yet, set them as pink.
                        newSizePuzzle[r, c] = new Node(6);
                    }
                }
                for (int r = rows; r < newRows; r++)
                {
                    for (int c = 0; c < newCols; c++)
                    {
                        //For all tiles in the new space of the larger puzzle, we have no information about their color yet, set them as pink.
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

            resetGraphEdges(); 

            buildGraph();
            
            //A*-based pathfinding algorithm

            //return something to represent path, maybe ordered list of tuples? that the mainwindow.xaml.cs can use to draw/highlight the path to the user.
        }

        public void buildGraph()
        {
            //Set edges from the startNode ("invisible platform" on the left that can reach all tiles with col = 0) to each tile on left side
            for(int r = 0; r < rows; r++)
            {
                checkForEdgeInDirection(startNode, r, -1, 1, 0); //-1 as column so it will check node at col = 0
            }

            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    //if((r == 1 || r == 2) && c == 24)
                    //{
                    //    r = r;
                    //}

                    /*
                    Note: findEdgesForNode will normally only check Nodes to the right and below the current node rather than in all 4 directions 
                    (doing so would perform duplicate checks)
                    */
                    findEdgesForNode(nodes[r, c], r, c);
                }
            }

            for (int r = 0; r < rows; r++)
            {
                nodes[r, cols - 1].edges.Add(new Edge(r, cols - 1, r, cols, endNode, "")); 
                /*
                Even if this node shouldnt connect to end, i.e. if node is red, the edge is still added, but in any of these cases, the node is 
                inaccessible otherwise, so it doesn't matter.
                */
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
                    //Red tiles (0) are inaccessible/"walls" so there should be no edges from or to it, do nothing.
                    break;
                case 1:
                    //Orange tiles (1) have normal movement rules except for the orange scent, but that isn't relevant in graph building.
                    checkForEdgeInDirection(nodeBeingChecked, row, col, 1, 0);
                    checkForEdgeInDirection(nodeBeingChecked, row, col, 0, 1);
                    break;
                case 2:
                    /*
                    Yellow tiles (2) are electricity, and when the puzzle is played, a player moving to one will be sent back in the direction
                    they tried entering the tile from, so edges cannot go from or to this tile. As this movement would send the player right back
                    where they started, modeling it in the graph is mostly unnecessary for path-finding.
                    HOWEVER: There is one strategic situation where a player moving to a yellow tile is useful and relevant to our algorithm
                    solving the puzzle. If the player moves onto a purple tile that slides them into a yellow tile, the yellow tile sends them back
                    to the tile they entered the purple tile from. This seems useless, but it will give the player a lemon scent which may be critical
                    in solving the puzzle (Note this doesn't have to be considered if the tile they entered from is orange).

                    So, when we find this situation, we create a "dummy" node with color pink and no real location on the grid. This acts as a space the
                    path-finding algorithm can move toto gain the lemon scent, and then can only move backwards back to the space it came from, all without
                    actually placing it on the yellow tile, potentially giving access to routes which shouldn't be.
                    */

                    //Need to check if a dummy node needs to be made for a leftward direction because we normally only check rightwards 
                    //(which is sufficient except in special cases like this)
                    if (col + 1 < cols && nodes[row, col + 1].color == 5) //If the tile to the right of this is purple
                    {
                        int i = col + 2;

                        //loop through/"slide across" all the purple tiles in a row (if there's more than one) to reach the non-purple tile 
                        //the player could slide onto them from.
                        while (i < cols && nodes[row, i].color == 5)
                        {
                            i++;
                        }
                        if (i == cols) { i--; } //If we've reached puzzle bound, purple tile is against a side, that will be where the player slides from (if at all)
                        checkForEdgeInDirection(nodes[row, i], row, i, 0, -1);
                    }

                    //Need to check if a dummy node needs to be made for an upward direction because we normally only check downwards 
                    //(which is sufficient except in special cases like this)
                    if (row + 1 < rows && nodes[row + 1, col].color == 5) //If the tile below the one being checked is purple
                    {
                        int i = row + 2;
                        //loop through/"slide across" all the purple tiles in a row (if there's more than one) to reach the non-purple tile 
                        //the player could slide onto them from.
                        while (i < rows && nodes[i, col].color == 5)
                        {
                            i++;
                        }
                        if (i == rows) { i--; } //If we've reached puzzle bound, purple tile is against a side, that will be where the player slides from (if at all)
                        checkForEdgeInDirection(nodes[i, col], i, col, -1, 0);
                    }
                    break;
                case 3:
                    //Green tile (3) has normal movement rules as far as graph-building is concerned.
                    checkForEdgeInDirection(nodeBeingChecked, row, col, 1, 0);
                    checkForEdgeInDirection(nodeBeingChecked, row, col, 0, 1);
                    break;
                case 4:
                    /*
                    Blue tile (4) is water. Has normal movement rules as far as graph-building is concerned, unless a yellow tile is adjacent,
                    in which case, the tile is treated like a yellow tile (electrifies the water).
                    */
                    if (isWaterElectrified(row, col)) //If water is electrified, acts exactly like yellow tile, refer to case 2 for explanation.
                    {
                        if (col + 1 < cols && nodes[row, col + 1].color == 5)
                        {
                            int i = col + 2;
                            while (i < cols && nodes[row, i].color == 5)
                            {
                                i++;
                            }
                            if (i == cols) { i--; }
                            checkForEdgeInDirection(nodes[row, i], row, i, 0, -1);
                        }
                        if (row + 1 < rows && nodes[row + 1, col].color == 5)
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
                    else //Not electrified, normal movement rules
                    {
                        checkForEdgeInDirection(nodeBeingChecked, row, col, 1, 0);
                        checkForEdgeInDirection(nodeBeingChecked, row, col, 0, 1);
                    }
                    break;
                case 5:
                    /*
                    Purple tiles (5) "slide" a player walking onto one onto the next tile in the direction they entered it from (unless that
                    tile is a red tile, or the purple tile is against the puzzle's bounds (won't slide off the puzzle). The effect stacks
                    if multiple purple tiles in a row.
                    As a result, the player cannot stand on a purple tile except in special circumstances (the situations that stop the sliding
                    stated above). So, we only check for edges to/from this node if one of these situations is present.
                    */
                    /*
                    *Even if there's a electrified tile next to a purple tile, doesn't matter (don't have to worry about dummy nodes) because "player" already has lemon scent 
                    *from going to this purple, and any tile the dummy node would send them to is already accessible from the purple tile.
                    */

                    if ((((row + 1 < rows && nodes[row + 1, col].color == 0) || row + 1 == rows) && 
                        (row - 1 >= 0 && !(nodes[row - 1, col].color == 0 || nodes[row - 1, col].color == 2 || (nodes[row - 1, col].color == 4 && isWaterElectrified(row - 1, col)))))
                        || (((row - 1 >= 0 && nodes[row - 1, col].color == 0) || row - 1 == -1) && 
                        (row + 1 < rows && !(nodes[row + 1, col].color == 0 || nodes[row + 1, col].color == 2 || (nodes[row + 1, col].color == 4 && isWaterElectrified(row + 1, col)))))
                        || ((col + 1 < cols && nodes[row, col + 1].color == 0) && 
                        (col - 1 >= 0 && !(nodes[row, col - 1].color == 0 || nodes[row, col - 1].color == 2 || (nodes[row, col - 1].color == 4 && isWaterElectrified(row, col - 1)))))
                        || ((col - 1 >= 0 && nodes[row, col - 1].color == 0) && 
                        (col + 1 < cols && !(nodes[row, col + 1].color == 0 || nodes[row, col + 1].color == 2 || (nodes[row, col + 1].color == 4 && isWaterElectrified(row, col + 1))))))
                    /*
                    IF node below is red or nodeBeingChecked is against lower bound, and node above is not red or yellow or electrified water
                    OR node above is red or nodeBeingChecked is against upper bound, and node below is not red or yellow or electrified water
                    OR node to left is red and node to right is not red or yellow or electrified water
                    OR node to right is red and node to left is not red or yellow or electrified water
                    */
                    {
                        checkForEdgeInDirection(nodeBeingChecked, row, col, 1, 0);
                        checkForEdgeInDirection(nodeBeingChecked, row, col, 0, 1);
                        checkForEdgeInDirection(nodeBeingChecked, row, col, -1, 0);
                        checkForEdgeInDirection(nodeBeingChecked, row, col, 0, -1);
                    }
                    break;
                case 6:
                    //Pink tiles (6) have normal movement rules.
                    checkForEdgeInDirection(nodeBeingChecked, row, col, 1, 0);
                    checkForEdgeInDirection(nodeBeingChecked, row, col, 0, 1);
                    break;
                default:
                    break;
            }
        }

        public void checkForEdgeInDirection(Node nodeBeingChecked, int row, int col, int dx, int dy)
        {
            //If direction takes us out of bounds, stop here.
            if(row + dy < 0 || row + dy >= rows || col + dx < 0 || col + dx >= cols) { return; }

            Node adjacentNode = nodes[row + dy, col + dx];

            switch(adjacentNode.color)
            {
                case 0:
                    //Can't move onto red tile, no edges to make.
                    break;
                case 1:
                    //
                    addEdge(row, col, row + dy, col + dx, nodeBeingChecked, adjacentNode, false);
                    if (nodeBeingChecked.color == 5 && !(row == 0 || row == rows - 1 || col == 0 || col == cols - 1 || nodes[row - dy, col - dx].color == 0))
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
                    if (nodeBeingChecked.color == 5 && !(row == 0 || row == rows - 1 || col == 0 || col == cols - 1 || nodes[row - dy, col - dx].color == 0))
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
                        if (nodeBeingChecked.color == 5 && !(row == 0 || row == rows - 1 || col == 0 || col == cols - 1 || nodes[row - dy, col - dx].color == 0))
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
                    if (nodeBeingChecked.color == 5 && !(row == 0 || row == rows - 1 || col == 0 || col == cols - 1 || nodes[row - dy, col - dx].color == 0))
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
