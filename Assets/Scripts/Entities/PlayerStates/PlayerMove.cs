using UnityEngine;

public class PlayerMove : State
{
    private string animationName = "Run";

    public override void Update()
    {
        //defend
        if (InputManager.DefendKeyDown())
        {
            unit.stateMachine.SetState(new UnitDefend());
            return;
        }

        //jump
        if (InputManager.JumpKeyDown())
        {
            unit.stateMachine.SetState(new PlayerJump());
            return;
        }

        //use weapon
        //if(unit.weapon && InputManager.PunchKeyDown()){ unit.stateMachine.SetState(new PlayerWeaponAttack()); return; }

        //check for nearby enemy to ground pound
        if (InputManager.PunchKeyDown() && unit.NearbyEnemyDown())
        {
            unit.stateMachine.SetState(new PlayerGroundPunch());
            return;
        }

        //punch Key pressed
        if (InputManager.PunchKeyDown())
        {
            unit.stateMachine.SetState(new PlayerAttack(ATTACKTYPE.PUNCH));
            return;
        }

        //grab something (enemy or item)
        if (InputManager.GrabKeyDown())
        {
            unit.stateMachine.SetState(new PlayerTryGrab());
            return;
        }

        //drop current weapon
        //if(InputManager.GrabKeyDown() && unit.weapon){ unit.stateMachine.SetState(new UnitDropWeapon()); return; }
    }

    public override void FixedUpdate()
    {
        //get input
        Vector2 inputVector = InputManager.GetInputVector().normalized;

        //go to idle, if there is no input
        if (inputVector.magnitude == 0)
        {
            unit.stateMachine.SetState(new PlayerIdle());
            return;
        }

        //go to idle if there is a wall in front of us
        Vector2
            wallDistanceCheck =
                unit.col2D
                    ? (unit.col2D.size / 1.6f) * 1.1f
                    : Vector2.one *
                      .3f; //dividing by 1.6f because the distance check needs to be a bit larger than the collider (otherwise we never encounter a wall)
        if (unit.WallDetected(inputVector * wallDistanceCheck))
        {
            unit.stateMachine.SetState(new PlayerIdle()); //go to idle
            return;
        }

        //adjust input to move slower in the y position to create a sense of depth
        inputVector.y *= .8f;

        //move
        unit.MoveToVector(inputVector, unit.settings.moveSpeed);

        //play run anim
        unit.animator.Play(animationName);
    }
}