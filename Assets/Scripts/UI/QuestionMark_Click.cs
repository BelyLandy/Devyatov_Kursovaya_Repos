using System.Collections;
using TMPro;
using UnityEngine;

public class QuestionMark_Click : MonoBehaviour, IClickable
{
    [Header("TextMeshPro")]
    [Tooltip("Ссылка на TMP‑текст, у которого меняем Face Color")]
    [SerializeField] private TMP_Text tmpText;

    [Header("Шейдерное свойство")]
    [Tooltip("Название свойства Face Color в материале (обычно '_FaceColor')")]
    [SerializeField] private string colorProperty = "_FaceColor";

    [Header("Настройки затухания")]
    [Tooltip("Длительность фейда (в секундах)")]
    [SerializeField] private float fadeDuration = 1f;

    private bool _isVisible = false;
    private Coroutine _fadeCoroutine;
    private Material _matInstance;

    private void Awake()
    {
        if (tmpText == null)
        {
            Debug.LogError("[QuestionMark_Click] TMP_Text не назначен!", this);
            enabled = false;
            return;
        }
        
        _matInstance = tmpText.fontMaterial;
        tmpText.fontMaterial = _matInstance;
        
        Color col = _matInstance.GetColor(colorProperty);
        col.a = 0f;
        _matInstance.SetColor(colorProperty, col);
    }
    
    public void OnClick()
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);
        
        float targetAlpha = _isVisible ? 0f : 1f;
        _fadeCoroutine = StartCoroutine(FadeFaceColor(targetAlpha));
        _isVisible = !_isVisible;
    }

    public void DoHide()
    {
        _fadeCoroutine = StartCoroutine(FadeFaceColor(0));
    }
    
    private IEnumerator FadeFaceColor(float targetAlpha)
    {
        Color col = _matInstance.GetColor(colorProperty);
        float startAlpha = col.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            col.a = a;
            _matInstance.SetColor(colorProperty, col);
            yield return null;
        }
        
        col.a = targetAlpha;
        _matInstance.SetColor(colorProperty, col);
        _fadeCoroutine = null;
    }
}
