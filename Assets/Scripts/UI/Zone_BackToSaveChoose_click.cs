using UnityEngine;

public class Zone_BackToSaveChoose_click : MonoBehaviour
{
    [SerializeField] private ScaleAnimator _scaleAnimator;

    public void DoAction()
    {
        InkwellClick._isFeatherPickedUp = false;
        
        _scaleAnimator.PlayScaleDown(true, SceneLoader.Scene.ChooseSaveFile);
    }
}
