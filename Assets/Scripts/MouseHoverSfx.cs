using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

[RequireComponent(typeof(BoxCollider2D))]
public class MouseHoverPushSFX : MonoBehaviour
{
    [SerializeField] private string sfxName = "sound";
    [SerializeField] private float pushSpeedPixels = 600f;

    private Camera _cam;
    private Collider2D _col;
    private bool _hasPlayed;

    void Awake ()
    {
        _cam = Camera.main;
        _col = GetComponent<Collider2D>();
    }

    void Update ()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector2 worldPos  = _cam.ScreenToWorldPoint(screenPos);

        if (_col.OverlapPoint(worldPos))
        {
            if (!_hasPlayed)
            {
                AudioController.PlaySFX(sfxName);
                _hasPlayed = true;
            }
            
            Vector2 dir = (worldPos - (Vector2)_col.bounds.center).normalized;
            Vector2 newScreenPos = screenPos + dir * pushSpeedPixels * Time.deltaTime;
            
            Mouse.current.WarpCursorPosition(newScreenPos);
            
            InputState.Change(Mouse.current.position, newScreenPos);
        }
        else
        {
            _hasPlayed = false;
        }
    }
}