using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGame
{
    internal class SudokuBoard
    {
        // Function to fill the Sudoku board with numbers, returns false if failed.
        public bool FillBoard(int[,] board, int row = 0, int col = 0)
        {
            if (row == 9) return true;
            // If we reach the end of the row, move to the next row
            if (col == 9) return FillBoard(board, row + 1, 0);

            // Creates a random order of numbers between 1 to 9, using Guid.NewGuid() to shuffle
            var numbers = Enumerable.Range(1, 9).OrderBy(_ => Guid.NewGuid()).ToList();

            // Calling the IsSafe function to check if the number can be placed in the cell
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

        // Function to check if a number can be placed in the cell following Sudoku rules
        public bool IsSafe(int[,] board, int row, int col, int num)
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

        // Function to remove numbers from the board based on the difficulty level
        public void RemoveNumbers(int[,] board, SudokuLevel level)
        {
            int cellsToRemove = level switch
            {
                SudokuLevel.Easy => 35,
                SudokuLevel.Medium => 45,
                SudokuLevel.Hard => 60,
                _ => 35
            };

            // using Random to remove cells
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

        public bool IsUserInputValid(int[,] board, int row, int col, int value)
        {
            if (value < 1 || value > 9)
                return false;

            // Check row and column
            for (int i = 0; i < 9; i++)
            {
                if (board[row, i] == value)
                    return false;
                if (board[i, col] == value)
                    return false;
            }

            // Check 3x3 box, using integer division to find the first row and col of each 3x3 box
            int startRow = (row / 3) * 3;
            int startCol = (col / 3) * 3;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[startRow + i, startCol + j] == value)
                        return false;
                }
            }

            return true;
        }

    }
}
