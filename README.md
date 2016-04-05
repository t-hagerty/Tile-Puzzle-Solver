- This readme is under construction

# Tile Puzzle Solver
Tile Puzzle Solver was written as an academic exercise in developing a special-purpose algorithm to solve a relatively complex problem. It enables the user to create a color tile maze-like puzzle based on a puzzle from the game "Undertale" (http://undertale.wikia.com/wiki/Multicolor_Tile_Puzzle) and then search for a solution using an algorithm based on the A* path-finding algorithm.

#Features
- to be detailed

# Puzzle Rules
DISCLAIMER: As I have not myself played the video game from which this puzzle originates, I cannot guarantee that Tile Puzzle Solver's puzzle rules and behaviors 100% match the source puzzle, however I am fairly certain these rules are accurate after studying information on the game's puzzle as well as fan-made versions of the puzzle (https://putnam3145.github.io/tilemaze for example).

The goal of the puzzle is to make it from the left boundary to the right, entering from any tile space on the left and leaving from any tile on the right. The puzzle also has a "scent" mechanic, which alters the behavior of blue tiles depending on the scent's state (a player starts out with no scent). Essentially, the puzzle's state changes upon moving onto certain tiles, which makes this path-finding problem much more complex than a traditional, more simple path-finding situation. Disregarding any special rules for tile colors, 4-directional movement is allowed; up, down, left, or right, but not diagonally. The tile color rules are as follows:

- RED: An impassable tile, cannot be moved onto.

- ORANGE: Can be moved onto normally, gives a player an "orange scent" when moved onto.

- YELLOW: "Electric tiles". If a player tries to step onto a yellow tile, it moves them back in the direction from which they came. As a result, yellow tiles cannot be moved onto, and are not useful to consider in finding a path, unless the player aquires a "lemon scent" by sliding over a purple tile into a yellow tile (purple tile rules will be explained below).

- GREEN: Can be moved onto normally. However, the source puzzle provides incentive to avoid passing over green tiles, so Tile Puzzle Solver has an "Avoid Green Tiles" mode which attempts to find a path which avoids going over green tiles when possible.

- BLUE: "Water tiles". Acts like yellow tiles if a yellow tile is adjacent. Can be moved onto normally, unless the player has an "orange scent" in which case the tile also acts like a yellow tile (game explanation: piranhas in the water are attracted the to orange scent).

- PURPLE: "Soap tiles". Slides the player one space forward in the direction they traveled onto the purple tile, if possible (stops on the purple tile if there's a red tile or a puzzle boundary). If the next tile is also purple, the player will keep sliding. Washes away a player's "orange scent" and gives them a "lemon scent" which allows them to once again travel through blue tiles normally.

- PINK: Can be moved onto normally, pink tiles have no special rules.

#Path-finding Algorithm
- to be explained

#Installation
- to be explained

#License
- to be detailed
