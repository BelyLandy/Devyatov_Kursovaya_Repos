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

    // Переменная, характеризующая прогресс анимации:
    // 0 соответствует keyValue1, 0.5 – keyValue2, 1 – возвращение к keyValue1.
    private float animProgress = 0f;
    // Флаг, показывающий, находится ли мышь над объектом.
    private bool isHovered = false;
    // Скорости анимации для прямого и обратного воспроизведения.
    private float forwardSpeed;
    private float reverseSpeed;
    // Время, прошедшее с начала работы скрипта.
    private float elapsedTime = 0f;

    private void Start()
    {
        // Полный цикл (0 -> 1) занимает duration секунд.
        forwardSpeed = 1f / duration;
        reverseSpeed = forwardSpeed * reverseMultiplier;
    }

    private void Update()
    {
        // Увеличиваем общий счетчик времени
        elapsedTime += Time.deltaTime;

        // Если задержка не прошла, выходим из Update
        if (elapsedTime < delayBeforeStart)
            return;

        // Если курсор над объектом – запускаем обратную анимацию (уменьшаем прогресс до 0)
        if (isHovered)
        {
            if (animProgress > 0f)
            {
                animProgress -= reverseSpeed * Time.deltaTime;
                if (animProgress < 0f)
                    animProgress = 0f;
            }
        }
        else // Если курсора нет – идёт прямая анимация
        {
            animProgress += forwardSpeed * Time.deltaTime;
            // Зацикливаем анимацию: когда прогресс превышает 1, уменьшаем его на 1 (сохраняя остаток)
            if (animProgress > 1f)
                animProgress -= 1f;
        }

        // Вычисляем новое значение position.y с нелинейной интерполяцией (Mathf.SmoothStep)
        float newY;
        if (animProgress <= 0.5f)
        {
            // От 0 до 0.5 – интерполяция от keyValue1 до keyValue2
            float t = animProgress / 0.5f;
            newY = Mathf.Lerp(keyValue1, keyValue2, Mathf.SmoothStep(0f, 1f, t));
        }
        else
        {
            // От 0.5 до 1 – интерполяция от keyValue2 обратно к keyValue1
            float t = (animProgress - 0.5f) / 0.5f;
            newY = Mathf.Lerp(keyValue2, keyValue1, Mathf.SmoothStep(0f, 1f, t));
        }

        // Применяем новое значение к оси Y, сохраняя X и Z без изменений.
        Vector3 pos = transform.position;
        pos.y = newY;
        transform.position = pos;
    }

    // При наведении мыши устанавливаем флаг, чтобы анимация шла в обратном направлении.
    private void OnMouseEnter()
    {
        isHovered = true;
    }

    // При уходе мыши сбрасываем флаг; анимация продолжится вперед с текущего положения.
    private void OnMouseExit()
    {
        isHovered = false;
    }
}
