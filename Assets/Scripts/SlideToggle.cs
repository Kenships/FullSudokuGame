using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlideToggle : Selectable, IPointerClickHandler, ISubmitHandler, ICanvasElement
{
    [SerializeField]
    private GameObject toggleBall;

    [SerializeField]
    private Image background;

    [Tooltip("Set toggle to be off when on left side")]
    [SerializeField]
    private bool leftOff;

    [SerializeField]
    private Color onColor = Color.white;

    [SerializeField]
    private Color offColor = Color.gray;

    [SerializeField]
    private UnityEvent<bool> onValueChanged;

    private RectTransform m_toggleRectTransform;
    private RectTransform m_backgroundRectTransform;

    private float m_initialDelta;
    private bool m_isOn;

    private float m_toggleStart;
    private float m_toggleEnd;

    protected override void Start()
    {
        base.Start();
        onValueChanged.AddListener(OnToggle);
        
        m_toggleRectTransform = toggleBall.GetComponent<RectTransform>();
        m_backgroundRectTransform = background.gameObject.GetComponent<RectTransform>();

        float bgLeftAnchor = m_backgroundRectTransform.anchoredPosition.x -
                             (m_backgroundRectTransform.pivot.x * m_backgroundRectTransform.sizeDelta.x);
        Debug.Log(bgLeftAnchor);
        float bgRightAnchor = m_backgroundRectTransform.anchoredPosition.x +
                              ((1 - m_backgroundRectTransform.pivot.x) * m_backgroundRectTransform.sizeDelta.x);
        Debug.Log(bgRightAnchor);
        float toggleLeftAnchor = m_toggleRectTransform.anchoredPosition.x -
                                 (m_toggleRectTransform.pivot.x * m_toggleRectTransform.sizeDelta.x);
        Debug.Log(toggleLeftAnchor);
        float toggleRightAnchor = m_toggleRectTransform.anchoredPosition.x +
                                  ((1 - m_toggleRectTransform.pivot.x) * m_toggleRectTransform.sizeDelta.x);
        Debug.Log(toggleRightAnchor);

        if (leftOff)
        {
            float delta = toggleLeftAnchor - bgLeftAnchor;
            m_toggleStart = m_toggleRectTransform.anchoredPosition.x;
            m_toggleEnd = bgRightAnchor
                          - delta
                          - ((1 - m_toggleRectTransform.pivot.x) * m_toggleRectTransform.sizeDelta.x);
        }
        else
        {
            float delta = bgRightAnchor - toggleRightAnchor;
            Debug.Log(delta);
            m_toggleStart = m_toggleRectTransform.anchoredPosition.x;
            Debug.Log((m_backgroundRectTransform.pivot.x * m_backgroundRectTransform.sizeDelta.x));
            m_toggleEnd = bgLeftAnchor
                          + delta
                          + (m_toggleRectTransform.pivot.x * m_toggleRectTransform.sizeDelta.x);
        }
    }

    private void OnToggle(bool value)
    {
        m_toggleRectTransform.anchoredPosition = value
            ? new Vector2(m_toggleEnd, m_toggleRectTransform.anchoredPosition.y)
            : new Vector2(m_toggleStart, m_toggleRectTransform.anchoredPosition.y);
        
        background.color = value ? onColor : offColor;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
    }

    public void Rebuild(CanvasUpdate executing)
    {
#if UNITY_EDITOR
        if (executing == CanvasUpdate.Prelayout)
            onValueChanged.Invoke(m_isOn);
#endif
    }

    public void LayoutComplete()
    {
    }

    public void GraphicUpdateComplete()
    {
    }

    private void InternalToggle()
    {
        if (!IsActive() || !IsInteractable())
            return;

        m_isOn = !m_isOn;
        Debug.Log(m_isOn);
        onValueChanged.Invoke(m_isOn);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        InternalToggle();
    }

    public virtual void OnSubmit(BaseEventData eventData)
    {
        InternalToggle();
    }
}
