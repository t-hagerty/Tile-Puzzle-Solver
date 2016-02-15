using System;
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
        private bool isEditMode = false;
        private int selectedColor = 6;
        private int puzzleColumns = 39;
        private int puzzleRows = 4;
        Button[,] tilePuzzleGrid;
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
            tilePuzzle = new TilePuzzleModel(puzzleRows, puzzleColumns, startPuzzle);
            tilePuzzleGrid = new Button[4, 39];

            resizeTileGrid(4, 39);

            //PathTreeNode root = new PathTreeNode(0, -1);
            //PathTreeNode p1 = new PathTreeNode(0, 0, root);
            //PathTreeNode p2 = new PathTreeNode(0, 1, p1);
            //PathTreeNode p3 = new PathTreeNode(0, 3, p2);
            //PathTreeNode p4 = new PathTreeNode(0, 1, p3);
            //PathTreeNode p5 = new PathTreeNode(0, 4, p3);
            //PathTreeNode p6 = new PathTreeNode(0, 5, p5);
        }

        /// <summary>
        /// Resizes the puzzle based on input rows and columns, and rebuilds/replaces the visuals/buttons representing the puzzle in the View.
        /// </summary>
        /// <param name="newRows">The new amount of rows of the puzzle</param>
        /// <param name="newCols">The new amount of columns o the puzzle</param>
        public void resizeTileGrid(int newRows, int newCols)
        {
            TilePuzzle_UniformGrid.Children.Clear();

            puzzleColumns = newCols;
            puzzleRows = newRows;
            TilePuzzle_UniformGrid.Columns = puzzleColumns + 2;
            TilePuzzle_UniformGrid.Rows = puzzleRows;
            tilePuzzle.resizePuzzle(puzzleRows, puzzleColumns);
            TilePuzzle_UniformGrid.Width = (TilePuzzle_UniformGrid.Columns) * 25;
            TilePuzzle_UniformGrid.Height = TilePuzzle_UniformGrid.Rows * 25;
            TilePuzzleContainer_Grid.Width = TilePuzzle_UniformGrid.Width;
            TilePuzzleContainer_Grid.Height = TilePuzzle_UniformGrid.Height;

            Button[,] newTilePuzzleGrid = new Button[puzzleRows, puzzleColumns + 2];

            for(int r = 0; r < puzzleRows; r++)
            {
                newTilePuzzleGrid[r, 0] = new Button();
                newTilePuzzleGrid[r, 0].Background = new SolidColorBrush(Colors.Gray);
                newTilePuzzleGrid[r, 0].IsEnabled = false;
                newTilePuzzleGrid[r, 0].BorderThickness = new Thickness(0);

                for (int c = 1; c <= puzzleColumns; c++)
                {
                    newTilePuzzleGrid[r, c] = new Button();
                    newTilePuzzleGrid[r, c].Click += editTile_Click;
                    newTilePuzzleGrid[r, c].BorderThickness = new Thickness(0);
                    switch (tilePuzzle.nodes[r,c-1].color)
                    {
                        case 0:
                            newTilePuzzleGrid[r, c].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF72015")); //red
                            break;
                        case 1:
                            newTilePuzzleGrid[r, c].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF98626")); //orange
                            break;
                        case 2:
                            newTilePuzzleGrid[r, c].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFF637")); //yellow
                            break;
                        case 3:
                            newTilePuzzleGrid[r, c].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF6EFF37")); //green
                            break;
                        case 4:
                            newTilePuzzleGrid[r, c].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF48028F")); //blue
                            break;
                        case 5:
                            newTilePuzzleGrid[r, c].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC303C3")); //purple
                            break;
                        case 6:
                            newTilePuzzleGrid[r, c].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFF6CEB")); //pink
                            break;
                        default:
                            newTilePuzzleGrid[r, c].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFF6CEB")); //pink
                            break;
                    }
                }

                newTilePuzzleGrid[r, puzzleColumns + 1] = new Button();
                newTilePuzzleGrid[r, puzzleColumns + 1].Background = new SolidColorBrush(Colors.Gray);
                newTilePuzzleGrid[r, puzzleColumns + 1].IsEnabled = false;
                newTilePuzzleGrid[r, puzzleColumns + 1].BorderThickness = new Thickness(0);
            }

            tilePuzzleGrid = newTilePuzzleGrid;

            for(int r = 0; r < puzzleRows; r++)
            {
                for(int c = 0; c < puzzleColumns + 2; c++)
                {
                    TilePuzzle_UniformGrid.Children.Add(tilePuzzleGrid[r, c]);
                }
            }
        }

        /// <summary>
        /// Changes the color of a tile to the selected color if in edit mode.
        /// 
        /// If not in edit mode, doubles as a function to pop up info about the edges a node has (for testing purposes)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editTile_Click(object sender, RoutedEventArgs e)
        {
            int index = TilePuzzle_UniformGrid.Children.IndexOf(sender as Button);
            int row = index / (puzzleColumns + 2);
            int col = (index % (puzzleColumns + 2)) - 1;

            if (isEditMode)
            {
                switch (selectedColor)
                {
                    case 0:
                        (sender as Button).Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF72015")); //red
                        
                        tilePuzzle.nodes[row, col].color = 0;
                        break;
                    case 1:
                        (sender as Button).Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF98626")); //orange
                        tilePuzzle.nodes[row, col].color = 1;
                        break;
                    case 2:
                        (sender as Button).Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFF637")); //yellow
                        tilePuzzle.nodes[row, col].color = 2;
                        break;
                    case 3:
                        (sender as Button).Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF6EFF37")); //green
                        tilePuzzle.nodes[row, col].color = 3;
                        break;
                    case 4:
                        (sender as Button).Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF48028F")); //blue
                        tilePuzzle.nodes[row, col].color = 4;
                        break;
                    case 5:
                        (sender as Button).Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC303C3")); //purple
                        tilePuzzle.nodes[row, col].color = 5;
                        break;
                    case 6:
                        (sender as Button).Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFF6CEB")); //pink
                        tilePuzzle.nodes[row, col].color = 6;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                MessageBox.Show(tilePuzzle.nodes[row, col].edgesToString());
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
            Red_Button.IsEnabled = !Red_Button.IsEnabled;
            Orange_Button.IsEnabled = !Orange_Button.IsEnabled;
            Yellow_Button.IsEnabled = !Yellow_Button.IsEnabled;
            Green_Button.IsEnabled = !Green_Button.IsEnabled;
            Blue_Button.IsEnabled = !Blue_Button.IsEnabled;
            Purple_Button.IsEnabled = !Purple_Button.IsEnabled;
            Pink_Button.IsEnabled = !Pink_Button.IsEnabled;
            isEditMode = !isEditMode;

            removeGraph();
        }

        private void redButton_Click(object sender, RoutedEventArgs e)
        {
            selectedColor = 0;
        }

        private void orangeButton_Click(object sender, RoutedEventArgs e)
        {
            selectedColor = 1;
        }

        private void yellowButton_Click(object sender, RoutedEventArgs e)
        {
            selectedColor = 2;
        }

        private void greenButton_Click(object sender, RoutedEventArgs e)
        {
            selectedColor = 3;
        }

        private void blueButton_Click(object sender, RoutedEventArgs e)
        {
            selectedColor = 4;
        }

        private void purpleButton_Click(object sender, RoutedEventArgs e)
        {
            selectedColor = 5;
        }

        private void pinkButton_Click(object sender, RoutedEventArgs e)
        {
            selectedColor = 6;
        }

        /// <summary>
        /// Fires whenever the text is changed in the row/column text boxes. Must check and make sure the input is valid before doing anything to the puzzle/size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int rows, cols;
            if (Row_TextBox != null && Column_TextBox != null && TilePuzzle_UniformGrid != null) 
            //WPF fires textchanged event when initializing components, column_textbox gets initialized first and triggers this event, but row_textbox is still uninitialized
            //Also, the puzzle uniformgrid is initialized after the two textboxes, so it tries to run resizeTileGrid on the uninitialized uniformgrid unless we make sure it's not null
            {
                if(Row_TextBox.Text == "" || Column_TextBox.Text == "") { /*Do nothing, wait for input*/ }
                else if (int.TryParse(Row_TextBox.Text, out rows) && int.TryParse(Column_TextBox.Text, out cols) && rows >= 0 && cols >= 0)
                {
                    resizeTileGrid(rows, cols);
                    puzzleRows = rows;
                    puzzleColumns = cols;
                }
                else
                {
                    MessageBox.Show("Please enter a valid integer value");
                    Row_TextBox.Text = puzzleRows.ToString();
                    Column_TextBox.Text = puzzleColumns.ToString();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void solveButton_Click(object sender, RoutedEventArgs e)
        {
            if (isEditMode)
            {
                editButton_Click(sender, e);
            }
            removeGraph();

            //Something representing path, maybe ordered list of tuples? = tilePuzzle.Solve();
            List<PathTreeNode> deadEnds = tilePuzzle.solve();

            //Go through the path and draw/highlight it on the screen.
            Canvas graph = new Canvas();
            graph.Width = TilePuzzleContainer_Grid.Width;
            graph.Height = TilePuzzleContainer_Grid.Height;
            TilePuzzleContainer_Grid.Children.Add(graph);

            PathTreeNode currentStep = null;

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
                MessageBox.Show("No solution found!");
                return;
            }

            while (currentStep.parent != null)
            {
                Rectangle edge = new Rectangle();
                edge.Fill = new SolidColorBrush(Colors.Black);
                if (currentStep.row != currentStep.parent.row && (currentStep.row != -2 && currentStep.parent.row != -2))
                {
                    //vertical edge
                    edge.Width = 3;
                    edge.Height = 26 + ((Math.Max(currentStep.row, currentStep.parent.row) - Math.Min(currentStep.row, currentStep.parent.row) - 1) * 25);
                    graph.Children.Add(edge);
                    Canvas.SetTop(edge, (25 * Math.Min(currentStep.parent.row, currentStep.row) + 12));
                    Canvas.SetLeft(edge, 25 + (25 * currentStep.parent.col + 11));
                }
                else if (currentStep.col != currentStep.parent.col && (currentStep.col != -2 && currentStep.parent.col != -2))
                {
                    //horizontal edge
                    edge.Width = 26 + ((Math.Max(currentStep.parent.col, currentStep.col) - Math.Min(currentStep.parent.col, currentStep.col) - 1) * 25);
                    edge.Height = 3;
                    graph.Children.Add(edge);
                    Canvas.SetTop(edge, 25 * currentStep.parent.row + 11);
                    Canvas.SetLeft(edge, 25 + (25 * Math.Min(currentStep.parent.col, currentStep.col) + 12));
                }

                MessageBox.Show("Step from " + currentStep.row + ", " + currentStep.col + " to " + currentStep.parent.row + ", " + currentStep.parent.col);

                currentStep = currentStep.parent;
            }
            
        }

        /// <summary>
        /// Runs the buildGraph methods to find the edges for all nodes, and then graphically displays them for the user using rectangles in between each tile button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void graphButton_Click(object sender, RoutedEventArgs e)
        {
            if(isEditMode)
            {
                editButton_Click(sender, e);
            }
            removeGraph();

            tilePuzzle.buildGraph();

            Canvas graph = new Canvas();
            graph.Width = TilePuzzleContainer_Grid.Width;
            graph.Height = TilePuzzleContainer_Grid.Height;
            TilePuzzleContainer_Grid.Children.Add(graph);

            for(int r = 0; r < puzzleRows; r++)
            {
                for(int c = 0; c < puzzleColumns; c++)
                {
                    //FOR TESTING:
                    int i = 0;

                    foreach (Edge anEdge in tilePuzzle.nodes[r,c].edges)
                    {
                        Rectangle edge = new Rectangle();
                        edge.Fill = new SolidColorBrush(Colors.Black);
                        if(anEdge.childRow != anEdge.parentRow && (anEdge.childRow != -2 && anEdge.parentRow != -2))
                        {
                            //vertical edge
                            edge.Width = 3;
                            edge.Height = 10 + ((Math.Max(anEdge.parentRow, anEdge.childRow) - Math.Min(anEdge.parentRow, anEdge.childRow) - 1) * 25);
                            graph.Children.Add(edge);
                            Canvas.SetTop(edge, (25 * Math.Min(anEdge.parentRow, anEdge.childRow) + 20));
                            //Canvas.SetLeft(edge, 25 + (25 * anEdge.parentCol + 11));

                            //FOR TESTING:
                            Canvas.SetLeft(edge, 25 + (25 * anEdge.parentCol + 3 + i));
                            i = i + 5;
                        }
                        else if(anEdge.childCol != anEdge.parentCol && (anEdge.childCol != -2 && anEdge.parentCol != -2))
                        {
                            //horizontal edge
                            edge.Width = 10 + ((Math.Max(anEdge.parentCol, anEdge.childCol) - Math.Min(anEdge.parentCol, anEdge.childCol) - 1) * 25);
                            edge.Height = 3;
                            graph.Children.Add(edge);
                            //Canvas.SetTop(edge, 25 * anEdge.parentRow + 11);
                            Canvas.SetLeft(edge, 25 + (25 * Math.Min(anEdge.parentCol, anEdge.childCol) + 20));

                            //FOR TESTING:
                            Canvas.SetTop(edge, 25 * anEdge.parentRow + 3 + i);
                            i = i + 5;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Brings up an open file window for the user to load a text file representing a puzzle, then attempts to build a puzzle from the file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            removeGraph();

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
                        puzzleRows = loadedPuzzleRows;
                        puzzleColumns = loadedPuzzleCols;

                        for (int r = 0; r < loadedPuzzleRows; r++)
                        {
                            for (int c = 0; c < loadedPuzzleCols; c++)
                            {
                                tilePuzzle.nodes[r, c] = new Node(int.Parse(reader.ReadLine()));
                            }
                        }

                        Row_TextBox.Text = loadedPuzzleRows + "";
                        Column_TextBox.Text = loadedPuzzleCols + "";
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
                String fileText = tilePuzzle.rows + Environment.NewLine + tilePuzzle.cols + Environment.NewLine;

                for(int r = 0; r < tilePuzzle.rows; r++)
                {
                    for(int c = 0; c < tilePuzzle.cols; c++)
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
        }
    }

}
