using UnityEngine;

namespace CW_Devyatov_238 {

    public class UnitKnockDownGrounded : State {

        private string animationName = "KnockDownGrounded";
        public override bool canGrab => false; //cannot be grabbed in this state
   
        public override void Enter(){
            unit.StopMoving();
            unit.animator.Play(animationName, 0, 0);
            unit.isGrounded = true;
        }

        public override void Update(){
            
            //go to death state if there is no health left
            HealthController hs = unit.GetComponent<HealthController>();
            if(hs != null && hs.IsDead)  unit.stateMachine.SetState(new UnitDeath(false)); 

            //stand up
            if(Time.time - stateStartTime > unit.settings.knockDownFloorTime) unit.stateMachine.SetState(new UnitStandUp());
        }
    }
}
