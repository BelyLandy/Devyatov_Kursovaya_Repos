using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TwoStringToggler : MonoBehaviour
{
    [Tooltip("Компонент TextMeshPro, в котором будем менять текст")]
    public TMP_Text textComponent;

    [Tooltip("Список из двух строк; обмен происходит по кругу")]
    public string text;
    
    private bool useSecondNext = true;

    [ContextMenu("ToggleText")]
    public void ToggleText()
    {
        if (textComponent == null)
        {
            Debug.LogError("Не задан TextMeshPro компонент!");
            return;
        }

        textComponent.text = text;
        
        useSecondNext = !useSecondNext;
    }
}