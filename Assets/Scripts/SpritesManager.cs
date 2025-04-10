using UnityEngine;

public class SpritesManager : MonoBehaviour
{
    private SpriteRenderer _mainSprite;
    private SpriteRenderer _shadowSprite;
    
    void Awake()
    {
        _mainSprite = GetComponent<SpriteRenderer>();
        _shadowSprite = GetComponentInChildren<SpriteRenderer>();
    }
    
    void Update()
    {
        _shadowSprite.sprite = _mainSprite.sprite;
    }
}
