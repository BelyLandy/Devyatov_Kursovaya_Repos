using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayScriptActivator : MonoBehaviour
{
    [System.Serializable]
    public class TargetInfo
    {
        [Tooltip("Компонент, который нужно включить")]
        public MonoBehaviour script;

        [Tooltip("Сколько секунд ждать перед включением")]
        public float delay = 1f;
    }

    [Tooltip("Список целей. Можно задать любое количество.")]
    public List<TargetInfo> targets = new List<TargetInfo>();

    private void Start()
    {
        foreach (TargetInfo t in targets)
        {
            if (t.script == null) continue;

            t.script.enabled = false;

            StartCoroutine(EnableAfterDelay(t));
        }
    }

    private IEnumerator EnableAfterDelay(TargetInfo t)
    {
        yield return new WaitForSeconds(t.delay);
        t.script.enabled = true;
    }
}