using UnityEngine;

public class AnimateYPositionWithHover : MonoBehaviour
{
    [Header("Настройки анимации")]
    [Tooltip("Полная длительность цикла анимации (от keyValue1 до keyValue2 и обратно) в секундах")]
    public float duration = 4f;
    
    [Tooltip("Множитель скорости обратной анимации (1 – та же скорость, что и вперед)")]
    public float reverseMultiplier = 1f;

    [Tooltip("Задержка перед запуском анимации (в секундах)")]
    public float delayBeforeStart = 0f;

    [Header("Ключевые значения оси Y")]
    [Tooltip("Начальное значение (и конечное) position.y")]
    public float keyValue1 = -0.9f;
    [Tooltip("Промежуточное значение position.y")]
    public float keyValue2 = -1.1f;
    
    private float animProgress = 0f;
    private bool isHovered = false;
    private float forwardSpeed;
    private float reverseSpeed;
    private float elapsedTime = 0f;

    private void Start()
    {
        forwardSpeed = 1f / duration;
        reverseSpeed = forwardSpeed * reverseMultiplier;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        
        if (elapsedTime < delayBeforeStart)
            return;
        
        if (isHovered)
        {
            if (animProgress > 0f)
            {
                animProgress -= reverseSpeed * Time.deltaTime;
                if (animProgress < 0f)
                    animProgress = 0f;
            }
        }
        else
        {
            animProgress += forwardSpeed * Time.deltaTime;
            if (animProgress > 1f)
                animProgress -= 1f;
        }
        
        float newY;
        if (animProgress <= 0.5f)
        {
            float t = animProgress / 0.5f;
            newY = Mathf.Lerp(keyValue1, keyValue2, Mathf.SmoothStep(0f, 1f, t));
        }
        else
        {
            float t = (animProgress - 0.5f) / 0.5f;
            newY = Mathf.Lerp(keyValue2, keyValue1, Mathf.SmoothStep(0f, 1f, t));
        }
        
        Vector3 pos = transform.position;
        pos.y = newY;
        transform.position = pos;
    }
    
    private void OnMouseEnter()
    {
        isHovered = true;
    }
    
    private void OnMouseExit()
    {
        isHovered = false;
    }
}
