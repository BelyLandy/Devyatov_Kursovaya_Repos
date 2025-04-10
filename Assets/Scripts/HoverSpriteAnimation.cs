using System.Collections;
using UnityEngine;

public class HoverSpriteAnimation : MonoBehaviour
{
    [Tooltip("Массив спрайтов анимации, которые будут проигрываться по порядку")]
    [SerializeField] private Sprite[] sprites;

    [Tooltip("Время задержки между кадрами анимации")]
    [SerializeField] private float frameDelay = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Coroutine animCoroutine;
    private int currentFrame = 0;
    private bool isHovered = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Компонент SpriteRenderer не найден на объекте!");
        }
    }
    
    private void OnMouseEnter()
    {
        SoundManager.Instance.PlaySound2D("OpenBook");
        isHovered = true;

        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
        }

        animCoroutine = StartCoroutine(PlayForward());
    }
    
    private void OnMouseExit()
    {
        SoundManager.Instance.PlaySound2D("CloseBook");
        isHovered = false;
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
        }
        animCoroutine = StartCoroutine(PlayReverse());
    }
    
    private IEnumerator PlayForward()
    {
        currentFrame = 0;
        while (isHovered && currentFrame < sprites.Length)
        {
            spriteRenderer.sprite = sprites[currentFrame];
            currentFrame++;
            yield return new WaitForSeconds(frameDelay);
        }
    }
    
    private IEnumerator PlayReverse()
    {
        int frameIndex = currentFrame - 1;
        while (frameIndex >= 0)
        {
            spriteRenderer.sprite = sprites[frameIndex];
            frameIndex--;
            yield return new WaitForSeconds(frameDelay);
        }
        currentFrame = 0;
    }
}
