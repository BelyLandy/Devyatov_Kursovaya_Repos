using UnityEngine;

public class GoToGameAfter : MonoBehaviour
{

    public void DoAfter()
    {
        InkwellClick._isFeatherPickedUp = false;
        
        foreach (GameObject inkwell in GameObject.FindGameObjectsWithTag("Inkwell"))
        {
            inkwell.SetActive(false);
        }
    }
}
