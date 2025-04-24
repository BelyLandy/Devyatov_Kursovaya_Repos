using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class TextureColorReplacer : MonoBehaviour
{
    [Tooltip("Texture to process (e.g. from Assets)")]
    public Texture2D sourceTexture;

    [Tooltip("Optional: Material to apply the processed texture to")]
    public Material targetMaterial;

    [Tooltip("List of colors in the texture to replace")]
    public List<Color> oldColors = new List<Color>();

    [Tooltip("List of replacement colors (must match length of oldColors)")]
    public List<Color> newColors = new List<Color>();

    [ContextMenu("Replace Colors")]  // Adds a menu item to the component's context menu
    public void ReplaceColors()
    {
        if (sourceTexture == null)
        {
            Debug.LogError("Source Texture is not assigned.");
            return;
        }

        if (oldColors.Count != newColors.Count)
        {
            Debug.LogError("Old colors list and new colors list must have the same length.");
            return;
        }

#if UNITY_EDITOR
        string assetPath = AssetDatabase.GetAssetPath(sourceTexture);
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null)
        {
            Debug.LogError("Could not get TextureImporter for " + assetPath);
            return;
        }

        // Ensure texture is readable
        if (!importer.isReadable)
        {
            importer.isReadable = true;
            importer.SaveAndReimport();
        }
#endif

        // Modify the original texture
        Color[] pixels = sourceTexture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            for (int j = 0; j < oldColors.Count; j++)
            {
                if (pixels[i] == oldColors[j])
                {
                    pixels[i] = newColors[j];
                    break;
                }
            }
        }
        sourceTexture.SetPixels(pixels);
        sourceTexture.Apply();

        // Apply to material if provided
        if (targetMaterial != null)
        {
            targetMaterial.mainTexture = sourceTexture;
        }

#if UNITY_EDITOR
        // Write back to asset file
        byte[] pngData = sourceTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(assetPath, pngData);
        AssetDatabase.ImportAsset(assetPath);
        Debug.Log("Updated original texture asset: " + assetPath);
#endif

        Debug.Log("Color replacement completed on original texture: " + sourceTexture.name);
    }
}
