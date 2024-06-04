using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LodonTridentThrowing : LodonBaseState
{
    public LodonTridentThrowing(LodonBoss lodonBossCharacter) : base(lodonBossCharacter) { }

    public bool tridentReturned = false;

    public override void Enter()
    {
        base.Enter();
        lodonBossCharacter.animationMustFollowTarget = true;
        tridentReturned = false;
        lodonBossCharacter.Animator.SetTrigger(lodonBossCharacter.TRIDENT_THROWING);
    }


    override public void EndAnimation()
    {
        Debug.Log("Trident Throwing Exit");
        lodonBossCharacter.StartCoroutine(WaitTridentReturn());
    }

    IEnumerator WaitTridentReturn()
    {
        yield return new WaitUntil(() => tridentReturned);
        stateMachine.SetState(LodonState.TargetAttackSelection);
    }

}
