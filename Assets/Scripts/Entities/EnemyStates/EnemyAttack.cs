using UnityEngine;

public class EnemyAttack : State
{
    private string animationName => attack.animationState;
    private float animDuration => unit.GetAnimDuration(animationName);
    private AttackData attack;
    private bool damageDealt;

    public EnemyAttack(AttackData attack)
    {
        this.attack = attack;
    }

    public override void Enter()
    {
        unit.StopMoving();
        unit.TurnToTarget();
        
        if (unit.target && unit.target.GetComponent<HealthController>().IsDead)
            unit.stateMachine.SetState(new EnemyIdle());
        
        unit.animator.Play(animationName);
    }

    public override void Update()
    {
        if (!damageDealt) damageDealt = unit.CheckForHit(attack);
        
        if (Time.time - stateStartTime > animDuration) unit.stateMachine.SetState(new EnemyIdle());
    }
}