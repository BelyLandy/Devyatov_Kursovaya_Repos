using UnityEngine;

public class PlayerJump : State
{
    private string animationName = "Jump";
    private string sfxName = "JumpUp";
    private bool hasLanded;

    public override void Enter()
    {
        unit.StopMoving(true);
        unit.animator.Play(animationName);
        unit.groundPos = unit.transform.position.y;
        unit.isGrounded = false;
        unit.yForce = unit.settings.jumpHeight;
        AudioController.PlaySFX(sfxName, unit.transform.position);
    }

    public override void Update()
    {
        if (InputManager.PunchKeyDown())
        {
            unit.stateMachine.SetState(new PlayerJumpAttack(unit.settings.jumpPunch));
            return;
        }

        if (hasLanded)
            unit.stateMachine.SetState(new PlayerLand());
    }

    public override void FixedUpdate()
    {
        unit.JumpSequence();

        bool jumpFinished = unit.transform.position.y < unit.groundPos;

        if (jumpFinished)
        {
            unit.GetComponent<Collider2D>().offset = Vector2.zero;
            unit.transform.position = new Vector3(unit.transform.position.x, unit.groundPos, 0);
            unit.isGrounded = true;
            hasLanded = true;
        }
    }
}