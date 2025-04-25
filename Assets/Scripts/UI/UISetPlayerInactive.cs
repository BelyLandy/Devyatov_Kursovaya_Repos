using System.Collections;
using CW_Devyatov_238;
using UnityEngine;

public class UISetPlayerInactive : MonoBehaviour
{
    public float startDelay = 3f;

    void OnEnable()
    {
        StartCoroutine(SetPlayerInactive(startDelay));
    }

    IEnumerator SetPlayerInactive(float delay)
    {
        yield return new WaitForSeconds(delay);

        var players = Object.FindObjectsByType<StateMachine>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None);

        foreach (var sm in players)
        {
            if (sm.settings.unitType == UNITTYPE.PLAYER)
                sm.SetState(new PlayerInActive());
        }
    }
}