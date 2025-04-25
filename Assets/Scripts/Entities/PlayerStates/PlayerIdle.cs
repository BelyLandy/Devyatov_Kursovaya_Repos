public class PlayerIdle : State
{
    private string animationName = "Idle";

    public override void Enter()
    {
        unit.animator.Play(animationName);
    }

    public override void Update()
    {
        unit.StopMoving(false);

        if (InputManager.DefendKeyDown())
        {
            unit.stateMachine.SetState(new UnitDefend());
            return;
        }

        if (InputManager.JumpKeyDown())
        {
            unit.stateMachine.SetState(new PlayerJump());
            return;
        }

        if (InputManager.PunchKeyDown() && unit.NearbyEnemyDown())
        {
            unit.stateMachine.SetState(new PlayerGroundPunch());
            return;
        }

        if (InputManager.PunchKeyDown())
        {
            unit.stateMachine.SetState(new PlayerAttack(ATTACKTYPE.PUNCH));
            return;
        }

        if (InputManager.GrabKeyDown())
        {
            unit.stateMachine.SetState(new PlayerTryGrab());
            return;
        }

        if (InputManager.GetInputVector().magnitude > 0)
        {
            unit.stateMachine.SetState(new PlayerMove());
        }
    }
}