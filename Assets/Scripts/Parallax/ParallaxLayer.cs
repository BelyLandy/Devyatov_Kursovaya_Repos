using UnityEngine;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("Коэффициент, определяющий, насколько сильно этот слой движется по сравнению с камерой.")]
    public float parallaxFactor = 1.0f;
    [Tooltip("Включить циклическое повторение слоя")]
    public bool loop = true;

    private float spriteWidth;

    void Start()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            spriteWidth = sr.bounds.size.x;
        }
    }

    public void Move(float delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta * parallaxFactor;
        transform.localPosition = newPos;

        if (loop)
        {
            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                if (transform.localPosition.x <= -spriteWidth)
                {
                    transform.localPosition += new Vector3(2 * spriteWidth, 0, 0);
                }
                else if (transform.localPosition.x >= spriteWidth)
                {
                    transform.localPosition -= new Vector3(2 * spriteWidth, 0, 0);
                }
            }
        }
    }
}
