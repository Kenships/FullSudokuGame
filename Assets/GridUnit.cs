using System;
using TMPro;
using UnityEngine;

public class GridUnit : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    private int value;
    
    private void Start()
    {
        value = 0;
        text.gameObject.SetActive(false);
    }

    public void Increment()
    {
        value++;
        text.gameObject.SetActive(true);
        text.text = value.ToString();
    }
}
