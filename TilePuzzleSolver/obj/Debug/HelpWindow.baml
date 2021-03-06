   M S B A M L   `   `   `  ����K  GTilePuzzleSolver, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:TilePuzzleSolverTilePuzzleSolver  5   	   !    TilePuzzleSolver.HelpWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:TilePuzzleSolver     5   	     � Title$
  Help��5   	   $	��400��6   $	��500��6#    � 
ResizeMode$ NoResize=�6/   .��5	      � $��0,0,0,2q�6   .+�5
   
   �� -   5         ��Name$ 
tabControl��5
      $
��Left=�6)   $	��369��6D   $	��Top=�6Q   $	��494��6i   .@�5      �� $��About��6   .��5      � ���	#FFFFFFFF.+�5      �� $
��Left=�6#   $	��334��6>   $	��Top=�6K   $	��480��6c   .��5      � $	��765��6   $	��470��6,   .+�5      �� -   5         ��Name$ About_Label��5   $   $��About��69   $
��Left=�6I   $	��Top=�6d   $��37��6|   $	��127��6�   $��18.667(�6�   �� 5      -   5        $ Rules_Label��5   $   $��Puzzle Rules��69   $
��Left=�6P   $��	0,103,0,0q�6k   $	��Top=�6~   $��37��6�   $	��127��6�   $��18.667(�6�   �� 5      -   5         ��Name$ About_TextBox��5   &   $
��Left=�6=   $��65��6X   $��	10,38,0,0q�6d    �� TextWrapping$
 Wrap=�6w   $����Tile Puzzle Solver was written by Tyler Hagerty as an exercise in writing path-finding algorithms for complex problems. With it, you are able to make your own puzzles based on a puzzle from the video game "Undertale", and have the program attempt to find the shortest path from the left side to the right, if one exists.��6�   $	��Top=�6�  $	��450��6�  $��0q�6  {�.  6  �� 5      -   5        $ Rules_TextBox��5   &   $
��Left=�6=   $	��597��6X   $��
45,143,0,0q�6e   $
 Wrap=�6y   $	��Top=�6�   $	��410��6�   $��0q�6�   {�.  $����Red tiles are impassable, a player can never step onto a red space.

Orange tiles can be moved onto normally (from above/below/left/right adjacent tiles). However, they also introduce the "scent" mechanic, the source of added complexity to our puzzle, as they make a player "smell like oranges" if the tile is stepped on.

Yellow tiles are "electricity tiles". Stepping onto one sends the player back in the direction they came from. As a result, a player can never end up standing on a yellow tile.

Green tiles can be moved onto normally. However, the source of this puzzle provides incentive to avoid stepping on a green tile, so this program can be configured to avoid green tiles in its solutions. 

Blue tiles are "water tiles" and can be moved onto normally, unless they are either adjacent to a yellow tile (in which case the water is electrified) or the player has an orange scent from previously stepping on an orange tile (in which case, "piranhas in the water will bite the player"). In either of these cases, a blue tile will act like a yellow tile.

Purple tiles are "soap tiles" which are slippery, forcing the player forward another tile in the direction they traveled (unless forced against a wall or the puzzle's edge, in which case they stop on the purple tile) and removing the orange scent, allowing them to again go through blue tiles. The sliding effect can stack, so if the next tile is also purple, the player would slide over that as well.
Note that if sliding into a yellow tile, the player will be forced back, sliding over the purple tile in the opposite direction.

Pink tiles can be moved onto normally and have no special rules.

For the purposes of this program, it is assumed that a player trying to solve the puzzle starts on a platform on the left side of the puzzle, from which they can enter the puzzle onto any accessible tile along the left side. The goal is to cross the puzzle and reach the right side, from any accessible tile along the puzzle's right side.��6�   �� 5      $
��Left=�6(   $��25��6C   $��
10,141,0,0q�6O   
���   �$	��Top=�6r   $��25��6�   ���	#FFF720156�   �� 5      $
��Left=�6(   $��25��6C   $��
10,174,0,0q�6O   
���   �$	��Top=�6r   $��25��6�   ���	#FFF986266�   �� 5      $
��Left=�6(   $��25��6C   $��
10,253,0,0q�6O   
���   �$	��Top=�6r   $��25��6�   ���	#FFFFF6376�   �� 5      $
��Left=�6(   $��25��6C   $��
10,317,0,0q�6O   
���   �$	��Top=�6r   $��25��6�   ���	#FF6EFF376�   �� 5      $
��Left=�6(   $��25��6C   $��
10,382,0,0q�6O   
���   �$	��Top=�6r   $��25��6�   ���	#FF48028F6�   �� 5      $
��Left=�6(   $��25��6C   $��
10,477,0,0q�6O   
���   �$	��Top=�6r   $��25��6�   ���	#FFC303C36�   �� 5      $
��Left=�6(   $��25��6C   $��
10,619,0,0q�6O   
���   �$	��Top=�6r   $��25��6�   ���	#FFFF6CEB6�   5      5      5      5      �� 5      $��Editing��6   .��5      � ���	#FFFFFFFF.+�5       �� -   5        $ 
Edit_Label��5       $��Editing��60   $
��Left=�6B   $	��Top=�6]   $��37��6u   $��72��6�   $��18.667(�6�   �� 5!      -   5        $ Edit_TextBox��5!      $
��Left=�64   $	��282��6O   $��	10,38,0,0q�6\   $
 Wrap=�6o   $����The puzzle solver starts off with a default puzzle, however the size of the puzzle and the colors of the tiles within can be easily changed by clicking the "EDIT" button to open the Edit Bar. The "EDIT" button may again be clicked to remove the Edit Bar.

To resize the puzzle, you can change the number of rows and columns in their respective text boxes, and then click the "RESIZE" button. When making a larger puzzle, any new tiles will default to pink. When making a smaller puzzle, the corresponding tile colors nearest the top left corner that fit into the new size are preserved in the new puzzle. 

To change the color of individual tiles, click on the color you wish to select in the Edit Bar, and then click on any tile in the puzzle to change its color to the selected color.

Within the Edit Bar are also features to toggle avoiding green tiles in the solution (unchecked by default to not avoid green tiles) and to generate a random puzzle with the current size.��6�   $	��Top=�6�  $	��450��6�  $��0q�6�  {�.  6�  5"      5#      �� 5$      $��Saving/Loading��6   .��5%      � ���	#FFFFFFFF.+�5&      �� -   5        $ SaveLoad_Label��5&      $��Saving and Loading��64   $
��Left=�6Q   $	��Top=�6l   $��37��6�   $	��176��6�   $��18.667(�6�   �� 5'      -	   5        $ SaveLoad_TextBox��5'      $
��Left=�68   $	��282��6S   $��	10,38,0,0q�6`   $
 Wrap=�6s   $����If you have created a puzzle that you wish to save for later use, you can simply press the "SAVE" button, which will open up a window that will allow you to save a .txt file that represents the puzzle. You are free to name it and store it in whatever file location you see fit. The .txt file can be shared between computers and users outside of this program, if desired.

