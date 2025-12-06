
using System;
using System.Collections.Generic;
using DefaultNamespace;
using Obvious.Soap;
using SudokuLogic;
using UnityEngine;

public class GameBoardController : MonoBehaviour
{
    [SerializeField] private List<Transform> subGrids;
    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private SudokuSO sudoku;
    [SerializeField] private ScriptableEventVector2Int onSelect;
    [SerializeField] private List<InputListenerBase> inputListeners;
    [SerializeField] private BoolVariable noteModeVar;
    [SerializeField] private ScriptableEventNoParam onSolved;


    private GridUnit[,] m_gridUnits;
    private List<GridUnit> m_highlightedGridUnits; 

    private GridUnit m_selectedGridUnit;
    private bool m_noteMode;

    private SudokuLogicCore sudokuLogicCore;
    
    private void Awake()
    {
        sudokuLogicCore = new SudokuLogicCore();
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
        noteModeVar.OnValueChanged += OnModeChangeRaised;
        
        if (inputListeners is { Count: > 0 })
        {
            foreach (InputListenerBase listener in inputListeners)
            {
                listener.OnValueInput += OnValueInputRaised;
                listener.OnEraseInput += OnEraseInput;
                listener.OnMoveInput += OnMoveInput;
                listener.OnSelectInput += OnSelectRaised;
                listener.OnDeselectInput += OnDeselect;
                listener.OnEscapeInput += OnDeselect;
            }
        }
    }

    private void OnDestroy()
    {
        noteModeVar.OnValueChanged -= OnModeChangeRaised;

        if (inputListeners is { Count: > 0 })
        {
            foreach (InputListenerBase listener in inputListeners)
            {
                listener.OnValueInput -= OnValueInputRaised;
                listener.OnEraseInput -= OnEraseInput;
                listener.OnMoveInput -= OnMoveInput;
                listener.OnSelectInput -= OnSelectRaised;
                listener.OnDeselectInput -= OnDeselect;
                listener.OnEscapeInput -= OnDeselect;
            }
        }
    }

    private void OnMoveInput(Vector2 moveDirection)
    {
        if (!m_selectedGridUnit)
        {
            m_selectedGridUnit = m_gridUnits[0, 0];
        }
        
        //Cast to int
        Vector2Int castMoveDirection = new Vector2Int(Mathf.RoundToInt(moveDirection.x), Mathf.RoundToInt(moveDirection.y));
        
        int newRow = m_selectedGridUnit.Position.x - castMoveDirection.y;
        int newCol = m_selectedGridUnit.Position.y + castMoveDirection.x;

        if (newRow >= 0 && newCol >= 0 && newRow < 9 && newCol < 9)
        {
            OnSelectRaised(new Vector2Int(newRow, newCol));
        }
    }

    private void OnDeselect()
    {
        DeselectAll();
    }
    
    private void OnEraseInput()
    {
        if (!m_selectedGridUnit)
            return;

        m_selectedGridUnit.Erase();
    }

    private void OnModeChangeRaised(bool noteMode)
    {
        m_noteMode = noteMode;
    }

    private void OnValueInputRaised(int value)
    {
        if (!m_selectedGridUnit)
            return;

        if (m_noteMode)
        {
            m_selectedGridUnit.ToggleNote(value);
        }
        else
        {
            m_selectedGridUnit.SetValue(value);
            OnSelectRaised(new Vector2Int(m_selectedGridUnit.Position.x, m_selectedGridUnit.Position.y));
            if (sudokuLogicCore.EvaluateSudoku(m_gridUnits) == SudokuLogicCore.EvaluationState.Solved)
            {
                onSolved.Raise();
            }
        }
    }

    private void OnSelectRaised(Vector2Int position)
    {
        m_selectedGridUnit = m_gridUnits[position.x, position.y];
        int selectedValue = m_selectedGridUnit.GetValue();
        DeselectAll();

        int boxRow = position.x / 3 * 3;
        int boxCol = position.y / 3 * 3;

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (position.x == row || position.y == col)
                {
                    m_highlightedGridUnits.Add(m_gridUnits[row, col]);
                }
                
                if (row >= boxRow && col >= boxCol && row < boxRow + 3 && col < boxCol + 3)
                {
                    m_highlightedGridUnits.Add(m_gridUnits[row, col]);
                }
                
                if (selectedValue == 0)
                {
                    continue;
                }
                
                if (m_gridUnits[row, col].GetValue() == m_selectedGridUnit.GetValue())
                {
                    m_highlightedGridUnits.Add(m_gridUnits[row, col]);
                }
                
                if (m_gridUnits[row, col].HasNote(m_selectedGridUnit.GetValue()))
                {
                    m_highlightedGridUnits.Add(m_gridUnits[row, col]);
                }
            }
        }

        foreach (GridUnit gridUnit in m_highlightedGridUnits)
        {
            int value = gridUnit.GetValue();
            if (selectedValue != 0 && value == 0 && !gridUnit.IsEmpty())
            {
                gridUnit.BoldNote(selectedValue);
                continue;
            }

            bool selected = (gridUnit == m_selectedGridUnit) || (value != 0 && value == selectedValue);
            
            gridUnit.Highlight(selected);
        }
    }

    private void DeselectAll()
    {
        foreach (var gridUnit in m_highlightedGridUnits)
        {
            gridUnit.UnboldAll();
            gridUnit.Unhighlight();
        }
        
        m_highlightedGridUnits.Clear();
    }
}
