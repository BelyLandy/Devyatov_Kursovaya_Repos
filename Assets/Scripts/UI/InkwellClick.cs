using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class InkwellClick : MonoBehaviour, IClickable
{
    [Header("Настройки пера этой чернильницы")]
    [SerializeField] private PenColorSettings _penColorSettings;
    [SerializeField] private SpriteRenderer _featherRenderer;
    [SerializeField] private Color _color;
    
    public static bool _isFeatherPickedUp;

    public void OnClick()
    {
        if (!_isFeatherPickedUp && _featherRenderer.enabled)
        {
            PickUpFeather();
        }
        else if (_isFeatherPickedUp && !_featherRenderer.enabled)
        {
            PutDownFeather();
        }
    }

    private void PickUpFeather()
    {

        CustomCursor.Instance.SetCursorSprite(CursorType.Feather);

        _isFeatherPickedUp = true;

        _featherRenderer.enabled = false;

        _penColorSettings.currentColor = _color;

        ToggleAnimatableColliders(true);

        ToggleAllInkwellColliders();
    }

    private void PutDownFeather()
    {
        CustomCursor.Instance.SetCursorSprite(CursorType.Hand);

        _isFeatherPickedUp = false;

        _featherRenderer.enabled = true;

        //_lastSource._featherRenderer.enabled = false;
        //_lastSource = null;

        ToggleAnimatableColliders(false);

        ToggleFalseOtherInkwellColliders();
    }

    private void ToggleAnimatableColliders(bool enable)
    {
        var animatables = GameObject.FindGameObjectsWithTag("Animatable");
        foreach (var go in animatables)
        {
            var col = go.GetComponent<BoxCollider2D>();
            if (col != null)
                col.enabled = enable;
        }
    }

    private void ToggleFalseOtherInkwellColliders()
    {
        // **Active** inkwells only – match old behaviour
        var allInkwells = Object.FindObjectsByType<InkwellClick>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None);

        foreach (var inkwell in allInkwells)
        {
            if (inkwell == this) continue;

            var childCol = inkwell.GetComponentInChildren<Collider2D>();
            if (childCol) childCol.enabled = false;
        }
    }

    private void ToggleAllInkwellColliders()
    {
        var allInkwells = Object.FindObjectsByType<InkwellClick>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None);

        foreach (var inkwell in allInkwells)
        {
            var childCol = inkwell.GetComponentInChildren<Collider2D>();
            if (childCol) childCol.enabled = true;
        }
    }
}
