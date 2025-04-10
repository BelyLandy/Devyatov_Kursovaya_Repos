using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance { get; private set; }

    // Список всех созданных зон
    public List<GameObject> zones = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary> 
    /// Создает новую зону с нужными компонентами:
    /// SpriteRenderer, BoxCollider2D и ZoneObject.
    /// Объект создаётся в позиции (0,0,0).
    /// </summary>
    public void AddZone()
    {
        // Создаём новый GameObject и задаём имя
        GameObject newZone = new GameObject("Zone_" + zones.Count);
        newZone.transform.position = new Vector3(0, 0, -2);

        // Добавляем компонент SpriteRenderer для визуального отображения зоны
        SpriteRenderer sr = newZone.AddComponent<SpriteRenderer>();
        sr.color = new Color(1f, 1f, 1f, 0.5f); // Полупрозрачный белый цвет

        // Добавляем BoxCollider2D для определения границ зоны
        BoxCollider2D box = newZone.AddComponent<BoxCollider2D>();
        box.size = new Vector2(1f, 1f); // Размер коллайдера

        // Добавляем скрипт ZoneObject для хранения настроек зоны
        ZoneObject zoneObject = newZone.AddComponent<ZoneObject>();

        // Создаём пустой список жестов (можно редактировать в инспекторе)
        zoneObject.requiredGestureNames = new List<RecognizedShape>();

        // Добавляем новый объект в список зон
        zones.Add(newZone);
    }


    /// <summary>
    /// Проверяет, попадает ли мировая точка (например, центр распознанного жеста)
    /// в область какой-либо зоны и, если имя распознанного жеста совпадает с требуемым,
    /// вызывает привязанный UnityEvent.
    /// </summary>
    public void CheckZones(Vector2 worldPoint, string recognizedGesture)
    {
        // Пробуем преобразовать строку в RecognizedShape
        if (!Enum.TryParse(recognizedGesture, out RecognizedShape recognizedShape))
        {
            Debug.LogWarning($"Не удалось распознать жест: {recognizedGesture}");
            return; // Выходим, если не удалось преобразовать
        }

        foreach (GameObject zoneObj in zones)
        {
            ZoneObject zone = zoneObj.GetComponent<ZoneObject>();
            if (zone != null && zone.IsPointInside(worldPoint))
            {
                if (zone.requiredGestureNames.Contains(recognizedShape)) // Теперь тут правильный тип
                {
                    zone.onZoneAction.Invoke();
                    Debug.Log($"Жест {recognizedGesture} ({recognizedShape}) активировал зону {zoneObj.name}");
                    SoundManager.Instance.PlaySound2D("FastWritingSound");
                }
            }
        }
    }


}
