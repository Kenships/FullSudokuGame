using UnityEngine;

[CreateAssetMenu(fileName = "NewSudoku", menuName = "Sudoku/Sudoku Grid")]
public class SudokuSO : ScriptableObject
{
    // Use a serializable wrapper for rows so Unity can serialize it cleanly
    [SerializeField] private SudokuRow[] rows = new SudokuRow[9];
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Ensure 9x9 size and default values
        if (rows == null || rows.Length != 9)
        {
            rows = new SudokuRow[9];
        }

        for (int r = 0; r < 9; r++)
        {
            if (rows[r] == null)
            {
                rows[r] = new SudokuRow();
            }

            if (rows[r].cells == null || rows[r].cells.Length != 9)
            {
                rows[r].cells = new int[9];
            }

            // Ensure default 0 if anything uninitialized (Unity already zeros, but for safety)
            for (int c = 0; c < 9; c++)
            {
                // nothing to do here normally; kept for clarity
                // rows[r].cells[c] = Mathf.Clamp(rows[r].cells[c], 0, 9); // if you want 0–9 only
            }
        }
    }
#endif
    /// <summary>
    /// Convenience getter/setter to treat it as a 2D array in code.
    /// </summary>
    public int this[int row, int col]
    {
        get => rows[row].cells[col];
        set => rows[row].cells[col] = value;
    }

    public int GetValue(int row, int col)
    {
        return rows[row].cells[col];
    }
}

[System.Serializable]
public class SudokuRow
{
    public int[] cells = new int[9]; // Unity serializes this fine
}
