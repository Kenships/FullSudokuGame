using System;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardVisual : MonoBehaviour
{
    [SerializeField]private List<Transform> subGrids;
    [SerializeField]private GameObject gridPrefab;
    [SerializeField]private SudokuSO sudoku;

    private GridUnit[,] m_gridUnits;

    private void Awake()
    {
        m_gridUnits = new GridUnit[9, 9];
    }

    private void Start()
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                Transform parentSubGrid = subGrids[row/3*3 + col/3];
                GameObject grid = Instantiate(gridPrefab, parentSubGrid);
                var gridUnit = grid.GetComponent<GridUnit>();
                int value = sudoku.GetValue(row, col);
                gridUnit.Initialize(value, value != 0);
                m_gridUnits[row, col] = gridUnit;
            }
        }
    }
}
