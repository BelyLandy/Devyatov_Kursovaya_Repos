using System.Collections.Generic;
using UnityEngine;

public class ZoneSoundPlayer : MonoBehaviour
{
    [Header("Список имён звуков для поочередного воспроизведения")]
    [SerializeField]
    private List<string> soundNames = new List<string>();
    
    private int currentIndex = 0;
    
    public void OnZoneShapeRecognized()
    {
        if (soundNames == null || soundNames.Count == 0)
            return;
        
        SoundManager.Instance.PlaySound2D(soundNames[currentIndex]);
        
        currentIndex = (currentIndex + 1) % soundNames.Count;
    }
}
