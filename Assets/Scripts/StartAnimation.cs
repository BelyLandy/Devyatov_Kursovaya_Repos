using UnityEngine;

//[RequireComponent(typeof(Animation))]
public class StartAnimation : MonoBehaviour
{
    [SerializeField]
    private Animation _animation;
    
    [SerializeField]
    private AnimationClip[] clips;
    
    public void StartAnim()
    {
        _animation.Play();
    }
    
    public void StartAnim(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
        {
            Debug.LogWarning("StartAnim called with empty clipName. Playing default animation.");
            _animation.Play();
            return;
        }

        if (_animation.GetClip(clipName) == null)
        {
            Debug.LogWarning($"Clip '{clipName}' not found in Animation component.");
            return;
        }

        _animation.Play(clipName);
    }
    
    public void PlayClipByIndex(int index)
    {
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning("No clips assigned in the inspector.");
            return;
        }

        if (index < 0 || index >= clips.Length)
        {
            Debug.LogWarning($"Clip index {index} out of range (0..{clips.Length - 1}).");
            return;
        }

        var clip = clips[index];
        if (_animation.GetClip(clip.name) == null)
        {
            _animation.AddClip(clip, clip.name);
        }
        _animation.Play(clip.name);
    }

    public void EnableOrDisableObj(string doEnabled = "")
    {
        if (doEnabled == "true")
        {
            GameObject owner = _animation.gameObject;
            
            owner.SetActive(true);
        }
        else if (doEnabled == "false")
        {
            GameObject owner = _animation.gameObject;
            
            owner.SetActive(false);
        }
    }
}