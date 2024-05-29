using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LodonFuryCharge : LodonBaseState
{
    public LodonFuryCharge(LodonBoss lodonBossCharacter) : base(lodonBossCharacter) { }

    public override void Enter()
    {
        base.Enter();
        lodonBossCharacter.animationMustFollowTarget = true;
        lodonBossCharacter.Animator.SetTrigger(lodonBossCharacter.FURY_CHARGE);
    }


    override public void EndAnimation()
    {
        Debug.Log("Fury Charge Exit");
        stateMachine.SetState(LodonState.TargetAttackSelection);
       
    }
}
