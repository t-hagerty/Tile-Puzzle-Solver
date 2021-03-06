﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace TilePuzzleSolver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// 
    /// Tile Puzzle ViewModel (MainWindow.xaml is the View, TilePuzzleModel is the Model)
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isGraphed = false;
        private bool isEditMode = false;
        private int selectedColor = 6;
        private int puzzleColumns;
        private int puzzleRows;
        TilePuzzleModel tilePuzzle;

        /// <summary>
        /// Constructor for the View/ViewModel of the tile puzzle editor/solver
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            int[,] startPuzzle = new int[4,39] { {4, 3, 2, 6, 3, 5, 6, 2, 3, 0, 2, 6, 0, 3, 6, 1, 2, 0, 0, 1, 0, 2, 6, 2, 2, 3, 3, 4, 2, 4, 1, 3, 3, 3, 0, 3, 3, 3, 0},
                                                 {3, 1, 5, 4, 1, 0, 0, 4, 2, 0, 0, 4, 4, 4, 0, 0, 0, 6, 6, 0, 4, 6, 3, 0, 5, 1, 4, 1, 5, 5, 6, 4, 0, 3, 0, 3, 0, 3, 0},
                                                 {6, 0, 6, 6, 4, 4, 0, 1, 0, 6, 4, 5, 0, 6, 2, 3, 5, 1, 5, 2, 5, 5, 0, 0, 6, 1, 4, 5, 5, 4, 2, 6, 0, 3, 0, 3, 0, 3, 0},
                                                 {3, 2, 4, 2, 0, 6, 3, 1, 5, 5, 1, 6, 0, 1, 6, 1, 0, 4, 4, 6, 1, 4, 3, 1, 6, 4, 5, 1, 2, 4, 6, 6, 3, 3, 3, 3, 0, 3, 3}};
            tilePuzzle = new TilePuzzleModel(4, 39, startPuzzle);

            resizeTileGrid(4, 39);
        }

        /// <summary>
        /// Resizes the puzzle based on input rows and columns, and rebuilds/replaces the visuals/buttons representing the puzzle in the View.
        /// </summary>
        /// <param name="newRows">The new amount of rows of the puzzle</param>
        /// <param name="newCols">The new amount of columns o the puzzle</param>
        public void resizeTileGrid(int newRows, int newCols)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            removeGraph();

            if (newCols == puzzleColumns)
            {
                if (newRows == puzzleRows)
                {
                    return; //No need to resize
                }
                else if(newRows > puzzleRows) //If only new rows are being added, we can resize without clearing TilePuzzle_UniformGrid, saving time.
                {
                    tilePuzzle.resizePuzzle(newRows, puzzleColumns);
                    if (tilePuzzle.Rows != newRows)
                    {
                        //If the new size is too large, resizePuzzle reverts tilePuzzle.Rows/Cols to the old size, so they won't match up to newRows, newCols
                        //Handle informing the user the puzzle size is too large for memory, return everything to old puzzle.
                        MessageBox.Show("Error: Out of memory! Try a smaller puzzle");
                        Mouse.OverrideCursor = null;
                        Row_TextBox.Text = puzzleRows.ToString();
                        return;
                    }
                    TilePuzzle_UniformGrid.Rows = newRows;
                    TilePuzzle_UniformGrid.Height = TilePuzzle_UniformGrid.Rows * 25.0;
                    TilePuzzleContainer_Grid.Height = TilePuzzle_UniformGrid.Height;

                    for(int r = puzzleRows; r < newRows; r++)
                    {
                        Button startSideButton = new Button();
                        startSideButton.Background = new SolidColorBrush(Colors.DarkGray);
                        startSideButton.IsEnabled = true;
                        startSideButton.BorderThickness = new Thickness(0);
                        TilePuzzle_UniformGrid.Children.Add(startSideButton);

                        for (int c = 1; c <= puzzleColumns; c++)
                        {
                            Button aButton = new Button();
                            aButton.Click += tile_Click;
                            aButton.BorderThickness = new Thickness(0);
                            aButton.Background = getTileColor(tilePuzzle.nodes[r, c - 1].color);
                            TilePuzzle_UniformGrid.Children.Add(aButton);
                        }
                        Button endSideButton = new Button();
                        endSideButton.Background = new SolidColorBrush(Colors.DarkGray);
                        endSideButton.IsEnabled = true;
                        endSideButton.BorderThickness = new Thickness(0);
                        TilePuzzle_UniformGrid.Children.Add(endSideButton);
                    }
                    puzzleRows = newRows;
                }
                else //If only removing rows, we can cut them off the end of the collection of TilePuzzle_UniformGrid's children without having to rebuild.
                {
                    tilePuzzle.resizePuzzle(newRows, puzzleColumns);
                    if (tilePuzzle.Rows != newRows)
                    {
                        //If the new size is too large, resizePuzzle reverts tilePuzzle.Rows/Cols to the old size, so they won't match up to newRows, newCols
                        //Handle informing the user the puzzle size is too large for memory, return everything to old puzzle.
                        MessageBox.Show("Error: Out of memory! Try a smaller puzzle");
                        //tilePuzzle.resizePuzzle(puzzleRows, puzzleColumns);
                        Mouse.OverrideCursor = null;
                        Row_TextBox.Text = puzzleRows.ToString();
                        return;
                    }
                    TilePuzzle_UniformGrid.Rows = newRows;
                    TilePuzzle_UniformGrid.Height = TilePuzzle_UniformGrid.Rows * 25.0;
                    TilePuzzleContainer_Grid.Height = TilePuzzle_UniformGrid.Height;
                    TilePuzzle_UniformGrid.Children.RemoveRange(newRows * (puzzleColumns + 2), TilePuzzle_UniformGrid.Children.Count - (newRows * (puzzleColumns + 2)));
                    puzzleRows = newRows;
                }
            }
            else
            {
                tilePuzzle.resizePuzzle(newRows, newCols);
                if (tilePuzzle.Rows != newRows || tilePuzzle.Cols != newCols)
                {
                    //If the new size is too large, resizePuzzle reverts tilePuzzle.Rows/Cols to the old size, so they won't match up to newRows, newCols
                    //Handle informing the user the puzzle size is too large for memory, return everything to old puzzle.
                    MessageBox.Show("Error: Out of memory! Try a smaller puzzle");
                    //tilePuzzle.resizePuzzle(puzzleRows, puzzleColumns);
                    Mouse.OverrideCursor = null;
                    Row_TextBox.Text = puzzleRows.ToString();
                    Column_TextBox.Text = puzzleColumns.ToString();
                    return;
                }
                TilePuzzle_UniformGrid.Children.Clear();
                puzzleColumns = newCols;
                puzzleRows = newRows;
                TilePuzzle_UniformGrid.Columns = puzzleColumns + 2;
                TilePuzzle_UniformGrid.Rows = puzzleRows;
                TilePuzzle_UniformGrid.Width = (TilePuzzle_UniformGrid.Columns) * 25.0;
                TilePuzzle_UniformGrid.Height = TilePuzzle_UniformGrid.Rows * 25.0;
                TilePuzzleContainer_Grid.Width = TilePuzzle_UniformGrid.Width;
                TilePuzzleContainer_Grid.Height = TilePuzzle_UniformGrid.Height;

                for (int r = 0; r < puzzleRows; r++)
                {
                    Button startSideButton = new Button();
                    startSideButton.Background = new SolidColorBrush(Colors.DarkGray);
                    startSideButton.IsEnabled = true;
                    startSideButton.BorderThickness = new Thickness(0);
                    TilePuzzle_UniformGrid.Children.Add(startSideButton);

                    for (int c = 1; c <= puzzleColumns; c++)
                    {
                        Button aButton = new Button();
                        aButton.Click += tile_Click;
                        aButton.BorderThickness = new Thickness(0);
                        aButton.Background = getTileColor(tilePuzzle.nodes[r, c - 1].color);
                        TilePuzzle_UniformGrid.Children.Add(aButton);
                        if (tilePuzzle.nodes[r, c - 1].color == 3 && AvoidGreen_CheckBox.IsChecked == true)
                        {
                            tilePuzzle.nodes[r, c - 1].weight = newCols;
                        }
                    }

                    Button endSideButton = new Button();
                    endSideButton.Background = new SolidColorBrush(Colors.DarkGray);
                    endSideButton.IsEnabled = true;
                    endSideButton.BorderThickness = new Thickness(0);
                    TilePuzzle_UniformGrid.Children.Add(endSideButton);
                }
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tileType">The integer that represents the color/type of the tile whose SolidColorBrush we're returning</param>
        /// <returns>The SolidColorBrush color that matches the type of the tile whose background color we wish to find</returns>
        private SolidColorBrush getTileColor(int tileType)
        {
            SolidColorBrush tileColor = new SolidColorBrush();

            switch (tileType)
            {
                case 0:
                    tileColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF72015")); //red
                    break;
                case 1:
                    tileColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF98626")); //orange
                    break;
                case 2:
                    tileColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFF637")); //yellow
                    break;
                case 3:
                    tileColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF6EFF37")); //green
                    break;
                case 4:
                    tileColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF48028F")); //blue
                    break;
                case 5:
                    tileColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC303C3")); //purple
                    break;
                case 6:
                    tileColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFF6CEB")); //pink
                    break;
                default:
                    tileColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFF6CEB")); //pink
                    break;
            }

            return tileColor;
        }

        /// <summary>
        /// Changes the color of a tile to the selected color if in edit mode.
        /// 
        /// If not in edit mode, pops up info about the edges a node has (for testing purposes)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tile_Click(object sender, RoutedEventArgs e)
        {
            int index = TilePuzzle_UniformGrid.Children.IndexOf(sender as Button);
            int row = index / (puzzleColumns + 2);
            int col = (index % (puzzleColumns + 2)) - 1;

            if (isEditMode)
            {
                tilePuzzle.nodes[row, col].color = selectedColor;
                if (selectedColor == 3 && AvoidGreen_CheckBox.IsChecked == true)
                {
                    tilePuzzle.nodes[row, col].weight = puzzleColumns;
                }
                else
                {
                    tilePuzzle.nodes[row, col].weight = 0;
                }
                (sender as Button).Background = getTileColor(tilePuzzle.nodes[row, col].color);
            }
            else
            {
                if (isGraphed)
                {
                    MessageBox.Show(tilePuzzle.nodes[row, col].edgesToString());
                }
            }
        }

        /// <summary>
        /// Toggles whether or not the user is in edit mode, that is, if they click on a tile in the puzzle, if it will change to their selected color.
        /// Also enables color-switching buttons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProgramGrid.RowDefinitions[1].Height != new GridLength(0))
            {
                ProgramGrid.RowDefinitions[1].Height = new GridLength(0);
            }
            else
            {
                ProgramGrid.RowDefinitions[1].Height = new GridLength(100);
            }
            Red_Button.IsEnabled = !Red_Button.IsEnabled;
            Orange_Button.IsEnabled = !Orange_Button.IsEnabled;
            Yellow_Button.IsEnabled = !Yellow_Button.IsEnabled;
            Green_Button.IsEnabled = !Green_Button.IsEnabled;
            Blue_Button.IsEnabled = !Blue_Button.IsEnabled;
            Purple_Button.IsEnabled = !Purple_Button.IsEnabled;
            Pink_Button.IsEnabled = !Pink_Button.IsEnabled;
            Randomize_Button.IsEnabled = !Randomize_Button.IsEnabled;
            isEditMode = !isEditMode;

            removeGraph();
        }

        private void redButton_Click(object sender, RoutedEventArgs e)
        {
            selectedColor = 0;
            unselectColorButtons();
            Red_Button.BorderThickness = new Thickness(4);
            Red_Button.BorderBrush = new SolidColorBrush(Colors.White);
        }

        private void orangeButton_Click(object sender, RoutedEventArgs e)
        {
            selectedColor = 1;
            unselectColorButtons();
            Orange_Button.BorderThickness = new Thickness(4);
            Orange_Button.BorderBrush = new SolidColorBrush(Colors.White);
        }

        private void yellowButton_Click(object sender, RoutedEventArgs e)
        {
            selectedColor = 2;
            unselectColorButtons();
            Yellow_Button.BorderThickness = new Thickness(4);
            Yellow_Button.BorderBrush = new SolidColorBrush(Colors.White);
        }

        private void greenButton_Click(object sender, RoutedEventArgs e)
        {
            selectedColor = 3;
            unselectColorButtons();
            Green_Button.BorderThickness = new Thickness(4);
            Green_Button.BorderBrush = new SolidColorBrush(Colors.White);
        }

        private void blueButton_Click(object sender, RoutedEventArgs e)
        {
            selectedColor = 4;
            unselectColorButtons();
            Blue_Button.BorderThickness = new Thickness(4);
            Blue_Button.BorderBrush = new SolidColorBrush(Colors.White);
        }

        private void purpleButton_Click(object sender, RoutedEventArgs e)
        {
            selectedColor = 5;
            unselectColorButtons();
            Purple_Button.BorderThickness = new Thickness(4);
            Purple_Button.BorderBrush = new SolidColorBrush(Colors.White);
        }

        private void pinkButton_Click(object sender, RoutedEventArgs e)
        {
            selectedColor = 6;
            unselectColorButtons();
            Pink_Button.BorderThickness = new Thickness(4);
            Pink_Button.BorderBrush = new SolidColorBrush(Colors.White);
        }

        private void unselectColorButtons()
        {
            Red_Button.BorderThickness = new Thickness(1);
            Red_Button.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF707070"));
            Orange_Button.BorderThickness = new Thickness(1);
            Orange_Button.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF707070"));
            Yellow_Button.BorderThickness = new Thickness(1);
            Yellow_Button.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF707070"));
            Green_Button.BorderThickness = new Thickness(1);
            Green_Button.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF707070"));
            Blue_Button.BorderThickness = new Thickness(1);
            Blue_Button.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF707070"));
            Purple_Button.BorderThickness = new Thickness(1);
            Purple_Button.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF707070"));
            Pink_Button.BorderThickness = new Thickness(1);
            Pink_Button.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF707070"));
        }

        /// <summary>
        /// Runs the pathfinding algorithm on the current puzzle and then deciphers the returned information to draw
        /// a path onto the puzzle for the user to see.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void solveButton_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (isEditMode)
            {
                editButton_Click(sender, e);
            }
            removeGraph();

            List<PathTreeNode> deadEnds = tilePuzzle.solve();

            //Go through the path and draw/highlight it on the screen.
            Canvas graph = new Canvas();
            graph.Width = TilePuzzleContainer_Grid.Width;
            graph.Height = TilePuzzleContainer_Grid.Height;
            TilePuzzleContainer_Grid.Children.Add(graph);

            PathTreeNode currentStep = null;

            //Find the step that goes to the endNode that has the lowest height in the tree (if any)
            foreach(PathTreeNode deadEnd in deadEnds)
            {
                if(deadEnd.col == puzzleColumns)
                {
                    if(currentStep == null || currentStep.height > deadEnd.height)
                    {
                        currentStep = deadEnd;
                    }
                }
            }

            if(currentStep == null)
            {
                //If currentStep == null, no steps were made onto the endNode, so no solution
                MessageBox.Show("No solution found!");
                return;
            }

            //Trace through the steps from endNode w/ lowest height to startNode (when currentStep.parent will == null) to draw through the shortest found path
            while (currentStep.parent != null)
            {
                Rectangle edge = new Rectangle();
                edge.Fill = new SolidColorBrush(Colors.Black);
                if (currentStep.col != currentStep.parent.col)
                {
                    if(currentStep.parent.col == -1)
                    {
                        currentStep.parent.row = currentStep.row;
                    }
                    //horizontal edge
                    edge.Width = 26 + ((Math.Max(currentStep.parent.col, currentStep.col) - Math.Min(currentStep.parent.col, currentStep.col) - 1) * 25);
                    edge.Height = 3;
                    graph.Children.Add(edge);
                    Canvas.SetTop(edge, 25 * currentStep.parent.row + 11);
                    Canvas.SetLeft(edge, 25 + (25 * Math.Min(currentStep.parent.col, currentStep.col) + 12));
                }
                else if (currentStep.row != currentStep.parent.row)
                {
                    //vertical edge
                    edge.Width = 3;
                    edge.Height = 26 + ((Math.Max(currentStep.row, currentStep.parent.row) - Math.Min(currentStep.row, currentStep.parent.row) - 1) * 25);
                    graph.Children.Add(edge);
                    Canvas.SetTop(edge, (25 * Math.Min(currentStep.parent.row, currentStep.row) + 12));
                    Canvas.SetLeft(edge, 25 + (25 * currentStep.parent.col + 11));
                }

                //MessageBox.Show("Step from " + currentStep.row + ", " + currentStep.col + " to " + currentStep.parent.row + ", " + currentStep.parent.col);

                currentStep = currentStep.parent;
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Runs the buildGraph methods to find the edges for all nodes, and then graphically displays them for the user using rectangles in between each tile button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void graphButton_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            if (isEditMode)
            {
                editButton_Click(sender, e);
            }
            removeGraph();

            tilePuzzle.buildGraph();
            isGraphed = true;

            Canvas graph = new Canvas();
            graph.Width = TilePuzzleContainer_Grid.Width;
            graph.Height = TilePuzzleContainer_Grid.Height;
            TilePuzzleContainer_Grid.Children.Add(graph);

            for(int r = 0; r < puzzleRows; r++)
            {
                for(int c = 0; c < puzzleColumns; c++)
                {
                    foreach (Edge anEdge in tilePuzzle.nodes[r,c].edges)
                    {
                        Rectangle edge = new Rectangle();
                        edge.Fill = new SolidColorBrush(Colors.Black);
                        if (anEdge.childNode.col != c)
                        {
                            //horizontal edge
                            edge.Width = 10 + ((Math.Max(c, anEdge.childNode.col) - Math.Min(c, anEdge.childNode.col) - 1) * 25);
                            edge.Height = 3;
                            graph.Children.Add(edge);
                            Canvas.SetTop(edge, 25 * r + 11);
                            Canvas.SetLeft(edge, 25 + (25 * Math.Min(c, anEdge.childNode.col) + 20));
                        }
                        else if (anEdge.childNode.row != r)
                        {
                            //vertical edge
                            edge.Width = 3;
                            edge.Height = 10 + ((Math.Max(r, anEdge.childNode.row) - Math.Min(r, anEdge.childNode.row) - 1) * 25);
                            graph.Children.Add(edge);
                            Canvas.SetTop(edge, (25 * Math.Min(r, anEdge.childNode.row) + 20));
                            Canvas.SetLeft(edge, 25 + (25 * c + 11));
                        }
                    }
                }
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Brings up an open file window for the user to load a text file representing a puzzle, then attempts to build a puzzle from the file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog loadFileWindow = new OpenFileDialog();
            loadFileWindow.Filter = "Text files (*.txt)|*.txt";
            if(loadFileWindow.ShowDialog() == true)
            {
                using (TextReader reader = File.OpenText(loadFileWindow.FileName))
                {
                    try
                    {
                        int loadedPuzzleRows = int.Parse(reader.ReadLine());
                        int loadedPuzzleCols = int.Parse(reader.ReadLine());

                        tilePuzzle.resizePuzzle(loadedPuzzleRows, loadedPuzzleCols);

                        for (int r = 0; r < loadedPuzzleRows; r++)
                        {
                            for (int c = 0; c < loadedPuzzleCols; c++)
                            {
                                tilePuzzle.nodes[r, c] = new Node(int.Parse(reader.ReadLine()), r, c);
                            }
                        }

                        Row_TextBox.Text = loadedPuzzleRows + "";
                        Column_TextBox.Text = loadedPuzzleCols + "";

                        resizeTileGrid(loadedPuzzleRows, loadedPuzzleCols);
                    }
                    catch(FormatException fe)
                    {
                        MessageBox.Show("Invalid File!");
                    }
                }
            }
        }

        /// <summary>
        /// Brings up a save file window that allows the user to name a text file that will store the current puzzle in a format readable by this program's load feature.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileWindow = new SaveFileDialog();
            saveFileWindow.Filter = "Text files (*.txt)|*.txt";
            if(saveFileWindow.ShowDialog() == true)
            {
                String fileText = puzzleRows + Environment.NewLine + puzzleColumns + Environment.NewLine;

                for(int r = 0; r < puzzleRows; r++)
                {
                    for(int c = 0; c < puzzleColumns; c++)
                    {
                        fileText += tilePuzzle.nodes[r, c].color + Environment.NewLine;
                    }
                }

                File.WriteAllText(saveFileWindow.FileName, fileText);
            }
        }

        /// <summary>
        /// Clears the displayed edges by removing the canvas that contains the rectangles that represents them, and resets all relations every tile has to each other.
        /// </summary>
        private void removeGraph()
        {
            //Removes the canvas (which we don't have a reference to) that displays all edges between tiles/nodes (if it's even been made).
            if (TilePuzzleContainer_Grid.Children.Count > 1)
            {
                TilePuzzleContainer_Grid.Children.RemoveAt(1);
            }

            tilePuzzle.resetGraphEdges();
            isGraphed = false;
        }

        /// <summary>
        /// Randomizes the tile colors in the puzzle to make a new puzzle (with the same size)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void randomizeButton_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            removeGraph();
            Random rng = new Random();

            for (int row = 0; row < puzzleRows; row++)
            {
                for(int col = 0; col < puzzleColumns; col++)
                {
                    switch (rng.Next(7))
                    {
                        case 0:
                            tilePuzzle.nodes[row, col].color = 0;
                            (TilePuzzle_UniformGrid.Children[(row * (puzzleColumns + 2)) + col + 1] as Button).Background = getTileColor(0);
                            break;
                        case 1:
                            tilePuzzle.nodes[row, col].color = 1;
                            (TilePuzzle_UniformGrid.Children[(row * (puzzleColumns + 2)) + col + 1] as Button).Background = getTileColor(1);
                            break;
                        case 2:
                            tilePuzzle.nodes[row, col].color = 2;
                            (TilePuzzle_UniformGrid.Children[(row * (puzzleColumns + 2)) + col + 1] as Button).Background = getTileColor(2);
                            break;
                        case 3:
                            tilePuzzle.nodes[row, col].color = 3;
                            (TilePuzzle_UniformGrid.Children[(row * (puzzleColumns + 2)) + col + 1] as Button).Background = getTileColor(3);
                            break;
                        case 4:
                            tilePuzzle.nodes[row, col].color = 4;
                            (TilePuzzle_UniformGrid.Children[(row * (puzzleColumns + 2)) + col + 1] as Button).Background = getTileColor(4);
                            break;
                        case 5:
                            tilePuzzle.nodes[row, col].color = 5;
                            (TilePuzzle_UniformGrid.Children[(row * (puzzleColumns + 2)) + col + 1] as Button).Background = getTileColor(5);
                            break;
                        case 6:
                            tilePuzzle.nodes[row, col].color = 6;
                            (TilePuzzle_UniformGrid.Children[(row * (puzzleColumns + 2)) + col + 1] as Button).Background = getTileColor(6);
                            break;
                        default:
                            tilePuzzle.nodes[row, col].color = 6;
                            (TilePuzzle_UniformGrid.Children[(row * (puzzleColumns + 2)) + col + 1] as Button).Background = getTileColor(6);
                            break;
                    }
                }
            }
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// For testing, continuously generates a random puzzle, saves it, then trys to solve it, so if it encounters a bug and crashes, the last saved
        /// randomized.txt puzzle will contain a situation that causes an unanticipated bug.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DebugButton_Click(object sender, RoutedEventArgs e)
        {
            while(true)
            {
                randomizeButton_Click(sender, e);

                String fileText = puzzleRows + Environment.NewLine + puzzleColumns + Environment.NewLine;

                for (int r = 0; r < puzzleRows; r++)
                {
                    for (int c = 0; c < puzzleColumns; c++)
                    {
                        fileText += tilePuzzle.nodes[r, c].color + Environment.NewLine;
                    }
                }

                File.WriteAllText("randomized.txt", fileText);

                solveButton_Click(sender, e);
            }
        }

        /// <summary>
        /// Resizes the puzzle based on the #rows and columns input by the user in the respective text boxes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeButton_Click(object sender, RoutedEventArgs e)
        {
            int rows, cols;
            if (int.TryParse(Row_TextBox.Text, out rows) && int.TryParse(Column_TextBox.Text, out cols) && rows >= 0 && cols >= 0)
            {
                if(rows > 2147483645 || cols > 2147483645)
                {
                    MessageBox.Show("Maximum value is 2147483645"); //slightly less than 2^31 -1, but maximum value we'll allow (there's a few places puzzleRow/Col gets <=2 added to it)
                    Row_TextBox.Text = puzzleRows.ToString();
                    Column_TextBox.Text = puzzleColumns.ToString();
                    return;
                }
                resizeTileGrid(rows, cols);
            }
            else
            {
                MessageBox.Show("Please enter valid 32 bit integer values");
                Row_TextBox.Text = puzzleRows.ToString();
                Column_TextBox.Text = puzzleColumns.ToString();
            }
        }

        /// <summary>
        /// Adds/removes extra weight to green tiles if the checkBox is checked/unchecked, respectively.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void avoidGreen_Clicked(object sender, RoutedEventArgs e)
        {
            removeGraph();
            int greenWeight = 0;
            if(AvoidGreen_CheckBox.IsChecked == true)
            {
                greenWeight = puzzleColumns;
            }

            for(int r = 0; r < puzzleRows;  r++)
            {
                for(int c = 0; c < puzzleColumns; c++)
                {
                    if(tilePuzzle.nodes[r, c].color == 3)
                    {
                        tilePuzzle.nodes[r, c].weight = greenWeight;
                    }
                }
            }
        }

        /// <summary>
        /// Pops up a window explaining the program/how to use it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpButton_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow hw = new HelpWindow();
            hw.Show();
        }
    }

}
