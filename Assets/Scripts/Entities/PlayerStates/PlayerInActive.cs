using UnityEngine;

namespace CW_Devyatov_238 {

    public class PlayerInActive : State {

        private string animationName = "Idle";
    
        public override void Enter(){
            unit.animator.Play(animationName);
            unit.StopMoving();
        }
    }
}
