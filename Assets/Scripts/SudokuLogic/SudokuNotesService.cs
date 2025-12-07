using System.Collections.Generic;
using SudokuLogic.Interface;


namespace SudokuLogic
{
    public class SudokuNotesService<T> where T : ISudokuCell 
    {
        private readonly SudokuGridExtractor<T> m_extractor;
        
        public SudokuNotesService(SudokuGridExtractor<T> extractor)
        {
            m_extractor = extractor;
        }
        
        public List<T> RemoveNotesWithValue(T clue, T[,] sudokuBoard)
        {
            List<T> removedFrom = new List<T>();
            
            int value = clue.Value;
            
            T[] row = m_extractor.ExtractFrom(sudokuBoard, ExtractorType.row, clue.Position.x);
            T[] col = m_extractor.ExtractFrom(sudokuBoard, ExtractorType.column, clue.Position.y);
            T[] box = m_extractor.ExtractFrom(sudokuBoard, ExtractorType.box, clue.Position.x, clue.Position.y);

            for (int i = 0; i < 9; i++)
            {
                if (row[i].Notes[value - 1])
                {
                    removedFrom.Add(row[i]);
                }

                if (col[i].Notes[value - 1])
                {
                    removedFrom.Add(col[i]);
                }

                if (box[i].Notes[value - 1])
                {
                    removedFrom.Add(box[i]);
                }
            }

            foreach (T c in removedFrom)
            {
                c.Notes[value - 1] = false;
            }
            
            return removedFrom;
        }
    }
}
