using System;
using Obvious.Soap;
using UnityEngine;

public class TestSolvedVisual : MonoBehaviour
{
    [SerializeField]
    private ScriptableEventNoParam OnSolved;

    private void Start()
    {
        Hide();
        OnSolved.OnRaised += Show;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
