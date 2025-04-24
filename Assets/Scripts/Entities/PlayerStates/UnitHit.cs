﻿using UnityEngine;

namespace CW_Devyatov_238 {

    public class UnitHit : State {

        private string animationName = "Hit";
        private float animDuration => unit.GetAnimDuration(animationName);

        public override void Enter(){
            unit.StopMoving();

            /*//lose equipped weapon when hit
            if(unit.settings.loseWeaponWhenHit && unit.weapon != null){
                unit.GetComponentInChildren<WeaponAttachment>()?.LoseCurrentWeapon();
            }*/

            //play hit animation
            unit.animator.Play(animationName, 0, 0);
        }

        public override void Update(){
            if(unit.GetComponent<HealthController>()?.CurrentHealth == 0) return;

            if(Time.time - stateStartTime > animDuration){
                if(unit.isPlayer) unit.stateMachine.SetState(new PlayerIdle());
                else unit.stateMachine.SetState(new EnemyIdle());
            }
        }
    }
}
