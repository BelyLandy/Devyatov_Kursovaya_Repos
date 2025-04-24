using UnityEngine;
using UnityEngine.Events;

public class DelayedInvoker : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float delay = 1f;
    [SerializeField] private UnityEvent onInvoke;
    
    public void InvokeDelayed()
    {
        StartCoroutine(InvokeAfterDelay(delay));
    }
    
    public void InvokeDelayed(float customDelay)
    {
        StartCoroutine(InvokeAfterDelay(customDelay));
    }
    
    private System.Collections.IEnumerator InvokeAfterDelay(float d)
    {
        if (d > 0f)
            yield return new WaitForSeconds(d);

        onInvoke?.Invoke();
    }
}