using UnityEngine;

public class ShadowGenerator : MonoBehaviour
{
    [Header("Настройки тени")]
    [Tooltip("Цвет тени (например, чёрный с прозрачностью)")]
    public Color shadowColor = new Color(0, 0, 0, 0.5f);
    
    [Tooltip("Вертикальный коэффициент сжатия тени (0.5 = высота вдвое меньше)")]
    [Range(0.1f, 1f)]
    public float verticalCompression = 0.5f;
    
    [Tooltip("Смещение тени относительно основного спрайта (общий сдвиг)")]
    public Vector2 shadowOffset = new Vector2(0.1f, -0.1f);

    private SpriteRenderer mainSpriteRenderer;
    private GameObject shadowObject;
    private SpriteRenderer shadowSpriteRenderer;

    void Start()
    {
        // Получаем основной SpriteRenderer
        mainSpriteRenderer = GetComponent<SpriteRenderer>();
        if (mainSpriteRenderer == null)
        {
            Debug.LogError("ShadowGenerator: Компонент SpriteRenderer не найден на объекте.");
            return;
        }
        if (mainSpriteRenderer.sprite == null)
        {
            Debug.LogError("ShadowGenerator: У SpriteRenderer не задан спрайт.");
            return;
        }
        
        // Создаём дочерний объект для тени и задаём смещение
        shadowObject = new GameObject("Shadow");
        shadowObject.transform.parent = transform;
        shadowObject.transform.localPosition = shadowOffset;
        shadowObject.transform.localRotation = Quaternion.identity;
        shadowObject.transform.localScale = Vector3.one;
        
        // Добавляем компонент SpriteRenderer для тени
        shadowSpriteRenderer = shadowObject.AddComponent<SpriteRenderer>();
        shadowSpriteRenderer.sortingLayerID = mainSpriteRenderer.sortingLayerID;
        // Отрисовка тени под основным спрайтом
        shadowSpriteRenderer.sortingOrder = mainSpriteRenderer.sortingOrder - 1;
    }

    void FixedUpdate()
    {
        // Обновляем тень каждый FixedUpdate
        Sprite newShadowSprite = GenerateStaircaseShadowSprite(mainSpriteRenderer.sprite, verticalCompression);
        if(newShadowSprite != null)
        {
            // Чтобы избежать утечек памяти, можно удалять предыдущую текстуру (если необходимо)
            if(shadowSpriteRenderer.sprite != null)
            {
                Destroy(shadowSpriteRenderer.sprite.texture);
            }
            shadowSpriteRenderer.sprite = newShadowSprite;
        }
    }

    /// <summary>
    /// Генерирует новый спрайт-тень из исходного спрайта, сжимая его по вертикали и создавая эффект «лесенки».
    /// Каждый ряд в сжатой текстуре смещается вправо на количество пикселей, равное индексу ряда.
    /// </summary>
    /// <param name="original">Исходный спрайт</param>
    /// <param name="verticalCompression">Коэффициент сжатия по вертикали (например, 0.5 означает, что высота уменьшится вдвое)</param>
    /// <returns>Новый спрайт-тень с эффектом «лесенки»</returns>
    Sprite GenerateStaircaseShadowSprite(Sprite original, float verticalCompression)
    {
        // Извлекаем область спрайта из текстуры
        Rect origRect = original.rect;
        int width = (int)origRect.width;
        int origHeight = (int)origRect.height;
        int newHeight = Mathf.Max(1, Mathf.RoundToInt(origHeight * verticalCompression));
        // Новая ширина увеличена на newHeight, чтобы учесть сдвиги для каждого ряда
        int newWidth = width + newHeight;

        Texture2D origTex = original.texture;
        // Текстура должна быть помечена как Read/Write Enabled в настройках импорта!
        if (!origTex.isReadable)
        {
            Debug.LogError("ShadowGenerator: Текстура должна быть отмечена как Read/Write Enabled.");
            return null;
        }

        // Получаем пиксели из области спрайта (если спрайт — часть текстуры)
        Color[] originalPixels = origTex.GetPixels((int)origRect.x, (int)origRect.y, width, origHeight);
        Color[] shadowPixels = new Color[newWidth * newHeight];

        // Заполняем всю новую текстуру прозрачным цветом
        for (int i = 0; i < shadowPixels.Length; i++)
            shadowPixels[i] = new Color(0, 0, 0, 0);

        // Для каждого ряда новой текстуры
        for (int y = 0; y < newHeight; y++)
        {
            // Вычисляем соответствующую строку в исходном спрайте (с использованием nearest neighbor)
            int origY = Mathf.FloorToInt(y / verticalCompression);
            origY = Mathf.Clamp(origY, 0, origHeight - 1);
            int rowOffset = y; // смещение: каждая строка сдвигается вправо на y пикселей
            for (int x = 0; x < width; x++)
            {
                int origIndex = origY * width + x;
                Color origColor = originalPixels[origIndex];
                // Если пиксель непрозрачный, задаём shadowColor; иначе делаем прозрачным
                Color newColor = (origColor.a > 0.1f) ? shadowColor : new Color(0, 0, 0, 0);
                // Вычисляем целевую x-координату с учётом смещения ряда
                int targetX = x + rowOffset;
                if (targetX >= newWidth)
                    continue; // если выходит за границы, пропускаем
                int targetIndex = y * newWidth + targetX;
                shadowPixels[targetIndex] = newColor;
            }
        }

        // Создаем новую текстуру с размерами newWidth x newHeight
        Texture2D shadowTex = new Texture2D(newWidth, newHeight, origTex.format, false);
        shadowTex.SetPixels(shadowPixels);
        // Режим фильтрации Point сохраняет четкость пикселей
        shadowTex.filterMode = FilterMode.Point;
        shadowTex.Apply();

        // Рассчитываем новый pivot: можно сжать pivot по вертикали аналогично текстуре
        // Здесь сдвигаем pivot по горизонтали на половину максимального смещения, чтобы центрировать тень
        Vector2 newPivot = new Vector2((original.pivot.x + newHeight / 2f) / newWidth,
                                       (original.pivot.y * verticalCompression) / newHeight);
        return Sprite.Create(shadowTex, new Rect(0, 0, newWidth, newHeight), newPivot, original.pixelsPerUnit);
    }
}
