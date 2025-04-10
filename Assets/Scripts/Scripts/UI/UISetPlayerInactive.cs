using System.Collections;
using UnityEngine;

namespace CW_Devyatov_238 {

    public class UISetPlayerInactive : MonoBehaviour {

        public float startDelay = 3f;

         void OnEnable() {
                StartCoroutine(SetPlayerInactive(startDelay));
            }

        //Set all player(s) to Inactive state
        IEnumerator SetPlayerInactive(float delay){
            yield return new WaitForSeconds(startDelay);

            // ReSharper disable once ArrangeStaticMemberQualifier
            foreach(StateMachine unitStateMachine in GameObject.FindObjectsOfType<StateMachine>()){
                if(unitStateMachine.settings.unitType == UNITTYPE.PLAYER){
                    unitStateMachine.SetState(new PlayerInActive());
                }
            }
        }
    }
}