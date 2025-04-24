using UnityEngine;

namespace CW_Devyatov_238 {

    public class UnitDeath : State {

        private const string KillStatsPath = "KillStatsSO";
        private static KillStatsSO stats;  
        
        private string animationName = "PlayerDeath";
        private bool showDeathAnimation;

        public UnitDeath(bool showDeathAnim){
            this.showDeathAnimation = showDeathAnim;
            
            if (stats == null)
                stats = Resources.Load<KillStatsSO>(KillStatsPath);
        }

        public override void Enter(){
            
            //play death animation
            if(showDeathAnimation) unit.animator.Play(animationName);

            //set this unit on the floor
            unit.GetComponent<Collider2D>().offset = Vector2.zero;
            unit.transform.position = unit.currentPosition;
            unit.isGrounded = true;
            
            foreach (Transform child in unit.transform)
            {
                child.gameObject.SetActive(false);
            }

            //stop moving
            unit.StopMoving(true);

            //disable all enemy AI if a player has died
            if(unit.isPlayer) EnemyManager.DisableAllEnemyAI();

            //flicker and remove enemy units from the field
            if(unit.isEnemy){
                if (stats != null)
                    stats.AddKill();
                SpriteFlickerAndDestroy flicker = unit.gameObject.AddComponent<SpriteFlickerAndDestroy>();
                flicker.startDelay = 1f;
            }
        }
    }
}