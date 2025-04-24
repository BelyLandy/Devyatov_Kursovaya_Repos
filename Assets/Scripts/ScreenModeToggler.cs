using UnityEngine;

public class ScreenModeToggler : MonoBehaviour
{
    [SerializeField] private int windowWidth  = 1280;
    [SerializeField] private int windowHeight = 720;
    [SerializeField] private PenColorSettings _penColorSettings;
    [SerializeField] private GameObject windowModeIndicator;
    
    public static bool isFullscreen = true;
    
    private void Start()
    {
        int full = PlayerPrefs.GetInt("Screenmanager Is Fullscreen mode",
                                      Screen.fullScreen ? 1 : 0);

        isFullscreen = full == 1;
        
        if (windowModeIndicator != null)
        {
            bool windowed = !isFullscreen;


            if (windowed)
            {
                windowModeIndicator.SetActive(windowed);
                windowModeIndicator.GetComponent<SpriteRenderer>().color =
                    _penColorSettings.currentColor;
            }
                
        }
    }

    public void ToggleWindowMode()
    {
        if (isFullscreen)
            SetWindowedMode();
        else
            SetFullscreenMode();
    }

    private void SetWindowedMode()
    {
        Screen.SetResolution(windowWidth, windowHeight, FullScreenMode.Windowed);
        isFullscreen = false;

        if (windowModeIndicator != null)
        {
            windowModeIndicator.SetActive(true);
            windowModeIndicator.GetComponent<SpriteRenderer>().color = _penColorSettings.currentColor;
        }

        Debug.Log($"Режим: Оконный ({windowWidth}×{windowHeight}, 16:9) — Индикатор ВКЛ");
    }

    private void SetFullscreenMode()
    {
        Resolution res = Screen.currentResolution;
        
        if (res.width * 9 != res.height * 16)
            res.height = res.width * 9 / 16;

        Screen.SetResolution(res.width, res.height, FullScreenMode.FullScreenWindow);
        isFullscreen = true;

        if (windowModeIndicator != null)
            windowModeIndicator.SetActive(false);

        Debug.Log($"Режим: Полноэкранный ({res.width}×{res.height}, 16:9) — Индикатор ВЫКЛ");
    }
}
