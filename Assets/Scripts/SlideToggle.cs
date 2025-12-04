using System;
using PrimeTween;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SlideToggle : Selectable, IPointerClickHandler, ISubmitHandler, ICanvasElement
{
    #region Serialized Fields
    [SerializeField]
    private bool m_isOn;

    [SerializeField]
    private GameObject toggleBall;

    [SerializeField]
    private Graphic background;

    [SerializeField]
    private Color onColor = Color.white;

    [SerializeField]
    private Color offColor = Color.gray;

    [SerializeField]
    private float animationTime = 0.3f;

    [SerializeField]
    private UnityEvent<bool> onValueChanged;
    #endregion
    
    #region Public Properties
    public bool IsOn
    {
        get => m_isOn;
        set => SetIsOn(value);
    }

    private void SetIsOn(bool value, bool sendCallback = true)
    {
        if (m_isOn == value)
            return;

        m_isOn = value;

        PlayEffect(false);

        if (sendCallback)
        {
            UISystemProfilerApi.AddMarker("Toggle.value", this);
            onValueChanged.Invoke(m_isOn);
        }
    }
    #endregion
    
    #region private Properties
    private RectTransform m_toggleRectTransform;
    private RectTransform m_backgroundRectTransform;

    private float m_initialDelta;

    private float m_toggleStart;
    private float m_toggleEnd;
    #endregion
    
    #region MonoBehaviour Functions
    protected override void OnEnable()
    {
        base.OnEnable();
        m_toggleRectTransform = toggleBall.GetComponent<RectTransform>();
        m_backgroundRectTransform = background.gameObject.GetComponent<RectTransform>();
        SetAnchors();
        if (m_isOn)
        {
            //Swap start and end positions
            (m_toggleStart, m_toggleEnd) = (m_toggleEnd, m_toggleStart);
        }
    }
    protected override void Start()
    {
        PlayEffect(true);
    }
    #endregion
    
    #region Animation Control Methods
    protected override void OnDidApplyAnimationProperties()
    {
        // Check if isOn has been changed by the animation.
        // Unfortunately there is no way to check if we don�t have a graphic.
        if (toggleBall)
        {
            bool oldValue = !Mathf.Approximately(m_toggleRectTransform.anchoredPosition.x, m_toggleEnd);
            if (m_isOn != oldValue)
            {
                m_isOn = oldValue;
                SetIsOn(!oldValue);
            }
        }

        base.OnDidApplyAnimationProperties();
    }

    private void SetAnchors()
    {
        float bgLeftAnchor = m_backgroundRectTransform.anchoredPosition.x -
                             (m_backgroundRectTransform.pivot.x * m_backgroundRectTransform.sizeDelta.x);
        float bgRightAnchor = m_backgroundRectTransform.anchoredPosition.x +
                              ((1 - m_backgroundRectTransform.pivot.x) * m_backgroundRectTransform.sizeDelta.x);
        float toggleLeftAnchor = m_toggleRectTransform.anchoredPosition.x -
                                 (m_toggleRectTransform.pivot.x * m_toggleRectTransform.sizeDelta.x);

        float delta = toggleLeftAnchor - bgLeftAnchor;
        m_toggleStart = m_toggleRectTransform.anchoredPosition.x;

        m_toggleEnd = bgRightAnchor - delta
                                    - ((1 - m_toggleRectTransform.pivot.x) * m_toggleRectTransform.sizeDelta.x);
    }

    private void PlayEffect(bool instant)
    {
#if UNITY_EDITOR
        if (Tween.GetTweensCount(m_toggleRectTransform) == 0 &&
            !Mathf.Approximately(m_toggleRectTransform.anchoredPosition.x, m_toggleStart) &&
            !Mathf.Approximately(m_toggleRectTransform.anchoredPosition.x, m_toggleEnd))
            SetAnchors();
#endif
        
        Tween.StopAll();

        if (instant)
        {
            m_toggleRectTransform.anchoredPosition = m_isOn
                ? new Vector2(m_toggleEnd, m_toggleRectTransform.anchoredPosition.y)
                : new Vector2(m_toggleStart, m_toggleRectTransform.anchoredPosition.y);
        
            background.color = m_isOn ? onColor : offColor;
            return;
        }
        
        Tween.UIAnchoredPositionX(
            target: m_toggleRectTransform,
            endValue: m_isOn ? m_toggleEnd : m_toggleStart,
            ease: Ease.InOutExpo,
            duration: animationTime,
            cycles: 1
        ).Group(
            Tween.Color(
                target: background,
                endValue: m_isOn ? onColor : offColor,
                duration: animationTime,
                ease: Ease.InOutExpo,
                cycles: 1
            ));
    }
    #endregion
    
    #region Editor Only
#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
    }
#endif

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
    #endregion
    
    #region Input Methods
    private void InternalToggle()
    {
        if (!IsActive() || !IsInteractable())
            return;

        IsOn = !IsOn;
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
    #endregion
}
