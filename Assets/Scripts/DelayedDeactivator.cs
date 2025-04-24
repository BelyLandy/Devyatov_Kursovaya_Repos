using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DelayedDeactivator : MonoBehaviour
{
    [SerializeField] private List<GameObject> targets = new List<GameObject>();
    [SerializeField] private float delay = 1f;

    [SerializeField] private bool doOnStart = false;
    
    private void Start()
    {
        if (doOnStart)
        {
            if (targets == null) return;
            StartCoroutine(DeactivateAfterDelay());
        }
    }

    public void DeactivateDelayed()
    {
        if (targets == null) return;
        StartCoroutine(DeactivateAfterDelay());
    }

    private IEnumerator DeactivateAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        foreach (var _gameObj in targets)
        {
            _gameObj.SetActive(false);
        }
    }
}