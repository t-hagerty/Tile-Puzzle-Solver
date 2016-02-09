using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilePuzzleSolver
{
    /// <summary>
    /// Models/handles the manipulating of a specific instance of the color tile puzzle.
    /// 
    /// RULES:
    /// Can move up, down, left, or right, not diagonally. Objective is to start at the left side entering on any leftmost tile you want
    /// and crossing the puzzle to the right side, exiting on any rightmost tile.
    /// 
    /// RED TILES (Represented as number 0): Impassable tiles, cannot walk onto them, can be thought of as walls
    /// ORANGE TILES (Represented as number 1): Passable, normal movement rules, but gives the player an "orange scent" if walked on
    /// YELLOW TILES (Represented as number 2): Not passable. In play, walking onto one will "shock" the player (electric tile) back to the tile they came from.
    /// GREEN TILES (Represented as number 3): Passable, normal movement rules. In gameplay, would "summon a monster to fight" but is irrelevant here, besides
    /// the interesting option to find a solution which avoids green tiles as much as possible.
    /// BLUE TILES (Represented as number 4): Passable "water" tile, unless next to a yellow/electric tile, which makes the blue tile act as a yellow tile, or if
    /// the player is orange scented.
    /// PURPLE TILES (Represented as number 5): Slides the player one tile forward in the direction they entered the purple tile from if possible, gives lemon scent
    /// "Soap" tile
    /// PINK TILES (Represented as number 6): Passable, normal movement rules, no extra rules/behavior.
    /// 
    /// SCENTS:
    /// Player starts off with no scent.
    /// ORANGE SCENT: Given by walking onto an orange tile. When orange scented, cannot enter water tiles, acts similarly to yellow/electric tiles
    /// "Piranhas in the water" attracted to orange scent.
    /// LEMON SCENT: Given by walking onto/sliding across purple/"soap" tiles. Removes orange scent. Repels piranhas in water and allows normal
    /// movement through non-electrified water tiles again.
    /// </summary>
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

        /// <summary>
        /// Constructor for a color tile puzzle with input for both number of rows and colors, and specifying the colors of each tile from
        /// the beginning using an input 2D Array of ints.
        /// </summary>
        /// <param name="r">The number of rows in the puzzle/grid</param>
        /// <param name="c">The number of columns in the puzzle/grid</param>
        /// <param name="tileGridColors">A 2D array of integers from 0 to 6 representing each tile's color (0 - red, 1 - orange, 2 - yellow, 3 - green, 4 - blue, 5 - purple, 6 - pink)</param>
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

        /// <summary>
        /// Resizes the puzzle based on new input number of rows and columns. Copies tile colors to their same positions if possible, fills in expanded space with pink tiles.
        /// </summary>
        /// <param name="newRows">The new numbe of rows of the puzzle/grid</param>
        /// <param name="newCols">The new number of columns of the puzzle/grid</param>
        public void resizePuzzle(int newRows, int newCols)
        {
            Node[,] newSizePuzzle = new Node[newRows, newCols];

            if(newRows < rows)
            {
                rows = newRows;
            }
            if(newCols < cols)
            {
                cols = newCols;
            }

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

            nodes = newSizePuzzle;
            rows = newRows;
            cols = newCols;
        }

        /// <summary>
        /// 
        /// </summary>
        public void solve()
        {
            bool isPlayerOrangeScented = false;

            resetGraphEdges(); 

            buildGraph();
            
            //A*-based pathfinding algorithm

            //return something to represent path, maybe ordered list of tuples? that the mainwindow.xaml.cs can use to draw/highlight the path to the user.
        }

        /// <summary>
        /// Translates the 2D array of color tiles/nodes which initially have no information about what nodes they connect to into a graph of nodes and edges
        /// based on the rules of the puzzle. For use in more easily implementing the path-finding algorithm.
        /// 
        /// Also connects the start node to all the leftmost nodes and the end node to all the rightmost nodes.
        /// </summary>
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

        /// <summary>
        /// Clears all edges of each node in the puzzle so that none have any relations to each other any more. Should be done whenever we know
        /// the puzzle has been editted.
        /// </summary>
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

        /// <summary>
        /// Looks at a specific node along with its position in the puzzle and takes action for finding the edges of the node based on its color
        /// (e.g. if the node being checked is red, nothing will be done because it can't have edges). Will normally check for downwards and
        /// rightwards edges only so we can do half the work (most of the time, an edge going from nodeBeingChecked to another node will 
        /// correspond to an edge going from that other node to the node being checked (except in special situations that are checked for).
        /// </summary>
        /// <param name="nodeBeingChecked">The current node being looked at to find edges</param>
        /// <param name="row">The row of the node being checked</param>
        /// <param name="col">The column of the node being checked</param>
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
                        checkForEdgeInDirection(nodes[row, i], row, i, -1, 0);
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
                        checkForEdgeInDirection(nodes[i, col], i, col, 0, -1);
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
                            checkForEdgeInDirection(nodes[row, i], row, i, -1, 0);
                        }
                        if (row + 1 < rows && nodes[row + 1, col].color == 5)
                        {
                            int i = row + 2;
                            while (i < rows && nodes[i, col].color == 5)
                            {
                                i++;
                            }
                            if (i == rows) { i--; }
                            checkForEdgeInDirection(nodes[i, col], i, col, 0, -1);
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

        /// <summary>
        /// Checks to see if there's an edge between the node being checked and the node in the direction specified by dx and dy.
        /// Note that dx and dy should only be -1, 0, or 1.
        /// </summary>
        /// <param name="nodeBeingChecked">The node being checked for a possible edge in the specified direction</param>
        /// <param name="row">The row of nodeBeingChecked</param>
        /// <param name="col">The column of nodeBeingChecked</param>
        /// <param name="dx">The change in left/right position (change in column). Should only be -1, 0, or 1</param>
        /// <param name="dy">The change in up/down position (change in row). Should only be -1, 0, or 1</param>
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
                    //Orange tiles have normal movement rules. addEdge method will add appropriate scent info to the edge.
                    addEdge(row, col, row + dy, col + dx, nodeBeingChecked, adjacentNode, false); //Add edge TO this orange tile.
                    if (nodeBeingChecked.color == 5 && !(row == 0 || row == rows - 1 || col == 0 || col == cols - 1 || nodes[row - dy, col - dx].color == 0))
                    //If nodeBeingChecked is purple, check if the node on the side of it opposite of this orange tile is red, or the puzzle's bounds, otherwise
                    //no edge should be made to go back to nodeBeingChecked.
                    {
                        break;
                    }
                    else
                    {
                        //nodeBeingChecked has no sliding involved, so we can make an edge going back from adjacentNode to it.
                        addEdge(row + dy, col + dx, row, col, adjacentNode, nodeBeingChecked, false);
                    }
                    break;
                case 2:
                    //Yellow/electric tile, no edge can go to this tile, do nothing.
                    break;
                case 3:
                    //Green tile, normal movement rules
                    addEdge(row, col, row + dy, col + dx, nodeBeingChecked, adjacentNode, false);
                    if (nodeBeingChecked.color == 5 && !(row == 0 || row == rows - 1 || col == 0 || col == cols - 1 || nodes[row - dy, col - dx].color == 0))
                    //Same explanation as case 1, if nodeBeingChecked is purple, must check if we can actually go to it from adjacentNode
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
                        //Same explanation as case 1, if nodeBeingChecked is purple, must check if we can actually go to it from adjacentNode
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
                    int slideDX = dx + dx; //col + slideDX will always be the column of the tile after the purple tile currently being looked at.
                    int slideDY = dy + dy; //row + slideDY will always be the row of the tile after the purple tile currently being looked at.

                    //We want to find the end of the purple tile "slide" (multiple purple tiles next to each other in a line) the first tile after the 
                    //purple tile(s) that isn't purple (where the player would stop) or the tile's bounds.
                    while (row + slideDY >= 0 && row + slideDY < rows && col + slideDX >= 0 && col + slideDX < cols && adjacentNode.color == 5)
                    //While not out of bounds and next tile is also purple
                    {
                        adjacentNode = nodes[row + slideDY, col + slideDX];
                        slideDX += dx;
                        slideDY += dy;
                    }

                    if(adjacentNode.color == 0)
                    {
                        //backtrack one, the purple tile before the red tile (wall/impassable) becomes where the edge stops (slide into tile against the wall)
                        nodes[row + slideDY - dy, col + slideDX - dx] = adjacentNode;
                    }

                    if(col + slideDX == cols && adjacentNode.color == 5)
                    {
                        //If while loop stopped against the right puzzle bound && the last tile before the bound was purple, it should slide into the endNode side.
                        addEdge(row, col, row, cols, nodeBeingChecked, endNode, true);
                        break;
                    }

                    if(nodeBeingChecked.color != 1 && (adjacentNode.color == 2 || (adjacentNode.color == 4 && isWaterElectrified(row + slideDY - dy, col + slideDX - dx))))
                    {
                        //If nodeBeingChecked isn't orange and adjacentNode is yellow or electrified blue
                        //(If the purple slide ends in electricity, we make a dummy node (see explanation in case 2 of findEdgesForNode)
                        //unless the starting tile was orange, because then the move is pointless (dummy node move only useful to get
                        //rid of orange scent))
                        Node dummyNode = new Node(6);
                        addEdge(row, col, -2, -2, nodeBeingChecked, dummyNode, true);
                        if(nodeBeingChecked.color == 5)
                            //If the node we started from in sliding into a yellow tile was purple, we might not be sent back to that node, check to see where we slide back to.
                        {
                            slideDX = dx * -1;
                            slideDY = dy * -1;

                            while(row + slideDY >= 0 && row + slideDY < rows && col + slideDX >= 0 && col + slideDX < cols && nodes[row + slideDY, col + slideDX].color == 5)
                            {
                                slideDX -= dx;
                                slideDY -= dy;
                            }

                            if (nodes[row + slideDY, col + slideDX].color == 1 || nodes[row + slideDY, col + slideDX].color == 2 || (nodes[row + slideDY, col + slideDX].color == 4 && isWaterElectrified(row + slideDY, col + slideDX)))
                            {
                                //No use in finishing the dummy node if it would slide back to another elctrified tile or an orange one.
                                //If this happens, we're left with an edge from nodeBeingChecked to a dummy node that has no edges. If the path-finding
                                ///algorithm even checks this dummy node, it will be seen as a dead end, so it's not much of a problem.
                                break;
                            }
                            if (nodes[row + slideDY, col + slideDX].color == 0)
                            {
                                //If sliding into a wall, backtrack to tile before the wall.
                                slideDX += dx;
                                slideDY += dy;
                            }

                            addEdge(-2, -2, row + slideDY, col + slideDX, dummyNode, nodes[row + slideDY, col + slideDX], true);
                        }
                        else
                        {
                            addEdge(-2, -2, row, col, dummyNode, nodeBeingChecked, true);
                        }
                    }
                    else
                    {
                        addEdge(row, col, row + slideDY - dy, col + slideDX - dx, nodeBeingChecked, adjacentNode, true); //from nodeBeingChecked to adjacentNode at the end of purple slide
                        //Need to check if nodeBeingChecked is purple or not (some situations where the player could end up on purple)
                        if (nodeBeingChecked.color != 5)
                        {
                            addEdge(row + slideDY - dy, col + slideDX - dx, row, col, adjacentNode, nodeBeingChecked, true);
                        }
                        else if((row - dy < 0 || row - dy >= rows || col - dx < 0 || col - dx >= cols) || nodes[row - dy, col - dx].color == 0)
                            //If node next to nodeBeingChecked in opposite direction is red or is a tile bound
                            //(because nodeBeingChecked is purple, need to see if we can go back to it from adjacentNode.
                        {
                            addEdge(row + slideDY - dy, col + slideDX - dx, row, col, adjacentNode, nodeBeingChecked, true);
                        }
                    }
                    break;
                case 6:
                    //Pink tile, normal movement rules
                    addEdge(row, col, row + dy, col + dx, nodeBeingChecked, adjacentNode, false);
                    if (nodeBeingChecked.color == 5)
                    //Same explanation as case 1, if nodeBeingChecked is purple, must check if we can actually go to it from adjacentNode
                    {
                        if(!(row == 0 || row == rows - 1 || col == 0 || col == cols - 1 || nodes[row - dy, col - dx].color == 0))
                        {
                            break;
                        }
                        else if((row == 0 || row == rows - 1) && dy == 0)
                        {
                            break;
                        }
                        else if((col == 0 || col == cols - 1) && dx == 0)
                        {
                            break;
                        }
                        else
                        {
                            addEdge(row + dy, col + dx, row, col, adjacentNode, nodeBeingChecked, false);
                        }
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

        /// <summary>
        /// Adds a directional edge in between two nodes. Handles applying scents to an edge when appropriate.
        /// </summary>
        /// <param name="rowFrom">The row of the node that the edge starts from</param>
        /// <param name="colFrom">The olumn of the node that the edge starts from</param>
        /// <param name="rowTo">The row of the node that the edge goes to</param>
        /// <param name="colTo">The column of the node that the edge goes to</param>
        /// <param name="edgeGoesFrom">The node that the edge starts from</param>
        /// <param name="edgeGoesTo">The node that the edge goes to</param>
        /// <param name="goesOverPurple">True if in the checking for edges process, we've gone over a purple tile. False otherwise.</param>
        public void addEdge(int rowFrom, int colFrom, int rowTo, int colTo, Node edgeGoesFrom, Node edgeGoesTo, bool goesOverPurple)
        {
            //Extra catch in edge-making in case of bugs: make sure neither node involved is an inaccessible tile
            if(edgeGoesFrom.color == 0 || edgeGoesFrom.color == 2 || (edgeGoesFrom.color == 4 && isWaterElectrified(rowFrom, colFrom)) ||
                edgeGoesTo.color == 0 || edgeGoesTo.color == 2 || (edgeGoesTo.color == 4 && isWaterElectrified(rowTo, colTo)))
            {
                return;
            }

            if(edgeGoesTo.color == 1)
            {
                //Edge goes to an orange tile, so it will give the orange scent
                edgeGoesFrom.edges.Add(new Edge(rowFrom, colFrom, rowTo, colTo, edgeGoesTo, "orange"));
            }
            else if(goesOverPurple)
            {
                //Doesn't go to an orange tile and goes over purple, gives lemon scent
                edgeGoesFrom.edges.Add(new Edge(rowFrom, colFrom, rowTo, colTo, edgeGoesTo, "lemon"));
            }
            else
            {
                //Normal unscented edge.
                edgeGoesFrom.edges.Add(new Edge(rowFrom, colFrom, rowTo, colTo, edgeGoesTo, ""));
            }
        }

        /// <summary>
        /// Checks if a blue/water tile is adjacent to a yellow/electric tile (directly above, below, left or right, not diagonal) and thus will act like a yellow tile.
        /// </summary>
        /// <param name="waterRow">Row of the water tile being checked</param>
        /// <param name="waterCol">Column of the water tile being checked</param>
        /// <returns>True if the blue tile is next to a yellow tile, false otherwise</returns>
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
