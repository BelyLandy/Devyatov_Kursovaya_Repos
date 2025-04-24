using UnityEngine;
using System.Collections;

public class PlayAudio : MonoBehaviour
{
    [Header("Audio")]
    public string audioItemName = "";

    [Header("Autoplay")]
    [SerializeField] private bool isOnStart = false;
    [SerializeField] private float startDelay = 0f;

    private void Start()
    {
        if (isOnStart)
        {
            if (startDelay > 0f)
            {
                PlayDelayed(startDelay);
                return;
            }
            Play();
        }
            
    }
    
    public void Play()
    {
        if (!string.IsNullOrEmpty(audioItemName))
            CW_Devyatov_238.AudioController.PlaySFX(audioItemName);
    }
    
    public void PlayDelayed(float customDelay)
    {
        StartCoroutine(PlayAfterDelay(customDelay));
    }
    
    private IEnumerator PlayAfterDelay(float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        Play();
    }
}