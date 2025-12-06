using System;
using System.Collections.Generic;
using Obvious.Soap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridUnit : MonoBehaviour
{
    #region Constants
    private const string MSPACE = "<mspace=0.5em>";
    #endregion
    
    #region Serialized Fields
    [Header("References")]
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image background;
    [SerializeField] private ScriptableEventVector2Int onSelect;
    [Space]
    [Header("Font Scale")]
    [SerializeField] private int valueSize;
    [SerializeField] private int noteSize;
    [Space]
    [Header("Grid Colors")]
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color selectColor;
    [SerializeField] private Color defaultFontColor;
    [SerializeField] private Color lockedFontColor;
    #endregion
    public Vector2Int Position { get; private set; }

    private int m_value;
    private bool[] m_notes;
    private bool m_isLocked;
    
    private bool[] m_boldNotes;

    public void Initialize(Vector2Int position, int value, bool lockNumber = false)
    {
        Position = position;
        m_value = value;
        m_notes = new bool[9];
        m_boldNotes = new bool[9];
        m_isLocked = lockNumber;
    }

    private void Start()
    {
        UpdateVisual();
    }
    
    #region Update Values

    public void SetValue(int value, bool force = false)
    {
        if(m_isLocked && !force)
            return;
        
        for (int i = 0; i < 9; i++)
        {
            m_notes[i] = false;
        }
        
        m_value = value;
        UpdateVisual();
    }

    public void ToggleNote(int note, bool force = false)
    {
        if(m_isLocked && !force)
            return;
        
        m_value = 0;
        m_notes[note - 1] = !m_notes[note - 1];
        UpdateVisual();
    }
    
    public void AddNote(int note, bool force = false)
    {
        if(m_isLocked && !force)
            return;
        
        m_value = 0;
        m_notes[note - 1] = true;
        UpdateVisual();
    }

    public void RemoveNote(int note, bool force = false)
    {
        if(m_isLocked && !force)
            return;
        
        m_notes[note - 1] = false;
        UpdateVisual();
    }
    
    public void Erase(bool force = false)
    {
        SetValue(0, force);
    }

    public void Lock()
    {
        m_isLocked = true;
        UpdateVisual();
    }

    public void Unlock()
    {
        m_isLocked = false;
        UpdateVisual();
    }
    #endregion

    #region UpdateVisual
    private void UpdateVisual()
    {
        if (m_value == 0 && IsNotesEmpty())
        {
            text.gameObject.SetActive(false);
            return;
        }
        if (m_value != 0)
        {
            text.fontSize = valueSize;
            text.text = m_value.ToString();
        }
        else
        {
            text.fontSize = noteSize;
            text.text = NoteString();
        }
        text.color = m_isLocked ? lockedFontColor : defaultFontColor;
        text.gameObject.SetActive(true);
    }

    public void Select()
    {
        onSelect.Raise(Position);
    }

    public void Highlight(bool selected = false)
    {
        background.color = selected ? selectColor : highlightColor;
        UpdateVisual();
    }

    public void Unhighlight()
    {
        background.color = defaultColor;
        UpdateVisual();
    }

    public void BoldNote(int note)
    {
        if (!m_notes[note - 1] || m_value != 0) return;
        
        m_boldNotes[note - 1] = true;
        UpdateVisual();
    }

    public void UnboldNote(int note, bool update = true)
    {
        if (!m_boldNotes[note - 1]) return;
        
        m_boldNotes[note - 1] = false;
        
        if (update)
            UpdateVisual();
    }

    public void UnboldAll()
    {
        for (int r = 1; r <= 9; r++)
        {
            UnboldNote(r, false);
        }
    }

    #endregion
    
    #region Util

    private bool IsNotesEmpty()
    {
        for (int i = 0; i < 9; i++)
        {
            if (m_notes[i]) return false;
        }
        
        return true;
    }
    
    private string NoteString()
    {
        string result = MSPACE;
        for (int i = 0; i < 9; i++)
        {
            if (m_notes[i])
            {
                result += m_boldNotes[i] ? "<b>" + (i + 1): i + 1;
            }
            else
            {
                result += "+";
            }
        }
        
        return result;
    }
    public bool[] GetNotes()
    {
        return m_notes;
    }

    public int GetValue()
    {
        return m_value;
    }
    #endregion
}
