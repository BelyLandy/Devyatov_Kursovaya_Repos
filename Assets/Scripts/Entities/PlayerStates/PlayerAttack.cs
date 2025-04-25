using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerAttack : State
{
    private bool damageDealt;
    private bool animFinished => Time.time - stateStartTime > animDuration;
    private float animDuration => unit.GetAnimDuration(attackData.animationState);
    private AttackData attackData;
    private ATTACKTYPE attackKeyPressed;
    private ATTACKTYPE currentAttackType;
    private Combo currentCombo;
    private int comboProgress => unit.attackList.Count - 1;

    public PlayerAttack(ATTACKTYPE attackType)
    {
        currentAttackType = attackType;
    }

    public override void Enter()
    {
        unit.StopMoving();
        unit.attackList.Add(currentAttackType);
        currentCombo = FindComBoMatch(unit.attackList);
        bool followUpTimeExpired = Time.time - unit.lastAttackTime > unit.settings.comboResetTime;
        unit.lastAttackTime = Time.time;
        if (currentCombo == null || followUpTimeExpired)
        {
            unit.attackList.Clear();
            unit.attackList.Add(currentAttackType);
            currentCombo = FindComBoMatch(unit.attackList);
            if (currentCombo == null)
                unit.stateMachine.SetState(new PlayerIdle());
        }
        unit.TurnToFloatDir(InputManager.GetInputVector().x);
        attackData = currentCombo?.attackSequence[comboProgress];
        if (attackData?.animationState.Length == 0)
            Debug.Log("Please enter animation state for combo: " + currentCombo.comboName + " - Attack: " + (comboProgress + 1));
        else
            unit.animator.Play(attackData.animationState, 0, 0);
    }

    public override void Update()
    {
        if (attackData == null) return;
        if (InputManager.PunchKeyDown())
            attackKeyPressed = ATTACKTYPE.PUNCH;
        if (!damageDealt)
            damageDealt = unit.CheckForHit(attackData);
        bool isLastAttack = currentCombo.attackSequence.Count - 1 == comboProgress;
        if (isLastAttack)
            attackKeyPressed = ATTACKTYPE.NONE;
        if (animFinished)
        {
            if (attackKeyPressed != ATTACKTYPE.NONE)
            {
                unit.stateMachine.SetState(new PlayerAttack(attackKeyPressed));
                return;
            }
            unit.stateMachine.SetState(new PlayerIdle());
        }
    }

    public override void Exit()
    {
        if (!damageDealt && unit.settings.continueComboOnHit)
            unit.attackList.Clear();
    }

    private Combo FindComBoMatch(List<ATTACKTYPE> attackList)
    {
        foreach (Combo combo in unit.settings.comboData)
        {
            if (combo.attackSequence.Count < attackList.Count)
                continue;
            List<ATTACKTYPE> comboAttackSequence = combo.attackSequence
                .Select(ad => ad.attackType)
                .ToList();
            if (attackList.SequenceEqual(comboAttackSequence.Take(attackList.Count)))
                return combo;
        }
        return null;
    }
}