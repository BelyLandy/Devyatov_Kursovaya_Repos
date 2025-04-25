using UnityEngine;

public class UnitHit : State
{
    private string animationName = "Hit";
    private float animDuration => unit.GetAnimDuration(animationName);

    public override void Enter()
    {
        unit.StopMoving();

        unit.animator.Play(animationName, 0, 0);
    }

    public override void Update()
    {
        if (unit.GetComponent<HealthController>()?.CurrentHealth == 0) return;

        if (Time.time - stateStartTime > animDuration)
        {
            if (unit.isPlayer) unit.stateMachine.SetState(new PlayerIdle());
            else unit.stateMachine.SetState(new EnemyIdle());
        }
    }
}