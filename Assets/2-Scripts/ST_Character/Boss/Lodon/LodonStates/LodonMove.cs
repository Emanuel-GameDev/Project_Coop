using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LodonMove : LodonBaseState
{
    public LodonMove(LodonBoss lodonBossCharacter) : base(lodonBossCharacter) { }

    UnityAction updateMoveAction;

    bool FCtargetReached = false;

    public override void Enter()
    {
        base.Enter();
        updateMoveAction = null;
        lodonBossCharacter.Agent.isStopped = false;
        lodonBossCharacter.animationMustFollowTarget = false;
        lodonBossCharacter.Animator.SetBool(lodonBossCharacter.MOVING, true);
        SelectMovement();

    }

    public override void Update()
    {
        updateMoveAction?.Invoke();

    }

    public override void Exit()
    {
        lodonBossCharacter.Animator.SetBool(lodonBossCharacter.MOVING, false);
    }

    private void SelectMovement()
    {
        switch (lodonBossCharacter.selectedAttack)
        {
            case LodonState.FuryCharge:
                FCtargetReached = false;
                updateMoveAction += FuryChargeMove;
                break;
        }
    }

    #region Fury Charge
    private void FuryChargeMove()
    {
        if (FCtargetReached) return;

        if(Vector2.Distance(lodonBossCharacter.transform.position, lodonBossCharacter.target.position) < lodonBossCharacter.FCtargetReachDistance)
        {
            FCtargetReached = true;
            lodonBossCharacter.Agent.isStopped = true;
            Emerge();
        }
        else
        {
            lodonBossCharacter.Agent.SetDestination(lodonBossCharacter.target.position);
        }
    }

    #endregion
    private void Emerge()
    {
        if(Utility.FindNearestObjectWithComponent<LodonPlatform>(lodonBossCharacter.transform.position, lodonBossCharacter.searchRadius, out LodonPlatform platformFounded))
        {
            lodonBossCharacter.transform.position = platformFounded.transform.position;
            platformFounded.BreakFromUnder();
            lodonBossCharacter.Animator.SetTrigger(lodonBossCharacter.EMERGE);
        }
    }

    public override void EndAnimation()
    {
        base.EndAnimation();
        Debug.Log("Move Exit");
        stateMachine.SetState(lodonBossCharacter.selectedAttack);

    }

}
