using System.Collections.Generic;
using UnityEngine;

namespace SudokuLogic
{
    public class SudokuGridHighlightService
    {
        private readonly List<ClueVisual> m_highlightedGridUnits = new();
        private readonly List<ClueVisual> m_boldedGridUnits = new();
        
        public void HighlightAndBoldSelectRegion(ClueVisual selectedUnit, ClueVisual[,] sudokuGrid)
        {
            LoadSelectRegion(selectedUnit, sudokuGrid);
            HighlightWithSelectCondition(selectedUnit);
        }
        
        public void UnhighlightAndBoldAll()
        {
            foreach (var clueVisual in m_highlightedGridUnits)
            {
                clueVisual.Unhighlight();
            }

            foreach (var clueVisual in m_boldedGridUnits)
            {
                clueVisual.UnboldAll();
            }
            
            m_highlightedGridUnits.Clear();
            m_boldedGridUnits.Clear();
        }

        private void HighlightWithSelectCondition(ClueVisual selectedClue)
        {
            int selectedValue = selectedClue.Value;
            foreach (ClueVisual gridUnit in m_highlightedGridUnits)
            {
                int value = gridUnit.Value;

                bool selected = (gridUnit == selectedClue) || (value != 0 && value == selectedValue);
            
                gridUnit.Highlight(selected);
            }

            foreach (ClueVisual gridUnit in m_boldedGridUnits)
            {
                gridUnit.BoldNote(selectedValue);
            }
        }

        private void LoadSelectRegion(ClueVisual selectedClue, ClueVisual[,] sudokuGrid)
        {
            Vector2Int position = selectedClue.Position;
            
            int boxRow = position.x / 3 * 3;
            int boxCol = position.y / 3 * 3;

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (position.x == row || position.y == col)
                    {
                        m_highlightedGridUnits.Add(sudokuGrid[row, col]);
                    }

                    if (row >= boxRow && col >= boxCol && row < boxRow + 3 && col < boxCol + 3)
                    {
                        m_highlightedGridUnits.Add(sudokuGrid[row, col]);
                    }

                    if (selectedClue.Value == 0)
                    {
                        continue;
                    }

                    if (sudokuGrid[row, col].Value == selectedClue.Value)
                    {
                        m_highlightedGridUnits.Add(sudokuGrid[row, col]);
                    }

                    if (sudokuGrid[row, col].HasNote(selectedClue.Value))
                    {
                        m_boldedGridUnits.Add(sudokuGrid[row, col]);
                    }
                }
            }
        }
    }
}
