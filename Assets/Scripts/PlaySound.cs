using UnityEngine;
using System.Collections.Generic;

public class PlaySound : MonoBehaviour, IClickable
{
    [Header("Список имён звуков для поочередного воспроизведения")]
    [SerializeField]
    private List<string> soundNames = new();
    
    private int currentIndex;

    public void OnClick()
    {
        if (soundNames == null || soundNames.Count == 0)
            return;
        
        AudioController.PlaySFX(soundNames[currentIndex]);

        currentIndex = (currentIndex + 1) % soundNames.Count;
    }
}