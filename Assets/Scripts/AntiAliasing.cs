using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AntiAliasing : MonoBehaviour
{
    [Header("Settings")]
    public Texture2D inputTexture;  // Загружаемая текстура через инспектор
    public int scale = 2;           // Масштаб уменьшения (должен быть степенью 2, например 2, 4, 8 и т.д.)
    public bool brighten = false;   // Увеличение яркости после уменьшения

    [Header("Output")]
    public RawImage outputImage;    // RawImage для отображения результата

    void Start()
    {
        if (inputTexture != null)
        {
            // Получаем пиксели текстуры
            Color32[] pixels = inputTexture.GetPixels32();
            int width = inputTexture.width;
            int height = inputTexture.height;

            // Преобразуем пиксели в двумерный массив
            int[][] original = new int[width][];
            for (int x = 0; x < width; x++)
            {
                original[x] = new int[height];
                for (int y = 0; y < height; y++)
                {
                    Color32 pixel = pixels[x + y * width];
                    original[x][y] = (pixel.r << 16) | (pixel.g << 8) | pixel.b;
                }
            }

            // Применяем билинейное уменьшение
            int[][] downscaled = ImageProcessor.BilinearDownscale(original, scale, brighten);

            // Преобразуем результат обратно в текстуру
            Texture2D newTexture = new Texture2D(downscaled.Length, downscaled[0].Length);
            for (int x = 0; x < downscaled.Length; x++)
            {
                for (int y = 0; y < downscaled[0].Length; y++)
                {
                    int colorValue = downscaled[x][y];
                    byte r = (byte)((colorValue >> 16) & 0xFF);
                    byte g = (byte)((colorValue >> 8) & 0xFF);
                    byte b = (byte)(colorValue & 0xFF);
                    newTexture.SetPixel(x, y, new Color32(r, g, b, 255));
                }
            }
            newTexture.Apply();

            // Применяем результат к RawImage для отображения
            outputImage.texture = newTexture;
        }
    }
}

public static class ImageProcessor
{
    public static int[][] BilinearDownscale(int[][] original, int scale, bool brighten)
    {
        int[][] result = new int[original.Length / 2][];
        for (int i = 0; i < result.Length; i++)
            result[i] = new int[original[0].Length / 2];

        int[] a = new int[3];
        int[] b = new int[3];
        int[] c = new int[3];
        int[] d = new int[3];

        for (int x = 0; x < result.Length; x++)
        {
            for (int y = 0; y < result[0].Length; y++)
            {
                // Получаем цветовые компоненты пикселей
                a[0] = (original[x * 2][y * 2] >> 16) & 0xFF;
                b[0] = (original[x * 2 + 1][y * 2] >> 16) & 0xFF;
                c[0] = (original[x * 2][y * 2 + 1] >> 16) & 0xFF;
                d[0] = (original[x * 2 + 1][y * 2 + 1] >> 16) & 0xFF;

                a[1] = (original[x * 2][y * 2] >> 8) & 0xFF;
                b[1] = (original[x * 2 + 1][y * 2] >> 8) & 0xFF;
                c[1] = (original[x * 2][y * 2 + 1] >> 8) & 0xFF;
                d[1] = (original[x * 2 + 1][y * 2 + 1] >> 8) & 0xFF;

                a[2] = original[x * 2][y * 2] & 0xFF;
                b[2] = original[x * 2 + 1][y * 2] & 0xFF;
                c[2] = original[x * 2][y * 2 + 1] & 0xFF;
                d[2] = original[x * 2 + 1][y * 2 + 1] & 0xFF;

                // Интерполяция
                int red = (int)(0.25f * (a[0] + b[0] + c[0] + d[0]));
                int green = (int)(0.25f * (a[1] + b[1] + c[1] + d[1]));
                int blue = (int)(0.25f * (a[2] + b[2] + c[2] + d[2]));

                // Увеличение яркости (если включено)
                if (brighten)
                {
                    Color.RGBToHSV(new Color32((byte)red, (byte)green, (byte)blue, 255), out float h, out float s, out float v);
                    v = -((v - 1) * (v - 1)) + 1;
                    Color resultColor = Color.HSVToRGB(h, s, v);
                    Color32 color32 = resultColor;
                    result[x][y] = (color32.r << 16) | (color32.g << 8) | color32.b;
                }
                else
                {
                    result[x][y] = (red << 16) | (green << 8) | blue;
                }
            }
        }

        // Рекурсия для уменьшения в 2 раза, если требуется
        if (scale > 2)
        {
            return BilinearDownscale(result, scale / 2, brighten);
        }

        return result;
    }
}

public static class ColorExtensions
{
    public static int ToInt(this Color32 color)
    {
        return (color.r << 16) | (color.g << 8) | color.b;
    }
}
