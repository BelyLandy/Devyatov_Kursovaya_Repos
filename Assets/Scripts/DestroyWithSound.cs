using UnityEngine;

public class DestroyWithSound : MonoBehaviour
{
    public AudioClip destroySound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void DestroyObject()
    {
        audioSource.PlayOneShot(destroySound);
        Destroy(gameObject, destroySound.length);
    }
}
