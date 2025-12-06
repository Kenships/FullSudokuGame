using UnityEngine;

namespace SudokuLogic
{
    public class SudokuLogicCore
    {
        public enum EvaluationState
        {
            Solved,
            Unsolved,
            Incorrect,
            Correct,
            Impossible
        }
        
        struct GridValue
        {
            public int Value{get; set;}
            public readonly bool[] notes;

            public GridValue(int value, bool[] notes)
            {
                this.Value = value;
                this.notes = notes;
            }
        }

        public EvaluationState EvaluateSudoku(GridUnit[,] sudokuInput)
        {
            GridValue[,] currentSudokuState = ParseSudokuGridUnits(sudokuInput);
            
            return IsValidSudoku(currentSudokuState) ? EvaluationState.Solved : EvaluationState.Unsolved;
        }
        
        #region Validation

        private bool IsValidSudoku(GridValue[,] currentSudokuState)
        {
            if (!IsSudokuFilled(currentSudokuState))
            {
                return false;
            }
            
            bool[] sectionCheck = new bool[3];
            for (int i = 0; i < 9; i++)
            {
                sectionCheck[0] = IsValidRow(currentSudokuState, i);
                sectionCheck[1] = IsValidCol(currentSudokuState, i);
                sectionCheck[2] = IsValidBox(currentSudokuState, i);
                
                if (!IsAllTrue(sectionCheck))
                {
                    return false;
                }
            }
            
            return true;
        }

        private bool IsValidRow(GridValue[,] currentSudokuState, int row)
        {
            bool[] valueCheck = new bool[10];
            for (int col = 0; col < 9; col++)
            {
                valueCheck[currentSudokuState[row, col].Value] = true;
            }
            
            return IsAllTrue(valueCheck, true);
        }

        private bool IsValidCol(GridValue[,] currentSudokuState, int col)
        {
            bool[] valueCheck = new bool[10];
            for (int row = 0; row < 9; row++)
            {
                valueCheck[currentSudokuState[row, col].Value] = true;
            }
            
            return IsAllTrue(valueCheck, true);
        }

        private bool IsValidBox(GridValue[,] currentSudokuState, int boxNumber)
        {
            int boxRow = boxNumber / 3;
            int boxCol = boxNumber % 3;
            
            return IsValidBox(currentSudokuState, boxRow, boxCol);
        }

        private bool IsValidBox(GridValue[,] currentSudokuState, int row, int col)
        {
            int boxRow = row / 3 * 3;
            int boxCol = col / 3 * 3;
            
            bool[] valueCheck = new bool[10];

            for (int r = boxRow; r < boxRow + 3; r++)
            {
                for (int c = boxCol; c < boxCol + 3; c++)
                {
                    valueCheck[currentSudokuState[r, c].Value] = true;
                }
            }
            
            return IsAllTrue(valueCheck, true);
        }

        private bool IsSudokuFilled(GridValue[,] currentSudokuState)
        {
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (currentSudokuState[r, c].Value == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        
        #endregion
        
        #region Util

        private GridValue[,] ParseSudokuGridUnits(GridUnit[,] sudoku)
        {
            GridValue[,] parsedSudoku = new GridValue[9, 9];
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    GridUnit unit = sudoku[row, col];
                    GridValue sudokuValue = new GridValue(unit.GetValue(), unit.GetNotes());
                    parsedSudoku[row, col] = sudokuValue;
                }
            }
            return parsedSudoku;
        }

        private bool IsAllTrue(bool[] values, bool falseOnZero = false)
        {
            if (values[0] && falseOnZero)
                return false;
            
            for (int row = falseOnZero ? 1 : 0; row < values.Length; row++)
            {
                if (!values[row])
                {
                    return false;
                }
            }
            
            return true;
        }
        
        #endregion
    }
}
