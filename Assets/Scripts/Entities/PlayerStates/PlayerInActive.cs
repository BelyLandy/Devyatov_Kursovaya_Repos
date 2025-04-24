using UnityEngine;

namespace CW_Devyatov_238 {

    //State that causes the player to remain inactive, effectively disabling all controls.
    public class PlayerInActive : State {

        private string animationName = "Idle";
    
        public override void Enter(){
            unit.animator.Play(animationName);
            unit.StopMoving();
        }
    }
}
