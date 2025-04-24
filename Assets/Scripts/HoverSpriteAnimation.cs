using System.Collections;
using CW_Devyatov_238;
using UnityEngine;

[System.Serializable]
public class FrameToggle
{
    [Tooltip("Объект для переключения активного состояния при указанном кадре")]
    public GameObject obj;
    [Tooltip("Индекс кадра, на котором переключать объект")]
    public int frameIndex;
}

[System.Serializable]
public class AlternateSprite
{
    [Tooltip("Индекс кадра, на котором использовать альтернативный спрайт")]
    public int frameIndex;
    [Tooltip("Альтернативный спрайт")]
    public Sprite sprite;
}

public class HoverSpriteAnimation : MonoBehaviour
{
    public enum PlayMode
    {
        Standard,
        LoopWhileHovered
    }

    [Header("Основные настройки")]
    [Tooltip("Режим проигрывания анимации")]
    [SerializeField] private PlayMode playMode = PlayMode.Standard;
    [Tooltip("Массив спрайтов анимации")]
    [SerializeField] private Sprite[] sprites;
    [Tooltip("Время задержки между кадрами анимации")]
    [SerializeField] private float frameDelay = 0.1f;

    [Header("Альтернативные спрайты")]
    [Tooltip("Список альтернативных спрайтов по индексам")]
    [SerializeField] private AlternateSprite[] alternateSprites;
    [Tooltip("Включать ли подстановку альтернативных спрайтов для этого объекта")]
    [SerializeField] private bool useAlternateSprites = false;

    /// <summary>
    /// Если нужно переключать из другого скрипта
    /// </summary>
    public bool UseAlternateSprites
    {
        get => useAlternateSprites;
        set => useAlternateSprites = value;
    }

    [Header("Звук")]
    [SerializeField] private string OnMouseEnterSound;
    [SerializeField] private string OnMouseExitSound;

    [Header("Настройка Light2D Intensity")]
    [Tooltip("Включить изменение intensity у целевого Light2D")]
    [SerializeField] private bool adjustLightIntensity = false;
    [Tooltip("Ссылка на Light2D, у которого меняем intensity")]
    [SerializeField] private UnityEngine.Rendering.Universal.Light2D targetLight;
    [Tooltip("Шаг изменения intensity за кадр")]
    [SerializeField] private float intensityStep = 0.05f;

    [Header("Переключаемые объекты")]
    [Tooltip("Список объектов и номеров кадров для их активации/деактивации")]
    [SerializeField] private FrameToggle[] toggles;

    private SpriteRenderer spriteRenderer;
    private Coroutine animCoroutine;
    private int currentFrame = 0;
    private bool isHovered = false;
    
    [SerializeField] private string killStatsPath = "KillStatsSO";
    
    private KillStatsSO stats;

    [SerializeField] private int statsIndex;
    
    private bool useToggles => toggles != null && toggles.Length > 0;

    private void Awake()
    {
        stats = Resources.Load<KillStatsSO>(killStatsPath);
        
        if (stats != null)
            useAlternateSprites = stats.GetKills(statsIndex) != 0;
        else
            useAlternateSprites = false;

  
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.LogError("Компонент SpriteRenderer не найден на объекте!", this);

        if (adjustLightIntensity)
        {
            if (targetLight == null)
                Debug.LogError("AdjustLightIntensity включён, но Light2D не назначен!", this);
            else
                targetLight.intensity = 0f;
        }

        if (useToggles)
        {
            foreach (var t in toggles)
                if (t.obj != null)
                    t.obj.SetActive(false);
        }
    }

    private void OnMouseEnter()
    {
        if (!string.IsNullOrEmpty(OnMouseEnterSound))
            AudioController.PlaySFX(OnMouseEnterSound);

        isHovered = true;
        StopCurrentAnimation();

        if (playMode == PlayMode.LoopWhileHovered)
            animCoroutine = StartCoroutine(PlayLoop());
        else
            animCoroutine = StartCoroutine(PlayForward());
    }

    private void OnMouseExit()
    {
        if (!string.IsNullOrEmpty(OnMouseExitSound))
            AudioController.PlaySFX(OnMouseExitSound);

        isHovered = false;
        StopCurrentAnimation();
        animCoroutine = StartCoroutine(PlayReverse());
    }

    private void StopCurrentAnimation()
    {
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
            animCoroutine = null;
        }
    }

    private IEnumerator PlayForward()
    {
        currentFrame = 0;
        while (isHovered && currentFrame < sprites.Length)
        {
            ApplyFrame(currentFrame);
            if (useToggles) EnableTogglesAtFrame(currentFrame);
            currentFrame++;
            yield return new WaitForSeconds(frameDelay);
        }
    }

    private IEnumerator PlayLoop()
    {
        currentFrame = 0;
        while (isHovered)
        {
            if (currentFrame >= sprites.Length)
                currentFrame = 0;
            ApplyFrame(currentFrame);
            if (useToggles) EnableTogglesAtFrame(currentFrame);
            currentFrame++;
            yield return new WaitForSeconds(frameDelay);
        }
    }

    private IEnumerator PlayReverse()
    {
        int frameIndex = currentFrame - 1;
        while (frameIndex >= 0)
        {
            if (useToggles) DisableTogglesAtFrame(frameIndex);
            ApplyFrame(frameIndex);
            frameIndex--;
            yield return new WaitForSeconds(frameDelay);
        }

        // Сброс состояния
        currentFrame = 0;
        if (adjustLightIntensity && targetLight != null)
            targetLight.intensity = 0f;

        if (useToggles)
        {
            foreach (var t in toggles)
                if (t.obj != null)
                    t.obj.SetActive(false);
        }
    }

    private void ApplyFrame(int frameIndex)
    {
        Sprite toApply = sprites[frameIndex];

        // Подстановка альтернативы, если включено
        if (useAlternateSprites)
        {
            var alt = GetAlternateSprite(frameIndex);
            if (alt != null)
                toApply = alt;
        }

        spriteRenderer.sprite = toApply;

        if (adjustLightIntensity && targetLight != null)
            targetLight.intensity = frameIndex * intensityStep;
    }

    private Sprite GetAlternateSprite(int frameIndex)
    {
        if (alternateSprites == null) return null;
        foreach (var a in alternateSprites)
        {
            if (a.sprite != null && a.frameIndex == frameIndex)
                return a.sprite;
        }
        return null;
    }

    private void EnableTogglesAtFrame(int frameIndex)
    {
        foreach (var t in toggles)
        {
            if (t.obj != null && t.frameIndex == frameIndex)
                t.obj.SetActive(true);
        }
    }

    private void DisableTogglesAtFrame(int frameIndex)
    {
        foreach (var t in toggles)
        {
            if (t.obj != null && t.frameIndex == frameIndex)
                t.obj.SetActive(false);
        }
    }
}
