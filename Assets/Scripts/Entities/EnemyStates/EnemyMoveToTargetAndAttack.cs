using UnityEngine;

public class EnemyMoveToTargetAndAttack : State
{
    private string animationName = "Run";
    private Vector2 maxAttackRange = new Vector2(1.2f, .1f);
    private float attackDistance = 1f;
    private AttackData attack;
    private float pauseBeforeAttack;

    public EnemyMoveToTargetAndAttack(AttackData attack)
    {
        this.attack = attack;
    }

    public override void Enter()
    {
        if (!unit.target)
            unit.stateMachine.SetState(new EnemyIdle());
        pauseBeforeAttack = unit.settings.enemyPauseBeforeAttack;
        unit.TurnToTarget();
    }

    public override void Update()
    {
        if (targetInRange())
        {
            unit.StopMoving();
            unit.animator.Play("Idle");

            if (pauseBeforeAttack > 0)
            {
                pauseBeforeAttack -= Time.deltaTime;
                return;
            }

            unit.stateMachine.SetState(
                attack != null ? new EnemyAttack(attack) : new EnemyIdle()
            );
        }
    }

    public override void FixedUpdate()
    {
        bool targetIsGrounded = unit.target.GetComponent<UnitActions>().isGrounded;
        if ((unit.distanceToTarget().y > maxAttackRange.y && targetIsGrounded)
            || unit.distanceToTarget().x > maxAttackRange.x)
        {
            Vector2 idealPos = getIdealAttackPos();
            Vector2 dirToPos = (idealPos - (Vector2)unit.transform.position).normalized;

            Vector2 wallDistanceCheck = unit.col2D
                ? (unit.col2D.size / 1.6f) * 1.1f
                : Vector2.one * .3f;
            if (unit.WallDetected(dirToPos * wallDistanceCheck))
            {
                unit.stateMachine.SetState(new EnemyIdle());
                return;
            }

            unit.MoveToVector(dirToPos, unit.settings.moveSpeed);
            unit.animator.Play(animationName);
        }
    }

    Vector2 getIdealAttackPos()
    {
        Vector2 XDirToTarget = unit.target.transform.position.x > unit.transform.position.x
            ? Vector2.right
            : Vector2.left;
        return unit.target.GetComponent<UnitActions>().currentPosition
               - XDirToTarget * attackDistance;
    }

    bool targetInRange()
    {
        return unit.distanceToTarget().x < maxAttackRange.x
               && unit.distanceToTarget().y < maxAttackRange.y;
    }
}