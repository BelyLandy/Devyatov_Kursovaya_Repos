using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UnitActions : MonoBehaviour
{
    [HideInInspector] public GameObject target;
    [HideInInspector] public float groundPos = 0;
    [HideInInspector] public Vector2 currentPosition => new Vector2(transform.position.x, groundPos);
    [HideInInspector] public float lastAttackTime = 0;
    [HideInInspector] public ATTACKTYPE lastAttackType;
    [HideInInspector] public float yForce = 0;
    [HideInInspector] public bool isGrounded = true;
    [HideInInspector] public bool targetSpotted;
    [HideInInspector] public CapsuleCollider2D col2D;
    [HideInInspector] public List<ATTACKTYPE> attackList = new List<ATTACKTYPE>();

    public Animator animator => GetComponent<Animator>();
    public StateMachine stateMachine => GetComponent<StateMachine>();
    public UnitSettings settings => GetComponent<UnitSettings>();
    public Rigidbody2D rb => GetComponent<Rigidbody2D>();
    public bool isPlayer => settings?.unitType == UNITTYPE.PLAYER;
    public bool isEnemy => settings?.unitType == UNITTYPE.ENEMY;
    private bool onApplicationQuit;
    private float currentSpeed = 0f;
    private float animDuration;
    public DIRECTION dir => transform.localRotation == Quaternion.Euler(Vector3.zero) ? DIRECTION.RIGHT : DIRECTION.LEFT;
    public DIRECTION invertedDir => (DIRECTION)((int)dir * -1);

    public delegate void OnUnitDealDamage(GameObject recipient, AttackData attackData);
    public static event OnUnitDealDamage onUnitDealDamage;

    void OnDestroy()
    {
        if (settings?.shadow && !onApplicationQuit)
            Destroy(settings.shadow);
    }

    public GameObject findClosestPlayer()
    {
        List<GameObject> allPlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
        allPlayers = allPlayers.OrderBy(player => Vector2.Distance(transform.position, player.transform.position)).ToList();
        if (allPlayers.Count > 0) return allPlayers[0];
        return null;
    }

    public Vector2 distanceToTarget()
    {
        if (!target) return Vector2.positiveInfinity;
        return new Vector2(
            Mathf.Abs(target.transform.position.x - transform.position.x),
            Mathf.Abs(target.transform.position.y - transform.position.y)
        );
    }

    public void TurnToTarget()
    {
        if (!target) return;

        bool toLeft = target.transform.position.x < transform.position.x;
        transform.localRotation = toLeft
            ? Quaternion.Euler(0f, 180f, 0f)
            : Quaternion.identity;

        foreach (Transform child in transform)
        {
            if (child.CompareTag("HealthBar"))
            {
                child.localRotation = toLeft
                    ? Quaternion.Euler(0f, 180f, 0f)
                    : Quaternion.identity;

                if (!toLeft)
                {
                    Vector3 p = child.localPosition;
                    p.x = -Mathf.Abs(p.x);
                    child.localPosition = p;
                }
                else
                {
                    Vector3 p = child.localPosition;
                    p.x = Mathf.Abs(p.x);
                    child.localPosition = p;
                }

                break;
            }
        }
    }

    public void TurnToDir(DIRECTION dir)
    {
        bool toLeft = dir == DIRECTION.LEFT;
        transform.localRotation = toLeft
            ? Quaternion.Euler(0f, 180f, 0f)
            : Quaternion.identity;

        foreach (Transform child in transform)
        {
            if (child.CompareTag("HealthBar"))
            {
                child.localRotation = toLeft
                    ? Quaternion.Euler(0f, 180f, 0f)
                    : Quaternion.identity;

                if (!toLeft)
                {
                    Vector3 p = child.localPosition;
                    p.x = -Mathf.Abs(p.x);
                    child.localPosition = p;
                }
                else
                {
                    Vector3 p = child.localPosition;
                    p.x = Mathf.Abs(p.x);
                    child.localPosition = p;
                }

                break;
            }
        }
    }

    public void TurnToFloatDir(float x)
    {
        if (x == 0) return;
        if (x > 0) TurnToDir(DIRECTION.RIGHT);
        else TurnToDir(DIRECTION.LEFT);
    }

    public bool CheckForHit(AttackData attackData)
    {
        bool damageDealt = false;
        if (attackData.inflictor == null) attackData.inflictor = gameObject;
        if (HitBoxActive())
        {
            foreach (GameObject obj in GetObjectsHit(attackData))
            {
                UnitActions targetUnit = obj.GetComponent<UnitActions>();
                if (targetUnit?.IsDefending(invertedDir) == true)
                {
                    damageDealt = true;
                    continue;
                }

                bool unitKnockdownInProgress = obj.GetComponent<StateMachine>()?.GetCurrentState() is UnitKnockDown;
                if (unitKnockdownInProgress) continue;

                ShowHitEffectAtPosition(settings.hitBox.transform.position + (Vector3.right * Random.Range(0, .5f)));

                HealthController targetHealthController = obj.GetComponent<HealthController>();
                if (attackData != null) targetHealthController?.SubtractHealth(attackData.damage);

                if (attackData.sfx.Length > 0) AudioController.PlaySFX(attackData.sfx);

                onUnitDealDamage?.Invoke(obj, attackData);

                if (targetUnit != null && targetUnit.isGrounded)
                {
                    if (targetHealthController.IsDead)
                    {
                        obj.GetComponent<StateMachine>().SetState(new UnitDeath(true));
                    }
                    else
                    {
                        bool doAKnockdown = attackData.knockdown && targetUnit.settings.canBeKnockedDown;
                        if (!doAKnockdown)
                        {
                            targetUnit.stateMachine.SetState(new UnitHit());
                        }
                        else
                        {
                            Vector2 knockDownForce = new Vector2(
                                targetUnit.settings.knockDownDistance,
                                targetUnit.settings.knockDownHeight
                            );
                            targetUnit.stateMachine.SetState(new UnitKnockDown(attackData, knockDownForce.x, knockDownForce.y));
                        }
                    }
                }
                damageDealt = true;
            }
        }
        return damageDealt;
    }

    public List<GameObject> GetObjectsHit(AttackData attackData)
    {
        List<GameObject> hitableObjects = new List<GameObject>();
        List<GameObject> ObjectsHit = new List<GameObject>();

        if (isPlayer)
        {
            hitableObjects = GameObject.FindGameObjectsWithTag("Enemy").ToList();
            hitableObjects.AddRange(GameObject.FindGameObjectsWithTag("Object").ToList());
        }

        if (isEnemy)
        {
            bool enemyIsBeingThrown = attackData.attackType == ATTACKTYPE.GRABTHROW;
            bool enemyDoesFallDamage = settings.hitOtherEnemiesWhenFalling;

            if (!enemyIsBeingThrown) hitableObjects = GameObject.FindGameObjectsWithTag("Player").ToList();

            if (enemyIsBeingThrown || enemyDoesFallDamage)
            {
                foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    bool enemyIsMyself = enemy == gameObject;
                    bool enemyIsKnockedDown = enemy.GetComponent<StateMachine>()?.GetCurrentState() is UnitKnockDown || enemy.GetComponent<StateMachine>()?.GetCurrentState() is UnitKnockDownGrounded;
                    if (!enemyIsMyself && !enemyIsKnockedDown) hitableObjects.Add(enemy);
                }
            }
        }

        for (int i = hitableObjects.Count - 1; i >= 0; i--)
        {
            if (hitableObjects[i].GetComponent<HealthController>()?.IsDead == true) hitableObjects.RemoveAt(i);
        }

        for (int i = hitableObjects.Count - 1; i >= 0; i--)
        {
            if (hitableObjects[i].GetComponent<StateMachine>()?.GetCurrentState() is UnitHit) hitableObjects.RemoveAt(i);
        }

        hitableObjects = hitableObjects.OrderBy(obj => Vector2.Distance(transform.position, obj.transform.position)).ToList();

        foreach (GameObject obj in hitableObjects)
        {
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null && settings.hitBox.bounds.Intersects(sr.bounds) && targetInZRange(obj, .5f))
                ObjectsHit.Add(sr.gameObject);
        }

        return ObjectsHit;
    }

    public GameObject GetClosestPickup(Vector2 pickupRange)
    {
        List<GameObject> allPickups = GameObject.FindGameObjectsWithTag("Pickup").ToList()
            .OrderBy(pickup => Vector2.Distance(transform.position, pickup.transform.position)).ToList();
        if (allPickups.Count == 0) return null;

        foreach (var pickup in allPickups)
        {
            if (Vector2.Distance(transform.position, pickup.transform.position) <= pickupRange.magnitude)
                return pickup;
        }
        return null;
    }

    public GameObject NearbyEnemyDown()
    {
        if (EnemyManager.enemyList.Count == 0) return null;
        float range = 1;
        foreach (GameObject enemy in EnemyManager.enemyList)
        {
            if (enemy && Vector2.Distance(transform.position, enemy.transform.position) < range)
            {
                if (enemy.GetComponent<HealthController>()?.IsDead == true) continue;

                StateMachine targetStateMachine = enemy.GetComponent<StateMachine>();
                if (targetStateMachine?.GetCurrentState() is UnitKnockDownGrounded) return enemy;
            }
        }
        return null;
    }

    bool targetInZRange(GameObject target, float zRange)
    {
        return Mathf.Abs(target.transform.position.y - groundPos) < zRange;
    }

    public void MoveToVector(Vector2 moveDir, float moveSpeed)
    {
        if (rb == null) return;
        if (isGrounded) groundPos = transform.position.y;

        if (settings.useAcceleration)
        {
            if (moveDir.magnitude > 0.1f)
                currentSpeed = Mathf.Min(currentSpeed + settings.moveAcceleration * Time.fixedDeltaTime, moveSpeed);
        }
        else
        {
            if (moveDir.magnitude > 0.1f) currentSpeed = settings.moveSpeed;
        }

        rb.linearVelocity = moveDir * currentSpeed;
        if (col2D == null) col2D = GetComponent<CapsuleCollider2D>();
        if (moveDir.x != 0) TurnToDir(moveDir.x > 0 ? DIRECTION.RIGHT : DIRECTION.LEFT);
    }

    public bool WallDetected(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Linecast(currentPosition, currentPosition + dir, 1 << LayerMask.NameToLayer("Environment"));
        Debug.DrawRay(currentPosition, dir, Color.yellow, Time.deltaTime);
        return hit.collider != null;
    }

    public bool EnemyWallDetected(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Linecast(currentPosition, currentPosition + dir, 1 << LayerMask.NameToLayer("Obstacles"));
        Debug.DrawRay(currentPosition, dir, Color.yellow, Time.deltaTime);
        return hit.collider != null;
    }

    public void AddForce(float force)
    {
        StartCoroutine(AddForceRoutine(force, .25f));
    }

    private IEnumerator AddForceRoutine(float force, float duration)
    {
        Vector2 startPos = transform.position;
        Vector2 endPos = startPos + Vector2.right * (int)dir * force;
        float t = 0;
        while (t < 1)
        {
            transform.position = Vector2.Lerp(startPos, endPos, MathUtilities.Sinerp(t));
            t += Time.deltaTime / duration;
            yield return 0;
        }
        transform.position = endPos;
    }

    public void JumpSequence()
    {
        Vector2 moveVector = transform.position;

        float inputVectorX = InputManager.GetInputVector().x;
        if (inputVectorX != 0) TurnToDir(inputVectorX > 0 ? DIRECTION.RIGHT : DIRECTION.LEFT);
        moveVector.x = transform.position.x + (inputVectorX * settings.moveSpeedAir * Time.fixedDeltaTime);

        moveVector.y += yForce * Time.fixedDeltaTime * settings.jumpSpeed;
