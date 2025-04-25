using UnityEngine;

public class EnemyMoveTo : State
{
    private string animationName = "Run";
    private Vector2 destination;

    public EnemyMoveTo(Vector2 pos)
    {
        destination = pos;
    }

    public override void FixedUpdate()
    {
        Vector2 unitPos = unit.GetComponent<UnitActions>().currentPosition;
        Vector2 moveDir = (destination - unitPos).normalized;


        Vector2 wallDistanceCheck = unit.col2D ? (unit.col2D.size / 1.8f) * 1.1f : Vector2.one * .3f;
        if (unit.EnemyWallDetected(moveDir * wallDistanceCheck))
        {
            unit.stateMachine.SetState(new EnemyIdle());
            return;
        }

        unit.MoveToVector(moveDir, unit.settings.moveSpeed);
        unit.animator.Play(animationName);

        if (Vector2.Distance(unitPos, destination) < .1f) unit.stateMachine.SetState(new EnemyIdle());
    }
}