To load a previously created puzzle, click the "LOAD" button, which will bring up an open file window. Navigate to the location of a .txt file that was created by this program and select it to change the current puzzle to the one represented by the selected file. The current puzzle will be lost.��6�   $	��Top=�6Q  $	��450��6i  $��0q�6u  {�.  6�  5(      5)      �� 5*      $��Graphing��6   .��5+      � ���	#FFFFFFFF.+�5,      �� -
   5        $ Graph_Label��5,      $��Graphing��61   $
��Left=�6D   $	��Top=�6_   $��37��6w   $��87��6�   $��18.667(�6�   �� 5-      -   5        $ Graph_TextBox��5-      $
��Left=�65   $	��208��6P   $��	10,38,0,0q�6]   $
 Wrap=�6p   $����In order to more easily solve input puzzles, this puzzle solver converts puzzles into graphs representing legitimate moves to consider (for example, a move onto a yellow tile which does not result in a "scent" change will have no affect and is therefore not graphed). Tiles are considered nodes, and edges are created between these nodes wherever a legitimate move can be made between the corresponding tiles.

To show a graphical representation of the determined edges, click the "GRAPH" button. Lines will be drawn between tiles wherever there is an edge, and clicking on a tile after the "GRAPH" button has been clicked will show detailed information on where all its edges lead. Note that all edges are erased if the puzzle is at all altered, and if you wish to view the edges again, you must re-click the "GRAPH" button. It is not necessary to click the "GRAPH" button before solving.��6�   $	��Top=�6I  $	��450��6a  $��0q�6m  {�.  6�  5.      5/      �� 50      $��Solving��6   .��51      � ���	#FFFFFFFF.+�52      �� -   5        $ Solve_Label��52      $��Solving��61   $
��Left=�6C   $	��Top=�6^   $��37��6v   $��70��6�   $��18.667(�6�   �� 53      -   5        $ Solve_TextBox��53      $
��Left=�65   $	��208��6P   $��	10,38,0,0q�6]   $
 Wrap=�6p   $����To have the program attempt to find a solution to the current puzzle, one must simply click the "SOLVE" button. If no path exists from the left side to the right, a window will pop up informing you such. If a path does exist, it will be graphically shown by a black line going through each successive tile. Depending on the size of the puzzle, finding a solution may take a few moments.

Toggling the avoid green tiles option in the Edit Bar (explained in the "Editing" help section) may return a different solution in which more weight is added to stepping onto a green tile, making the program more easily consider alternate solutions that avoid green tiles when possible.��6�   $	��Top=�6T  $	��450��6l  $��0q�6x  {�.  6�  54      55      56      58      59      