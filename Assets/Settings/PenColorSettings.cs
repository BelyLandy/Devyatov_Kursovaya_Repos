using UnityEngine;

[CreateAssetMenu(fileName = "PenColorSettings", menuName = "Scriptable Objects/PenColorSettings")]
public class PenColorSettings : ScriptableObject
{
    public Color currentColor = Color.red;
}