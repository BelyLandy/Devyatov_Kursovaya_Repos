public class EnemyIdle : State
{
    private string animationName = "Idle";

    public override void Enter()
    {
        unit.StopMoving();
        unit.animator.Play(animationName);

        if (!unit.target) unit.target = unit.findClosestPlayer();

        if (unit.targetSpotted) unit.TurnToTarget();
    }
}