using UnityEngine;

public class FeatherInInkwellClick : MonoBehaviour, IClickable
{
    private SpriteRenderer _spriteRenderer;
    private bool isFeatherActive;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnClick()
    {
        if (!isFeatherActive)
        {
            CustomCursor.Instance.SetCursorSprite(CursorType.Feather);
            _spriteRenderer.enabled = false;
            isFeatherActive = true;
        }
        else
        {
            CustomCursor.Instance.SetCursorSprite(CursorType.Hand);
            _spriteRenderer.enabled = true;
            isFeatherActive = false;
        }
    }
}