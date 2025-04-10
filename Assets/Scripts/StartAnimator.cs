using System.Collections;
using UnityEngine;

public class StartAnimator : MonoBehaviour
{
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();

        _animator.Play(0);
    }
    
}
