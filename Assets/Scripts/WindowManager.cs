using UnityEngine;

public class WindowManager : MonoBehaviour
{
    // Статическая ссылка на экземпляр синглтона
    public static WindowManager Instance { get; private set; }

    // Флаг, показывающий, включён ли полноэкранный режим.
    private bool isFullscreen = false;
    // Разрешение для оконного режима.
    private int windowedWidth = 1280;
    private int windowedHeight = 720;

    private void Awake()
    {
        // Если экземпляр еще не установлен, то сохраняем этот объект
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Объект сохраняется при смене сцен
        }
        else
        {
            // Если объект уже существует, уничтожаем текущий, чтобы избежать дубликатов
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Устанавливает оконный режим с указанной шириной и высотой.
    /// </summary>
    /// <param name="width">Ширина окна.</param>
    /// <param name="height">Высота окна.</param>
    public void SetWindowedMode(int width, int height)
    {
        windowedWidth = width;
        windowedHeight = height;
        Screen.SetResolution(width, height, false);
        isFullscreen = false;
    }

    /// <summary>
    /// Включает полноэкранный режим.
    /// </summary>
    public void SetFullScreenMode()
    {
        Screen.fullScreen = true;
        isFullscreen = true;
    }

    /// <summary>
    /// Переключает режим отображения между полноэкранным и оконным.
    /// При переходе в оконный режим используется ранее заданное разрешение.
    /// </summary>
    public void ToggleFullscreenMode()
    {
        if (isFullscreen)
        {
            // Переключаем на оконный режим с сохранённым разрешением
            Screen.SetResolution(windowedWidth, windowedHeight, false);
            isFullscreen = false;
        }
        else
        {
            // Переключаем на полноэкранный режим
            Screen.fullScreen = true;
            isFullscreen = true;
        }
    }
}
