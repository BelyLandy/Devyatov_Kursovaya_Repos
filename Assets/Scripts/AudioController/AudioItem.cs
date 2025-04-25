using UnityEngine;

[System.Serializable]
public class AudioItem
{
    public string name;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0f, 1f)] public float randomVolume = 0f;
    [Range(0f, 1f)] public float randomPitch = 0f;
    public float minTimeBetweenCall = .1f;
    public float range = 0f;
    public bool loop;
    public AudioClip[] clip;
    [HideInInspector] public float lastTimePlayed = 0;
}