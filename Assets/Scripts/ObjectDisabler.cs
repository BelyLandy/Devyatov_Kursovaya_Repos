using UnityEngine;

public class ObjectDisabler : MonoBehaviour
{
    [SerializeField] private GameObject _gameObject;

    public void DoDisable()
    {
        _gameObject.SetActive(false);
    }
}
