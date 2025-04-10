using UnityEngine;

public class ShapeRecognizer : MonoBehaviour
{
    [Tooltip("Изображение с формой (на прозрачном или белом фоне).")]
    public Texture2D inputImage;

    [Tooltip("Порог для определения наличия пикселя формы по альфа-каналу.")]
    public float alphaThreshold = 0.1f;

    // Пороговые значения для классификации форм (эти значения можно подстроить экспериментально)
    [Tooltip("Порог circularity для распознавания круга.")]
    public float circleThreshold = 0.85f;
    [Tooltip("Порог circularity для распознавания квадрата (прямоугольника).")]
    public float squareThreshold = 0.65f;

    /// <summary>
    /// Метод для распознавания формы в заданном изображении.
    /// При вызове выводит в консоль распознанную форму или сообщение об ошибке.
    /// </summary>
    public void RecognizeShape()
    {
        if (inputImage == null)
        {
            Debug.LogWarning("Изображение не задано.");
            return;
        }

        int width = inputImage.width;
        int height = inputImage.height;
        bool[,] mask = new bool[width, height];
        int area = 0;

        // Преобразуем изображение в бинарную маску
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixel = inputImage.GetPixel(x, y);
                bool isShape = pixel.a > alphaThreshold;
                mask[x, y] = isShape;
                if (isShape)
                    area++;
            }
        }

        if (area == 0)
        {
            Debug.Log("Форма не обнаружена.");
            return;
        }

        // Вычисляем периметр: считаем пиксели формы, у которых хотя бы один сосед – фон
        int perimeter = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (mask[x, y])
                {
                    // Если пиксель на границе изображения, сразу считаем его за край
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    {
                        perimeter++;
                    }
                    else
                    {
                        if (!mask[x - 1, y] || !mask[x + 1, y] || !mask[x, y - 1] || !mask[x, y + 1])
                            perimeter++;
                    }
                }
            }
        }

        // Вычисляем круговую меру: для идеального круга значение равно 1
        float circularity = 4 * Mathf.PI * area / (perimeter * perimeter);

        // Применяем пороговое сравнение
        string recognizedShape;
        if (circularity >= circleThreshold)
            recognizedShape = "Круг";
        else if (circularity >= squareThreshold)
            recognizedShape = "Квадрат";
        else
            recognizedShape = "Треугольник";

        Debug.Log($"Распознанная форма: {recognizedShape} (circularity = {circularity:F3})");
    }
}
