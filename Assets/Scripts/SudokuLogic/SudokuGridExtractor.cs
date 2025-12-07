namespace SudokuLogic
{
    public enum ExtractorType
    {
        row,
        column,
        box
    }
    
    public class SudokuGridExtractor<T>
    {
        public T[] ExtractFrom(T[,] sudoku, ExtractorType extractorType, int index, int secondIndex = -1)
        {
            switch (extractorType)
            {
                case ExtractorType.row:
                    return GetRow(sudoku, index);
                case ExtractorType.column:
                    return GetColumn(sudoku, index);
                case ExtractorType.box:
                    return secondIndex < 0 ? GetBox(sudoku, index) : GetBox(sudoku, index, secondIndex);
                default:
                    return new T[9];
            }
        }

        private T[] GetRow(T[,] sudoku, int rowIndex)
        {
            T[] rowResult = new T[9];

            for (int col = 0; col < 9; col++)
            {
                rowResult[col] = sudoku[rowIndex, col];
            }
            
            return rowResult;
        }

        private T[] GetColumn(T[,] sudoku, int columnIndex)
        {
            T[] columnResult = new T[9];

            for (int row = 0; row < 9; row++)
            {
                columnResult[row] = sudoku[row, columnIndex];
            }
            
            return columnResult;
        }

        private T[] GetBox(T[,] sudoku, int boxIndex)
        {
            int boxRow = boxIndex / 3;
            int boxCol = boxIndex % 3;
            
            return GetBox(sudoku, boxRow, boxCol);
        }

        private T[] GetBox(T[,] sudoku, int rowIndex, int columnIndex)
        {
            T[] boxResult = new T[9];
            int boxRow = rowIndex / 3 * 3;
            int boxCol = columnIndex / 3 * 3;

            for (int row = boxRow; row < boxRow + 3; row++)
            {
                for (int col = boxCol; col < boxCol + 3; col++)
                {
                    boxResult[(row - boxRow) * 3 + (col - boxCol)] = sudoku[row, col];
                }
            }
            
            return boxResult;
        }
    }
}
