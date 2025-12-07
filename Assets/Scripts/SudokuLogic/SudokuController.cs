using System.Collections.Generic;
using DefaultNamespace;
using Obvious.Soap;
using UnityEngine;

namespace SudokuLogic
{
    public class SudokuController : MonoBehaviour
    {
        [SerializeField] private List<Transform> subGrids;
        [SerializeField] private GameObject gridPrefab;
        [SerializeField] private SudokuSO sudoku;
        [SerializeField] private ScriptableEventVector2Int onSelect;
        [SerializeField] private List<InputListenerBase> inputListeners;
        [SerializeField] private BoolVariable noteModeVar;
        [SerializeField] private ScriptableEventNoParam onSolved;


        private ClueVisual[,] m_sudokuBoard;


        private ClueVisual m_selectedClueVisual;
        private bool m_noteMode;

        private SudokuLogicCore m_logicCore;
        private SudokuNotesService<ClueVisual> m_noteService;
        private SudokuGridHighlightService m_highlighter;
    
        private void Awake()
        {
            m_logicCore = new SudokuLogicCore(new SudokuGridExtractor<SudokuLogicCore.Clue>());
            m_noteService = new SudokuNotesService<ClueVisual>(new SudokuGridExtractor<ClueVisual>());
            m_highlighter = new SudokuGridHighlightService();
        
            m_sudokuBoard = new ClueVisual[9, 9];
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    Transform parentSubGrid = subGrids[row/3*3 + col/3];
                    GameObject grid = Instantiate(gridPrefab, parentSubGrid);
                    var gridUnit = grid.GetComponent<ClueVisual>();
                    int value = sudoku.GetValue(row, col);
                    gridUnit.Initialize(new Vector2Int(row, col), value, value != 0);
                    m_sudokuBoard[row, col] = gridUnit;
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
            if (!m_selectedClueVisual)
            {
                m_selectedClueVisual = m_sudokuBoard[0, 0];
            }
        
            //Cast to int
            Vector2Int castMoveDirection = new Vector2Int(Mathf.RoundToInt(moveDirection.x), Mathf.RoundToInt(moveDirection.y));
        
            int newRow = m_selectedClueVisual.Position.x - castMoveDirection.y;
            int newCol = m_selectedClueVisual.Position.y + castMoveDirection.x;

            if (newRow >= 0 && newCol >= 0 && newRow < 9 && newCol < 9)
            {
                OnSelectRaised(new Vector2Int(newRow, newCol));
            }
        }

        private void OnDeselect()
        {
            m_highlighter.UnhighlightAndBoldAll();
        }
    
        private void OnEraseInput()
        {
            if (!m_selectedClueVisual)
                return;

            m_selectedClueVisual.Erase();
        }

        private void OnModeChangeRaised(bool noteMode)
        {
            m_noteMode = noteMode;
        }

        private void OnValueInputRaised(int value)
        {
            if (!m_selectedClueVisual)
                return;

            if (m_noteMode)
            {
                m_selectedClueVisual.ToggleNote(value);
            }
            else
            {
            
                m_selectedClueVisual.SetValue(value);
            
                List<ClueVisual> cluesToUpdate = m_noteService.RemoveNotesWithValue(m_selectedClueVisual, m_sudokuBoard);
                UpdateClueVisuals(cluesToUpdate);
            
                OnSelectRaised(new Vector2Int(m_selectedClueVisual.Position.x, m_selectedClueVisual.Position.y));
                if (m_logicCore.EvaluateSudoku(m_sudokuBoard) == SudokuLogicCore.EvaluationState.Solved)
                {
                    onSolved.Raise();
                }
            }
        }

        private void OnSelectRaised(Vector2Int position)
        {
            m_selectedClueVisual = m_sudokuBoard[position.x, position.y];
        
            m_highlighter.UnhighlightAndBoldAll();
            m_highlighter.HighlightAndBoldSelectRegion(m_selectedClueVisual, m_sudokuBoard);
        }

        private void UpdateClueVisuals(List<ClueVisual> clueVisuals)
        {
            foreach (var clueVisual in clueVisuals)
            {
                clueVisual.UpdateVisual();
            }
        }
    }
}
