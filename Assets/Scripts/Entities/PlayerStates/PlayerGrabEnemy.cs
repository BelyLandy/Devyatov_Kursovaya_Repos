using UnityEngine;

public class PlayerGrabEnemy : State
{
    private GameObject enemy;

    public PlayerGrabEnemy(GameObject enemy)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        Vector2 grabPos =
            new Vector2(unit.settings.grabPosition.x * (int)unit.dir,
                unit.settings.grabPosition.y);
        Vector2 enemyGrabPos = (Vector2)unit.transform.position + grabPos;

        enemy.GetComponent<StateMachine>()?.SetState(new EnemyGrabbed(unit.gameObject, enemyGrabPos));
    }

    public override void Update()
    {
        unit.groundPos = unit.transform.position.y;

        if (InputManager.PunchKeyDown())
        {
            unit.stateMachine.SetState(new PlayerGrabAttack(unit.settings.grabPunch));
            enemy = null;
            return;
        }

        if (InputManager.GrabKeyDown())
        {
            unit.stateMachine.SetState(new PlayerThrowEnemy(enemy));
            enemy = null;
            return;
        }

        if (Time.time - stateStartTime > unit.settings.grabDuration)
        {
            unit.stateMachine.SetState(new PlayerIdle()); //player return to Idle
            enemy = null;
        }
    }

    public override void Exit()
    {
        if (enemy != null) enemy.GetComponent<StateMachine>()?.SetState(new EnemyIdle());
    }
}