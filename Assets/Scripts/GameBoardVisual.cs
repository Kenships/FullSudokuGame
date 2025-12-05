using System;
using System.Collections.Generic;
using Obvious.Soap;
using UnityEngine;

public class GameBoardVisual : MonoBehaviour
{
    [SerializeField]private List<Transform> subGrids;
    [SerializeField]private GameObject gridPrefab;
    [SerializeField]private SudokuSO sudoku;
    [SerializeField]private ScriptableEventVector2Int onSelect;
    [SerializeField]private ScriptableEventInt onValueInput;
    [SerializeField]private ScriptableEventBool onModeChange;
    [SerializeField]private ScriptableEventNoParam onDelete;
    [SerializeField]private ScriptableEventNoParam onDeselect;

    private GridUnit[,] m_gridUnits;
    private List<GridUnit> m_highlightedGridUnits; 

    private GridUnit m_selectedGridUnit;
    private bool m_noteMode;
    
    private void Awake()
    {
        m_gridUnits = new GridUnit[9, 9];
        m_highlightedGridUnits = new List<GridUnit>();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                Transform parentSubGrid = subGrids[row/3*3 + col/3];
                GameObject grid = Instantiate(gridPrefab, parentSubGrid);
                var gridUnit = grid.GetComponent<GridUnit>();
                int value = sudoku.GetValue(row, col);
                gridUnit.Initialize(new Vector2Int(row, col), value, value != 0);
                m_gridUnits[row, col] = gridUnit;
            }
        }
    }

    private void Start()
    {
        onSelect.OnRaised += OnSelectRaised;
        onValueInput.OnRaised += OnValueInputRaised;
        onModeChange.OnRaised += OnModeChangeRaised;
    }

    private void OnModeChangeRaised(bool noteMode)
    {
        m_noteMode = noteMode;
    }

    private void OnValueInputRaised(int value)
    {
        if(!m_selectedGridUnit)
            return;

        if (m_noteMode)
        {
            m_selectedGridUnit.ToggleNote(value);
        }
        else
        {
            m_selectedGridUnit.SetValue(value);
        }
    }

    private void OnSelectRaised(Vector2Int position)
    {
        DeselectAll();
        for (int row = 0; row < 9; row++)
        {
            m_highlightedGridUnits.Add(m_gridUnits[row, position.y]);
        }
        for (int col = 0; col < 9; col++)
        {
            m_highlightedGridUnits.Add(m_gridUnits[position.x, col]);
        }

        int boxRow = position.x / 3 * 3;
        int boxCol = position.y / 3 * 3;
        for (int row = boxRow; row < boxRow + 3; row++)
        {
            for (int col = boxCol; col < boxCol + 3; col++)
            {
                m_highlightedGridUnits.Add(m_gridUnits[row, col]);
            }
        }

        foreach (GridUnit gridUnit in m_highlightedGridUnits)
        {
            gridUnit.Highlight();
        }
        
        m_selectedGridUnit = m_gridUnits[position.x, position.y];
        m_selectedGridUnit.Highlight(true);
    }

    private void DeselectAll()
    {
        foreach (var gridUnit in m_highlightedGridUnits)
        {
            gridUnit.Unhighlight();
        }
        
        m_highlightedGridUnits.Clear();
    }
}
