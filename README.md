# Tile Puzzle Solver
Tile Puzzle Solver was written as an academic exercise in developing a special-purpose algorithm to solve a relatively complex problem. It enables the user to create a color tile maze-like puzzle based on [a puzzle from the game "Undertale"](http://undertale.wikia.com/wiki/Multicolor_Tile_Puzzle) and then search for a solution using an algorithm based on the A* path-finding algorithm. Detailed information on the algorithm developed can be found [here](https://github.com/t-hagerty/Tile-Puzzle-Solver/wiki/Path-Finding-Algorithm).

What separates the color tile puzzle from a normal maze path-finding problem, and what makes it more interesting to solve algorithmically is that the state of the puzzle's obstacles and movement rules change in the event of stepping over certain types of tiles. 

If you wish to try an example of the puzzle this program was made to solve, you may try a fan-made version [here](https://putnam3145.github.io/tilemaze).

#Features
- A GUI for easily creating custom puzzles of size and configuration.
- The ability to save created puzzles and load them later.
- Potentially relevant possible movements to the solution can be displayed with the "Graph" function.
- Show the solution to the created puzzle (if solvable).
- Determine an alternate path that avoids green tiles when possible.

# Puzzle Rules
The goal of the puzzle is to make it from the left boundary to the right, entering from any tile space on the left and leaving from any tile on the right. The puzzle also has a "scent" mechanic, which alters the behavior of blue tiles depending on the scent's state (a player starts out with no scent). Essentially, the puzzle's state changes upon moving onto certain tiles, which makes this path-finding problem much more complex than a traditional, more simple path-finding situation. Disregarding any special rules for tile colors, 4-directional movement is allowed; up, down, left, or right, but not diagonally. The tile color rules can be read on the wiki [here](https://github.com/t-hagerty/Tile-Puzzle-Solver/wiki/Puzzle-Rules).

#Installation
- The current release is only officially supported on Windows computers, and only fully tested on Windows 10.
- Grab the .zip of the latest version on the [releases page](https://www.microsoft.com/en-us/download/details.aspx?id=42643) and extract its contents to a folder.
- Ensure you have at least [.NET Framework 4.5.2](https://www.microsoft.com/en-us/download/details.aspx?id=42643).
- Run the .exe file from the extracted .zip file.

#License
This software and its code is made freely available under the MIT License. See LICENSE.txt for more information.

#Special Thanks
- BlueRaja's [Priority Queue for C#](https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp)
