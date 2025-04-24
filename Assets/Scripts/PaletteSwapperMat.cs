using UnityEngine;

public class PaletteMaterialSwapper : MonoBehaviour
{
    [Header("Материал, в котором нужно заменить цвета")]
    [SerializeField] private Material paletteMaterial;

    [Header("Новые цвета (для _NewColor0, _NewColor1, _NewColor2)")]
    [SerializeField] private Color newColor0 = Color.white;
    [SerializeField] private Color newColor1 = Color.white;
    [SerializeField] private Color newColor2 = Color.white;
    
    private static readonly int NewColor0ID = Shader.PropertyToID("_NewColor0");
    private static readonly int NewColor1ID = Shader.PropertyToID("_NewColor1");
    private static readonly int NewColor2ID = Shader.PropertyToID("_NewColor2");
    private static readonly int SwapCountID = Shader.PropertyToID("_ColorSwapCount");
    
    public void ApplyPalette()
    {
        if (paletteMaterial == null)
        {
            Debug.LogWarning($"[{name}] PaletteMaterialSwapper: материал не задан!");
            return;
        }

        paletteMaterial.SetColor(NewColor0ID, newColor0);
        paletteMaterial.SetColor(NewColor1ID, newColor1);
        paletteMaterial.SetColor(NewColor2ID, newColor2);
        paletteMaterial.SetFloat(SwapCountID, 3); // сообщаем шейдеру, что надо обрабатывать 3 цвета
    }
}