using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct CursorMapping
{
    public CursorType cursorType;
    public Sprite sprite;
}

public class CustomCursor : MonoBehaviour
{
    public static CustomCursor Instance { get; private set; }
    
    [SerializeField]
    private bool hideSystemCursor = true;
    
    [SerializeField]
    private List<CursorMapping> cursorMappings = new();

    private Dictionary<CursorType, Sprite> cursorSpriteDictionary;
    private SpriteRenderer spriteRenderer;
    
    [SerializeField] 
    private GameObject drawableObj;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Mouse.current.WarpCursorPosition(center);
        
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (hideSystemCursor)
        {
            Texture2D emptyTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            emptyTexture.SetPixel(0, 0, Color.clear);
            emptyTexture.Apply();
            Cursor.SetCursor(emptyTexture, Vector2.zero, CursorMode.Auto);
        }
        
        InitializeCursorDictionary();
        SetCursorSprite(CursorType.Hand);
    }

    private void InitializeCursorDictionary()
    {
        cursorSpriteDictionary = new Dictionary<CursorType, Sprite>();

        foreach (var mapping in cursorMappings)
        {
            if (cursorSpriteDictionary.ContainsKey(mapping.cursorType))
            {
                Debug.LogWarning("Дублирование типа курсора: " + mapping.cursorType);
                cursorSpriteDictionary[mapping.cursorType] = mapping.sprite;
            }
            else
            {
                cursorSpriteDictionary.Add(mapping.cursorType, mapping.sprite);
            }
        }
    }

    /// <summary>
    /// Меняет спрайт курсора по типу.
    /// </summary>
    public void SetCursorSprite(CursorType type)
    {
        if (cursorSpriteDictionary != null && cursorSpriteDictionary.TryGetValue(type, out Sprite newSprite))
        {
            if (newSprite != null)
            {
                spriteRenderer.sprite = newSprite;
                
                if (drawableObj != null)
                {
                    drawableObj.SetActive(type == CursorType.Feather);
                }
            }
            else
            {
                Debug.LogWarning("Спрайт для типа " + type + " не задан.");
            }
        }
        else
        {
            Debug.LogWarning("Курсор типа " + type + " не найден в словаре.");
        }
    }

    private void Update()
    {
        UpdateCursorPosition();
    }

    private void UpdateCursorPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        if (Camera.main != null)
        {
            mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.z = 0f;
            transform.position = worldPos;
        }
    }
}
