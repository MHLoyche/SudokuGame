using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SudokuGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TextBox[,] numberBlocks;
        private SudokuLevel currentLevel;
        private SudokuViewModel sudoku;

        public MainWindow()
        {
            InitializeComponent();
            GenerateGridBorders();
        }

        private void GenerateGridBorders()
        {
            numberBlocks = new TextBox[9, 9];

            // Create borders for the Sudoku board
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    // Creating textbox for each cell
                    numberBlocks[row, col] = new TextBox
                    {
                        FontSize = 8,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        MaxLength = 1, // Only allow one character
                        TextAlignment = TextAlignment.Center,
                        BorderThickness = new Thickness(0),
                        Padding = new Thickness(0),
                    };
                    // Calling the handlers to safeguard against letters and pasting in the cells
                    numberBlocks[row, col].PreviewTextInput += NumberBox_PreviewTextInput;
                    DataObject.AddPastingHandler(numberBlocks[row, col], NumberBox_Pasting);

                    // Adding the functionality to navigate using arrow keys
                    int localRow = row;
                    int localCol = col;
                    numberBlocks[row, col].PreviewKeyDown += (s, e) => NumberBox_PreviewKeyDown(s, e, localRow, localCol);

                    // Default thin border
                    double left = 0.1, top = 0.1, right = 0.1, bottom = 0.1;

                    // Thicker border on the left of each 3x3 block
                    if (col % 3 == 0) left = 0.5;
                    // Thicker border on the top of each 3x3 block
                    if (row % 3 == 0) top = 0.5;
                    // Thicker border on the right of the last cell in a block
                    if (col == 8) right = 0.5;
                    else if ((col + 1) % 3 == 0) right = 0.5;
                    // Thicker border on the bottom of the last cell in a block
                    if (row == 8) bottom = 0.5;
                    else if ((row + 1) % 3 == 0) bottom = 0.5;

                    var border = new Border
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(left, top, right, bottom),
                        Child = numberBlocks[row, col]
                    };
                    Grid.SetRow(border, row);
                    Grid.SetColumn(border, col);
                    SudokuGrid.Children.Add(border);
                }
            }
        }

        // Confirming input is numbers only
        private void NumberBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only digits 1-9
            e.Handled = !(e.Text.Length == 1 && e.Text[0] >= '1' && e.Text[0] <= '9');
        }

        // Preventing pasting of invalid characters
        private void NumberBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (text.Length != 1 || text[0] < '1' || text[0] > '9')
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void NewGame(object sender, RoutedEventArgs e)
        {
            int[,] board = new int[9, 9];
            FillBoard(board);
            RemoveNumbers(board, currentLevel);

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (board[row, col] == 0)
                    {
                        numberBlocks[row, col].Text = "";
                        numberBlocks[row, col].IsReadOnly = false;
                        numberBlocks[row, col].Foreground = Brushes.Black;
                    }
                    else
                    {
                        numberBlocks[row, col].Text = board[row, col].ToString();
                        numberBlocks[row, col].IsReadOnly = true;
                        numberBlocks[row, col].Foreground = Brushes.DarkBlue;
                    }
                }
            }
        }

        // Allowing navigation using arrow keys

        private void NumberBox_PreviewKeyDown(object sender, KeyEventArgs e, int row, int col)
        {
            switch (e.Key)
            {
                case Key.Up:
                    if (row > 0)
                        numberBlocks[row - 1, col].Focus();
                    e.Handled = true;
                    break;
                case Key.Down:
                    if (row < 8)
                        numberBlocks[row + 1, col].Focus();
                    e.Handled = true;
                    break;
                case Key.Left:
                    if (col > 0)
                        numberBlocks[row, col - 1].Focus();
                    e.Handled = true;
                    break;
                case Key.Right:
                    if (col < 8)
                        numberBlocks[row, col + 1].Focus();
                    e.Handled = true;
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        // Event handler for the difficulty slider calling on the SudokuLevel enum
        private void DifficultySlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            currentLevel = (SudokuLevel)e.NewValue;
            DifficultyTextBlock.Text = currentLevel.ToString().ToUpper();
        }

        private bool FillBoard(int[,] board, int row = 0, int col = 0)
        {
            if (row == 9) return true;
            if (col == 9) return FillBoard(board, row + 1, 0);

            var numbers = Enumerable.Range(1, 9).OrderBy(_ => Guid.NewGuid()).ToList();
            foreach (var num in numbers)
            {
                if (IsSafe(board, row, col, num))
                {
                    board[row, col] = num;
                    if (FillBoard(board, row, col + 1)) return true;
                    board[row, col] = 0;
                }
            }
            return false;
        }

        private bool IsSafe(int[,] board, int row, int col, int num)
        {
            for (int i = 0; i < 9; i++)
                if (board[row, i] == num || board[i, col] == num)
                    return false;

            int startRow = row / 3 * 3, startCol = col / 3 * 3;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (board[startRow + i, startCol + j] == num)
                        return false;

            return true;
        }

        private void RemoveNumbers(int[,] board, SudokuLevel level)
        {
            int cellsToRemove = level switch
            {
                SudokuLevel.Easy => 35,
                SudokuLevel.Medium => 45,
                SudokuLevel.Hard => 60,
                _ => 35
            };

            var rand = new Random();
            while (cellsToRemove > 0)
            {
                int row = rand.Next(0, 9);
                int col = rand.Next(0, 9);
                if (board[row, col] != 0)
                {
                    board[row, col] = 0;
                    cellsToRemove--;
                }
            }
        }
    }
}

