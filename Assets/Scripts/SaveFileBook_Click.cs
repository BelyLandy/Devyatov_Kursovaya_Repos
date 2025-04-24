using System;
using System.Collections;
using UnityEngine;

public class SaveFileBook_Click : MonoBehaviour, IClickable
{
    [Header("Color Settings")]
    [SerializeField] private ColorSettings colorSettings;
    [SerializeField] private Color         selectedColor;

    [Header("Animation Settings")]
    [SerializeField] private RectTransform animatedRect;     // UI‑элемент для анимации
    [SerializeField] private Canvas        parentCanvas;    // Канвас, в котором лежит animatedRect
    [SerializeField] private float         animationDuration = 0.5f;

    [Header("Save Settings")]
    [SerializeField] private int slotIndex;
    private KillStatsSO stats;

    private void Awake()
    {
        stats = Resources.Load<KillStatsSO>("KillStatsSO");
    }

    private void Start()
    {
        if (animatedRect == null)
            Debug.LogWarning("Animated RectTransform not assigned on " + name);
        if (parentCanvas == null)
            Debug.LogWarning("Parent Canvas not assigned on " + name);
    }

    public void OnClick()
    {
        stats.selectedSlot = slotIndex;
        
        // 1. Меняем цвет
        colorSettings.currentColor = selectedColor;

        // 2. Центрируем UI‑элемент на этом объекте
        Vector3 worldPos  = transform.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();
        Vector2       localPoint;

        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, screenPos, null, out localPoint);
        else
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, screenPos, parentCanvas.worldCamera, out localPoint);

        animatedRect.anchoredPosition = localPoint;

        // 3. Запускаем анимацию масштаба
        StartCoroutine(ScaleDown(animatedRect, animationDuration));
    }

    private IEnumerator ScaleDown(RectTransform rect, float duration)
    {
        Vector3 initialScale = rect.localScale;
        float   elapsed      = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            rect.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
            yield return null;
        }

        rect.localScale = Vector3.zero;
        
        SceneLoader.Load(SceneLoader.Scene.MenuScene);
    }
}
