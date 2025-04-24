using CW_Devyatov_238;
using UnityEngine;

public class VisibilitySwitcher : MonoBehaviour
{
    [SerializeField] private GameObject objectToHide;
    
    [SerializeField] private GameObject objectToShow;
    
    public void SwitchVisibility()
    {
        //SoundManager.Instance.PlaySound2D("OpenBook");
        AudioController.PlaySFX("OpenBook");
        if (objectToHide != null)
            objectToHide.SetActive(false);
        if (objectToShow != null)
            objectToShow.SetActive(true);
    }
}