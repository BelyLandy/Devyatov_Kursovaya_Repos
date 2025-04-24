using System.Collections;
using UnityEngine;

public class ScaleAnimator : MonoBehaviour
{
    [Header("Target & Timing")]
    [Tooltip("RectTransform to animate; if empty, uses this GameObject's RectTransform")]
    [SerializeField] private RectTransform targetRect;

    [Tooltip("Duration of the scale animation in seconds")]
    [SerializeField] private float duration = 0.5f;

    [Tooltip("Final scale for PlayScaleUp")]
    [SerializeField] private Vector3 targetScale = new Vector3(20f, 20f, 20f);

    private Coroutine currentCoroutine;

    [SerializeField] private MixerFader _mixerFader;

    [SerializeField] private bool doScaleDown = false;
    
    private void Awake()
    {
        if (targetRect == null)
            targetRect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        if (doScaleDown)
        {
            PlayScaleDown(true, SceneLoader.Scene.MenuScene);
            return;
        }
        
        PlayScaleUp();
    }

    /// <summary>
    /// Анимация scale от 0 до targetScale.
    /// </summary>
    public void PlayScaleUp()
    {
        PlayScaleUp(false, default);
    }

    /// <summary>
    /// Анимация scale от 0 до targetScale, с опциональной сменой сцены по окончании.
    /// </summary>
    public void PlayScaleUp(bool doSceneLoad, SceneLoader.Scene scene)
    {

        if (_mixerFader != null)
        {
            _mixerFader.FadeIn(duration);
        }
        
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        targetRect.localScale = Vector3.zero;
        currentCoroutine = StartCoroutine(ScaleRoutine(
            Vector3.zero, 
            targetScale, 
            doSceneLoad, 
            scene
        ));
    }

    /// <summary>
    /// Анимация scale от current до 0.
    /// </summary>
    public void PlayScaleDown()
    {
        PlayScaleDown(false, default);
    }

    /// <summary>
    /// Анимация scale от current до 0, с опциональной сменой сцены по окончании.
    /// </summary>
    public void PlayScaleDown(bool doSceneLoad, SceneLoader.Scene scene)
    {
        if (_mixerFader != null)
        {
            _mixerFader.FadeOut(duration);
        }
        
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        Vector3 startScale = targetRect.localScale;
        currentCoroutine = StartCoroutine(ScaleRoutine(
            startScale, 
            Vector3.zero, 
            doSceneLoad, 
            scene
        ));
    }

    /// <summary>
    /// Общая корутина анимации scale.
    /// </summary>
    private IEnumerator ScaleRoutine(
        Vector3 from,
        Vector3 to,
        bool    doSceneLoad,
        SceneLoader.Scene scene
    )
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            targetRect.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }

        // финальный масштаб
        targetRect.localScale = to;
        currentCoroutine = null;

        // переключаем сцену, если нужно
        if (doSceneLoad)
            SceneLoader.Load(scene);
    }
}
