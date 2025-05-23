﻿using UnityEngine;

public class UnitStandUp : State
{
    private string animationName = "StandUp";
    private float animDuration => unit.GetAnimDuration(animationName);
    public override bool canGrab => false;

    public override void Enter()
    {
        unit.animator.Play(animationName, 0, 0);
    }

    public override void Update()
    {
        if (Time.time - stateStartTime > animDuration)
        {
            if (unit.isPlayer) unit.stateMachine.SetState(new PlayerIdle());
            else unit.stateMachine.SetState(new EnemyIdle());
        }
    }
}