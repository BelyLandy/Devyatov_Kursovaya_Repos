using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; }

    public AudioItem[] AudioClipArray;

    private AudioSource source;

    private float clipVol = 1f;

    private static AudioController controller;

    void Awake()
    {
        source = GetComponent<AudioSource>();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Найдено несколько AudioController'ов в сцене :(. А может быть только один.");
            Destroy(gameObject);
        }
    }

    public static void PlaySFX(string name = "", Vector3? pos = null, Transform parent = null)
    {
        if (string.IsNullOrEmpty(name)) return;

        Vector3 finalPos = pos ?? Camera.main.transform.position;

        Transform finalParent = parent ?? Camera.main.transform;

        if (Instance) Instance.PlayAudioItem(name, finalPos, finalParent);
    }

    public static float GetSFXDuration(string name)
    {
        foreach (AudioItem audioItem in Instance.AudioClipArray)
        {
            if (audioItem.name == name) return audioItem.clip.Length;
        }

        return 0;
    }

    private void PlayAudioItem(string name, Vector3 worldPosition, Transform parent)
    {
        if (name.Length == 0)
        {
            return;
        }
        bool SFXFound = false;
        
        foreach (AudioItem audioItem in AudioClipArray)
        {
            if (audioItem.name == name)
            {
                if (audioItem.clip.Length == 0)
                {
                    Debug.Log("Аудиоклип '" + name + "' не найден!");
                    return;
                }
                
                if (Time.time - audioItem.lastTimePlayed < audioItem.minTimeBetweenCall)
                {
                    return;
                }
                else
                {
                    audioItem.lastTimePlayed = Time.time;
                }

                GameObject audioObj = new GameObject("AudioObj_" + name);
                audioObj.name = name;
                audioObj.transform.parent = audioItem.range == 0 ? Camera.main.transform : parent;
                audioObj.transform.position = worldPosition;
                
                AudioSource audiosource = audioObj.AddComponent<AudioSource>();
                
                int rand = Random.Range(0, audioItem.clip.Length);
                
                audiosource.clip = audioItem.clip[rand];
                
                audiosource.spatialBlend = 1.0f;
                
                audiosource.pitch = 1f + Random.Range(-audioItem.randomPitch, audioItem.randomPitch);
                
                audiosource.volume = audioItem.volume * clipVol + Random.Range(-audioItem.randomVolume, audioItem.randomVolume);
                audiosource.outputAudioMixerGroup = source.outputAudioMixerGroup;
                audiosource.rolloffMode = AudioRolloffMode.Custom;
                audiosource.loop = audioItem.loop;
                
                if (audioItem.range > 0)
                {
                    audiosource.maxDistance = audioItem.range; 
                }

                if (audioItem.range > 3)
                {
                    audiosource.minDistance = audiosource.maxDistance - 3;
                }
                
                audiosource.Play();

                if (!audioItem.loop && audiosource.clip != null)
                    Destroy(audioObj, audiosource.clip.length + Time.deltaTime);
                SFXFound = true;
            }
        }

        if (!SFXFound) Debug.Log("Нет в списке клипа с таким названием как - " + name);
    }
}