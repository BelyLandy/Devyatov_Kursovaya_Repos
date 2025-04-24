using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MixerFader : MonoBehaviour
{
    [Header("Mixer & экспонированный параметр")]
    public AudioMixer mixer;
    public string exposedParam = "MasterVol";
    
    public void FadeOut (float seconds) => StartCoroutine(FadeTo(-80f, seconds));
    public void FadeIn  (float seconds) => StartCoroutine(FadeTo(  0f, seconds));
    
    IEnumerator FadeTo (float targetDb, float duration)
    {
        if (!mixer.GetFloat(exposedParam, out float startDb))
            yield break;

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float newDb = Mathf.Lerp(startDb, targetDb, t / duration);
            mixer.SetFloat(exposedParam, newDb);
            yield return null;
        }
        mixer.SetFloat(exposedParam, targetDb);
    }
}