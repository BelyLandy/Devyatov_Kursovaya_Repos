using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CW_Devyatov_238 {

    [RequireComponent(typeof(SpriteRenderer))]
    public class Projectile : MonoBehaviour {

        public DIRECTION dir = DIRECTION.RIGHT;
        public float speed = 10;
        public float timeToLive = 10f;
        private SpriteRenderer projectileSprite;
        public GameObject hitEffect;
        public AttackData attackData;

        [Header("Sprite Collider Offsets")]
        public Vector2 spriteBoundSizeOffset;
        public Vector2 spriteBoundPositionOffset;
        public bool showHitbox;
        private float startTime;
    
        void Start() {
            projectileSprite = GetComponentInChildren<SpriteRenderer>();
            if (GetComponent<TrailRenderer>() && projectileSprite)
                GetComponent<TrailRenderer>().sortingOrder = projectileSprite.sortingOrder - 1;
            transform.localRotation = Quaternion.Euler(0, dir == DIRECTION.LEFT ? 180 : 0, 0);
            startTime = Time.time;
        }

        void Update() {
            transform.position += Vector3.right * speed * (int)dir * Time.deltaTime;

            GameObject enemyHit = CheckForHit();
            if (enemyHit) {
                enemyHit.GetComponent<HealthController>()?.SubtractHealth(attackData.damage);
                if (attackData.sfx.Length > 0)
                    AudioController.PlaySFX(attackData.sfx);

                UnitActions ua = enemyHit.GetComponent<UnitActions>();
                UnitSettings us = enemyHit.GetComponent<UnitSettings>();

                if (attackData.knockdown && ua?.settings.canBeKnockedDown == true && ua.isGrounded)
                    enemyHit.GetComponent<StateMachine>().SetState(
                        new UnitKnockDown(attackData, us.knockDownDistance, us.knockDownHeight)
                    );

                if (hitEffect)
                    GameObject.Instantiate(hitEffect, transform.position, Quaternion.identity);

                Destroy(gameObject);
            }

            if (Time.time - startTime > timeToLive)
                Destroy(gameObject);
        }

        public GameObject CheckForHit() {
            List<GameObject> hitableObjects = GameObject.FindGameObjectsWithTag("Enemy").ToList();
            hitableObjects = hitableObjects.OrderBy(
                obj => Vector2.Distance(transform.position, obj.transform.position)
            ).ToList();

            foreach (GameObject target in hitableObjects) {
                if (target == null) continue;

                Bounds bound1 = new Bounds(
                    (Vector2)transform.position + spriteBoundPositionOffset,
                    (Vector2)projectileSprite.bounds.size + spriteBoundSizeOffset
                );
                Bounds bound2 = target.GetComponent<SpriteRenderer>().bounds;

                if (showHitbox) {
                    MathUtilities.DrawRectGizmo(bound1.center, bound1.size, Color.red, Time.deltaTime);
                    MathUtilities.DrawRectGizmo(bound2.center, bound2.size, Color.red, Time.deltaTime);
                }

                if (bound1.Intersects(bound2))
                    return target.gameObject;
            }
            return null;
        }

        private void OnDrawGizmos() {
            if (!showHitbox) return;
            if (!projectileSprite) {
                projectileSprite = GetComponentInChildren<SpriteRenderer>();
                return;
            }

            Gizmos.color = Color.red;
            Bounds bound1 = new Bounds(
                (Vector2)transform.position + spriteBoundPositionOffset,
                (Vector2)projectileSprite.bounds.size + spriteBoundSizeOffset
            );
            Gizmos.DrawWireCube(bound1.center, bound1.size);
        }
    }
}