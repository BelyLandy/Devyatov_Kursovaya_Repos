using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class ZoneObject : MonoBehaviour
{
    [Tooltip("Список имен жестов, которые активируют зону")]
    public List<RecognizedShape> requiredGestureNames = new List<RecognizedShape>();

    [Tooltip("Метод, который вызывается при срабатывании зоны")]
    public UnityEvent onZoneAction;

    /// <summary>
    /// Проверяет, находится ли мировая точка внутри зоны
    /// </summary>
    public bool IsPointInside(Vector2 worldPoint)
    {
        Collider2D col = GetComponent<Collider2D>();
        return col.OverlapPoint(worldPoint);
    }
}