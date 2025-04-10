using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int id;
    public string colorHex;
    
    public SaveData(int id, Color color)
    {
        this.id = id;
        this.colorHex = ColorUtility.ToHtmlStringRGBA(color);
    }
}