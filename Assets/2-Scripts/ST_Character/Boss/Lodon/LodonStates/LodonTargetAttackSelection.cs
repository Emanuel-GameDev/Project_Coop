using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LodonTargetAttackSelection : LodonBaseState
{
    public LodonTargetAttackSelection(LodonBoss lodonBossCharacter) : base(lodonBossCharacter) { }

    override public void Enter()
    {
        base.Enter();
        SelectTarget();
        SelectAttack();
        stateMachine.SetState(LodonState.Move);
    }

    public void SelectTarget()
    {
        //TODO: Implement target selection
        
        lodonBossCharacter.TargetSelection();
    }

    public void SelectAttack()
    {
        //TODO: Implement attack selection
        if(Random.Range(0f, 1f) < 0.5f)
            lodonBossCharacter.selectedAttack = LodonState.FuryCharge;
        else
            lodonBossCharacter.selectedAttack = LodonState.TridentThrowing;
    }

}
