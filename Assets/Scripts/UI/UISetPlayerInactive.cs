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
        yield return new WaitForSeconds(startDelay);
        foreach (StateMachine unitStateMachine in
                 GameObject.FindObjectsOfType<StateMachine>())
        {
            if (unitStateMachine.settings.unitType == UNITTYPE.PLAYER)
            {
                unitStateMachine.SetState(new PlayerInActive());
            }
        }
    }
}