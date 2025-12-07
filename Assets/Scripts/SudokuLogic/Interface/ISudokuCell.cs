using UnityEngine;

namespace SudokuLogic.Interface
{
    public interface ISudokuCell
    {
        public int Value { get; set; }
        public bool[] Notes { get; }
        public Vector2Int Position { get; }
    }
}
