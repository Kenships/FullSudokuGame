using DefaultNamespace;
using SudokuLogic.Interface;
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
        
        public struct Clue : ISudokuCell
        {
            public int Value{get; set;}
            public bool[] Notes { get; }
            public Vector2Int Position { get; }

            public Clue(int value, bool[] notes, Vector2Int position)
            {
                Value = value;
                Notes = notes;
                Position = position;
            }
        }

        private SudokuGridExtractor<Clue> m_extractor;
        
        public SudokuLogicCore(SudokuGridExtractor<Clue> extractor)
        {
            m_extractor = extractor;
        }

        public EvaluationState EvaluateSudoku(ClueVisual[,] sudokuInput)
        {
            Clue[,] currentSudokuState = ParseSudokuGridUnits(sudokuInput);
            
            return IsValidSudoku(currentSudokuState) ? EvaluationState.Solved : EvaluationState.Unsolved;
        }
        
        #region Validation

        private bool IsValidSudoku(Clue[,] currentSudokuState)
        {
            if (!IsSudokuFilled(currentSudokuState))
            {
                return false;
            }
            
            bool[] sectionCheck = new bool[3];
            for (int i = 0; i < 9; i++)
            {
                sectionCheck[0] = IsValidSection(currentSudokuState, ExtractorType.row, i);
                sectionCheck[1] = IsValidSection(currentSudokuState, ExtractorType.column, i);
                sectionCheck[2] = IsValidSection(currentSudokuState, ExtractorType.box, i);
                
                if (!IsAllTrue(sectionCheck))
                {
                    return false;
                }
            }
            
            return true;
        }

        private bool IsValidSection(Clue[,] currentSudokuState, ExtractorType extractorType, int index)
        {
            Clue[] boxValues = m_extractor.ExtractFrom(currentSudokuState, extractorType, index);
            bool[] valueCheck = new bool[10];
            foreach (Clue value in boxValues)
            {
                valueCheck[value.Value] = true;
            }
            return IsAllTrue(valueCheck, true);
        }

        private bool IsSudokuFilled(Clue[,] currentSudokuState)
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

        private Clue[,] ParseSudokuGridUnits(ClueVisual[,] sudoku)
        {
            Clue[,] parsedSudoku = new Clue[9, 9];
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    ClueVisual unit = sudoku[row, col];
                    Clue sudokuClue = new Clue(unit.Value, unit.Notes, new Vector2Int(row, col));
                    parsedSudoku[row, col] = sudokuClue;
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
