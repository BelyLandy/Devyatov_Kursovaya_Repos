using System.Collections.Generic;
using UnityEngine;

public class ObjectEnablerOrDisabler : MonoBehaviour
{
    [SerializeField] private List<GameObject> _gameObjects = new List<GameObject>();

    public void DoEnable()
    {
        foreach (var _gmObj in _gameObjects)
        {
            _gmObj.SetActive(true);
        }
    }
    
    public void DoDisable()
    {
        foreach (var _gmObj in _gameObjects)
        {
            _gmObj.SetActive(false);
        }
    }

}
