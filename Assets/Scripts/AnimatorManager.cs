using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    private Animator _animator;
    private string currentState;
    
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;
        
        _animator.Play(newState);

        currentState = newState;
    }
}
