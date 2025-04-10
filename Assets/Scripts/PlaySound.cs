using UnityEngine;

public class PlaySound : MonoBehaviour, IClickable
{
    [SerializeField] 
    private string soundName;

    [SerializeField]
    private bool useSecondSound = false;

    [SerializeField] 
    private string secondSoundName;
    
    private bool playSecondSound = false;
    
    public void OnClick()
    {
        if (useSecondSound)
        {
            if (playSecondSound)
            {
                SoundManager.Instance.PlaySound2D(secondSoundName);
            }
            else
            {
                SoundManager.Instance.PlaySound2D(soundName);
            }
            playSecondSound = !playSecondSound;
        }
        else
        {
            SoundManager.Instance.PlaySound2D(soundName);
        }
    }
}