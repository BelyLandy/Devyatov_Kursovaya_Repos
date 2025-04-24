using UnityEngine;

public class CoverBookColor : MonoBehaviour
{
    [SerializeField] private ColorSettings colorSettings;
    private SpriteRenderer _sr;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        if (_sr == null)
            Debug.LogError("SpriteRenderer не найден", this);
        if (colorSettings == null)
            Debug.LogError("ColorSettings не назначен", this);
    }

    private void Start()
    {
        if (colorSettings != null && _sr != null)
        {
            _sr.color = colorSettings.currentColor;
        }
    }
}
