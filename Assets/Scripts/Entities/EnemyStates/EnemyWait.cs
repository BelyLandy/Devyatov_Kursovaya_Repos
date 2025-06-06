﻿using UnityEngine;

namespace CW_Devyatov_238 {

    public class EnemyWait : State {

        private string animationName = "Idle";
        private float timeToWait;


        public EnemyWait(float timeToWait){
            this.timeToWait = timeToWait;
        }

        public override void Enter(){
            unit.StopMoving(true);
            unit.TurnToTarget();
            unit.animator.Play(animationName);
        }

         public override void Update(){

            if(Time.time - stateStartTime > timeToWait) unit.stateMachine.SetState(new EnemyIdle()); 
        }
    }
}