using System;
using System.Collections.Generic;
using DefaultNamespace;
using Obvious.Soap;
using PrimeTween;
using SudokuLogic.Interface;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClueVisual : MonoBehaviour, ISudokuCell
{
    #region Constants

    private const string MSPACE = "<mspace=0.5em>";

    #endregion

    #region Serialized Fields

    [Header("References")]
    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    Image background;

    [SerializeField]
    GameObject animatedBackground;

    [SerializeField]
    private ScriptableEventVector2Int onSelect;

    [Space]
    [Header("Font Scale")]
    [SerializeField]
    private int valueSize;

    [SerializeField]
    private int noteSize;

    [Space]
    [Header("Grid Colors")]
    [SerializeField]
    private Color defaultColor;

    [SerializeField]
    private Color highlightColor;

    [SerializeField]
    private Color selectColor;

    [SerializeField]
    private Color defaultFontColor;

    [SerializeField]
    private Color lockedFontColor;

    #endregion

    public Vector2Int Position { get; private set; }

    public int Value { get; set; }
    public bool[] Notes { get; private set; }

    private bool m_isLocked;

    private bool[] m_boldNotes;

    private RectTransform animatedBackgroundRect;
    private Image animatedBackgroundImage;

    public void Initialize(Vector2Int position, int value, bool lockNumber = false)
    {
        Position = position;
        Value = value;
        Notes = new bool[9];
        m_boldNotes = new bool[9];
        m_isLocked = lockNumber;

        animatedBackgroundRect = animatedBackground.GetComponent<RectTransform>();
        animatedBackgroundImage = animatedBackground.GetComponent<Image>();
    }

    private void Start()
    {
        animatedBackground.gameObject.SetActive(false);
        UpdateVisual();
    }

    #region Update Values

    public void SetValue(int value, bool force = false)
    {
        if (m_isLocked && !force)
            return;

        for (int i = 0; i < 9; i++)
        {
            Notes[i] = false;
        }

        Value = value;
        UpdateVisual();
    }

    public void ToggleNote(int note, bool force = false)
    {
        if (m_isLocked && !force)
            return;

        Value = 0;
        Notes[note - 1] = !Notes[note - 1];
        UpdateVisual();
    }

    public void AddNote(int note, bool force = false)
    {
        if (m_isLocked && !force)
            return;

        Value = 0;
        Notes[note - 1] = true;
        UpdateVisual();
    }

    public void RemoveNote(int note, bool force = false)
    {
        if (m_isLocked && !force)
            return;

        Notes[note - 1] = false;
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

    public void UpdateVisual()
    {
        if (Value == 0 && IsNotesEmpty())
        {
            text.gameObject.SetActive(false);
            return;
        }

        if (Value != 0)
        {
            text.fontSize = valueSize;
            text.text = Value.ToString();
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
        if (!Notes[note - 1] || Value != 0)
            return;

        m_boldNotes[note - 1] = true;
        UpdateVisual();
    }

    public void UnboldNote(int note, bool update = true)
    {
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

        UpdateVisual();
    }

    #endregion

    #region Animation

    public void Animate()
    {
        animatedBackground.gameObject.SetActive(true);
        Tween.UISizeDelta(
                target: animatedBackgroundRect,
                startValue: new Vector2(20, 20),
                endValue: new Vector2(90, 90),
                duration: 0.5f,
                ease: Ease.OutExpo,
                cycles: 1,
                cycleMode: CycleMode.Rewind)
            .Group(
                Tween.Alpha(target: animatedBackgroundImage,
                    startValue: 0f,
                    endValue: 255f,
                    duration: 0.5f,
                    ease: Ease.OutExpo,
                    cycles: 1,
                    cycleMode: CycleMode.Rewind)
            )
            .Chain(
                Tween.LocalRotation(
                        target: animatedBackgroundRect,
                        startValue: Vector3.zero,
                        endValue: new Vector3(0, 0, 20),
                        duration: 0.3f,
                        ease: Ease.OutExpo,
                        cycles: 2,
                        cycleMode: CycleMode.Rewind
                    )
            ).Group(
                Tween.UISizeDelta(
                    target: animatedBackgroundRect,
                    startValue: new Vector2(89, 89),
                    endValue: new Vector2(20, 20),
                    duration: 0.3f,
                    ease: Ease.OutExpo,
                    cycles: 1,
                    cycleMode: CycleMode.Rewind)
            ).Group(
                Tween.Alpha(target: animatedBackgroundImage,
                    startValue: 255f,
                    endValue: 0f,
                    duration: 0.2f,
                    ease: Ease.OutExpo,
                    cycles: 1,
                    cycleMode: CycleMode.Rewind)
                ).OnComplete(() => animatedBackground.SetActive(false));
    }

    #endregion

    #region Util

    private bool IsNotesEmpty()
    {
        for (int i = 0; i < 9; i++)
        {
            if (Notes[i])
                return false;
        }

        return true;
    }

    private string NoteString()
    {
        string result = MSPACE;
        for (int i = 0; i < 9; i++)
        {
            if (Notes[i])
            {
                result += m_boldNotes[i] ? "<b>" + (i + 1) + "</b>" : i + 1;
            }
            else
            {
                result += "+";
            }
        }

        return result;
    }

    public bool HasNote(int note)
    {
        return Notes[note - 1];
    }

    public bool IsEmpty()
    {
        return IsNotesEmpty() && Value == 0;
    }

    public override string ToString()
    {
        return Position + "Value: " + Value + " Notes: " + NoteString();
    }

    #endregion
}
