using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ZoneManager))]
public class ZoneManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ZoneManager zoneManager = (ZoneManager)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Сгенерировать Новую Зону"))
        {
            zoneManager.AddZone();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Список созданных зон:", EditorStyles.boldLabel);

        if (zoneManager.zones != null)
        {
            for (int i = 0; i < zoneManager.zones.Count; i++)
            {
                EditorGUILayout.ObjectField("Zone " + i, zoneManager.zones[i], typeof(GameObject), true);
            }
        }
    }
}