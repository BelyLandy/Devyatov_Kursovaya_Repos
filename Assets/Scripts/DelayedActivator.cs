using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DelayedActivator : MonoBehaviour
{
    [SerializeField] private List<GameObject> targets = new List<GameObject>();
    [SerializeField] private float delay = 1f;   
    
    public void ActivateDelayed()
    {
        if (targets == null) return;
        StartCoroutine(ActivateAfterDelay());
    }

    private IEnumerator ActivateAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        foreach (var _gameObj in targets)
        {
            _gameObj.SetActive(true);
        }
    }
}