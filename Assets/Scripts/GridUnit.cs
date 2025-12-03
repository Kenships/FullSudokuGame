using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridUnit : MonoBehaviour
{
    private const string MSPACE = "<mspace=0.5em>";
    [Header("References")]
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Button button;
    [SerializeField] Image background;
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
    
    

    private int m_value;
    private bool[] m_notes;
    private bool m_isLocked;
    
    private bool[] m_boldNotes;

    public void Initialize(int value, bool lockNumber = false)
    {
        m_value = value;
        m_notes = new bool[9];
        m_isLocked = lockNumber;
    }

    private void Start()
    {
        UpdateVisual();
    }
    
    #region Update Values

    public void SetValue(int value)
    {
        Clear();
        m_value = value;
        UpdateVisual();
    }
    
    public void AddNote(int note)
    {
        m_notes[note - 1] = true;
        UpdateVisual();
    }

    public void RemoveNote(int note)
    {
        m_notes[note - 1] = false;
        UpdateVisual();
    }
    
    public void Clear()
    {
        m_value = 0;

        for (int i = 0; i < 9; i++)
        {
            m_notes[i] = false;
        }
        UpdateVisual();
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
        m_boldNotes[note - 1] = true;
        UpdateVisual();
    }

    public void UnboldNote(int note)
    {
        m_boldNotes[note - 1] = false;
        UpdateVisual();
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
                result += " ";
            }
        }
        
        return result;
    }
    
    #endregion
}
