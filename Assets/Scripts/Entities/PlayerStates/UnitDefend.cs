using UnityEngine;

    public class UnitDefend : State {

        private string animationName = "Defend";
        private string defendHitSFX = "DefendHit";
        private float enemyDefendDuration => unit.settings.defendDuration;
    
        public override void Enter(){
            unit.animator.Play(animationName);
            unit.StopMoving();
        }

        public override void Update(){


            if(unit.isPlayer){


                if(unit.isPlayer && unit.settings.canChangeDirWhileDefending){
                    Vector2 inputVector = InputManager.GetInputVector();
                    if(inputVector.x == 1) unit.TurnToDir(DIRECTION.RIGHT);
                    else if(inputVector.x == -1) unit.TurnToDir(DIRECTION.LEFT);
                }


                if(!InputManager.DefendKeyDown()) unit.stateMachine.SetState(new PlayerIdle());
            }


            if(unit.isEnemy && (Time.time - stateStartTime > enemyDefendDuration)) unit.stateMachine.SetState(new EnemyIdle());
        }

        public void Hit(){

            unit.ShowEffect("DefendEffect"); 

            AudioController.PlaySFX(defendHitSFX, unit.transform.position);
        }
    }

