using UnityEngine;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("Коэффициент, определяющий, насколько сильно этот слой движется по сравнению с камерой.")]
    public float parallaxFactor = 1.0f;
    [Tooltip("Включить циклическое повторение слоя")]
    public bool loop = true;

    // Для работы цикличности нам нужно знать ширину спрайта
    private float spriteWidth;

    void Start()
    {
        // Получаем ширину спрайта, на котором основан слой.
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            spriteWidth = sr.bounds.size.x;
            // Для корректной цикличности объекты должны быть расположены так:
            // один объект имеет x = 0, второй – x = -spriteWidth, третий – x = +spriteWidth.
            // Если ваши копии расположены иначе, отрегулируйте условия ниже.
        }
    }

    public void Move(float delta)
    {
        // Передвигаем слой относительно камеры с учетом parallaxFactor
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta * parallaxFactor;
        transform.localPosition = newPos;

        // Если включена цикличность и спрайт присутствует, проверяем границы
        if (loop)
        {
            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                // Если объект сдвинулся влево за границу (условно, его локальная X меньше -spriteWidth)
                if (transform.localPosition.x <= -spriteWidth)
                {
                    // Перемещаем его вправо ровно на 2 * spriteWidth.
                    // Так копия из левого края оказывается справа от центрального и правого экземпляра.
                    transform.localPosition += new Vector3(2 * spriteWidth, 0, 0);
                }
                // Если объект сдвинулся вправо за границу (условно, его локальная X больше +spriteWidth)
                else if (transform.localPosition.x >= spriteWidth)
                {
                    // Перемещаем его влево на 2 * spriteWidth.
                    transform.localPosition -= new Vector3(2 * spriteWidth, 0, 0);
                }
            }
        }
    }
}
