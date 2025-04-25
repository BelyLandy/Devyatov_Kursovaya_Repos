using UnityEngine;

public class UnitKnockDown : State
{
    private string animationNameUp = "KnockDown Up";
    private string animationNameDown = "KnockDown Down";
    public override bool canGrab => false;
    private AttackData attackData;
    private Collider2D col2D;
    private int bounceNum = 1;
    private float xForce;
    private float yForce;
    private bool fallDamageApplied;
    private bool hasHitFloor;

    public UnitKnockDown(AttackData _attackData, float xForce, float yForce)
    {
        attackData = _attackData;
        this.xForce = xForce;
        this.yForce = yForce;
    }

    public override void Enter()
    {
        col2D = unit.GetComponent<Collider2D>();
        unit.StopMoving();
        unit.isGrounded = false;
        if (attackData.inflictor != null && attackData.inflictor.CompareTag("Player"))
        {
            var dir = attackData.inflictor.GetComponent<UnitActions>().dir;
            unit.TurnToDir((DIRECTION)((int)dir * -1));
        }
    }

    public override void Update()
    {
        Vector2 moveVector = unit.transform.position;
        moveVector.x = unit.transform.position.x - (int)unit.dir * xForce * Time.deltaTime;
        xForce = xForce > 0 ? xForce - Time.deltaTime : 0;

        bool goingDown = yForce < 0;
        bool hurtThrown = unit.settings.hitOtherEnemiesWhenThrown && attackData.attackType == ATTACKTYPE.GRABTHROW;
        bool hurtFalling = unit.settings.hitOtherEnemiesWhenFalling;
        if ((!hasHitFloor && goingDown && hurtThrown) || hurtFalling)
            unit.CheckForHit(attackData);

        if (unit.transform.position.y < unit.groundPos)
        {
            hasHitFloor = true;
            if (!fallDamageApplied && attackData.attackType == ATTACKTYPE.GRABTHROW)
            {
                unit.GetComponent<HealthController>()?.SubtractHealth(attackData.damage);
                fallDamageApplied = true;
            }
            if (bounceNum > 0)
            {
                unit.transform.position = new Vector3(unit.transform.position.x, unit.groundPos, 0);
                yForce = unit.settings.knockDownHeight / 2f;
                unit.CamShake();
                bounceNum--;
                return;
            }
            if (col2D) col2D.offset = Vector2.zero;
            unit.stateMachine.SetState(new UnitKnockDownGrounded());
            return;
        }

        moveVector.y += yForce * Time.deltaTime * unit.settings.knockDownSpeed;
        yForce -= unit.settings.jumpGravity * Time.deltaTime * unit.settings.knockDownSpeed;

        if (col2D)
            col2D.offset = new Vector2(col2D.offset.x, -(moveVector.y - unit.groundPos));

        unit.transform.position = moveVector;
        if (yForce > 0)
            unit.animator.Play(animationNameUp);
        else
            unit.animator.Play(animationNameDown);
    }

    public override void Exit()
    {
        if (col2D) col2D.offset = Vector2.zero;
        unit.transform.position = unit.currentPosition;
        unit.isGrounded = true;
    }
}