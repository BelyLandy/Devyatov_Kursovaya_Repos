using UnityEngine;

public class PlayerJumpAttack : State
{
    private AttackData attackData;
    private bool damageDealt;
    private bool hasLanded;

    public PlayerJumpAttack(AttackData attackData)
    {
        this.attackData = attackData;
    }

    public override void Enter()
    {
        unit.isGrounded = false;
        unit.animator.Play(attackData.animationState);
    }

    public override void Update()
    {
        if (hasLanded)
            unit.stateMachine.SetState(new PlayerLand());

        if (!damageDealt)
            damageDealt = unit.CheckForHit(attackData);
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
