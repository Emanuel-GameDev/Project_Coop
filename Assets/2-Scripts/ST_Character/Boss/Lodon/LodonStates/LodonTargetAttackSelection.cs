using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LodonTargetAttackSelection : LodonBaseState
{
    public LodonTargetAttackSelection(LodonBoss lodonBossCharacter) : base(lodonBossCharacter) { }

    override public void Enter()
    {
        SelectTarget();
        SelectAttack();
        stateMachine.SetState(LodonState.Move);
    }

    public void SelectTarget()
    {
        lodonBossCharacter.TargetSelection();
    }

    public void SelectAttack()
    {
        lodonBossCharacter.selectedAttack = LodonState.FuryCharge;
    }

}
