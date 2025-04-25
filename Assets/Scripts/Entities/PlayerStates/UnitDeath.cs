using UnityEngine;

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
            
            if(showDeathAnimation) unit.animator.Play(animationName);

            unit.GetComponent<Collider2D>().offset = Vector2.zero;
            unit.transform.position = unit.currentPosition;
            unit.isGrounded = true;
            
            foreach (Transform child in unit.transform)
            {
                child.gameObject.SetActive(false);
            }


            unit.StopMoving(true);


            if(unit.isPlayer) EnemyManager.DisableAllEnemyAI();

            if(unit.isEnemy){
                if (stats != null)
                    stats.AddKill();
                SpriteFlickerAndDestroy flicker = unit.gameObject.AddComponent<SpriteFlickerAndDestroy>();
                flicker.startDelay = 1f;
            }
        }
    }
