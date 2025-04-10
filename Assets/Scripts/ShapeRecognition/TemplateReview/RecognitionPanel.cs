using System;
using UnityEngine;
using UnityEngine.UI;

public class RecognitionPanel : MonoBehaviour
{
    [SerializeField] private Button _dollar1;

    public void Initialize(Action<int> onAlgorithmChoose)
    {
        SetupButtons(0);
        _dollar1.onClick.AddListener(() =>
        {
            SetupButtons(0);
            onAlgorithmChoose.Invoke(0);
        });
    }

    private void SetupButtons(int choose)
    {
        _dollar1.image.color = choose == 0 ? Color.green : Color.white;
    }
}