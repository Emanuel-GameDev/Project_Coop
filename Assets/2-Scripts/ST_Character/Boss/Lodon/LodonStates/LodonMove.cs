using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class LodonMove : LodonBaseState
{
    public LodonMove(LodonBoss lodonBossCharacter) : base(lodonBossCharacter) { }

    UnityAction updateMoveAction;

    bool targetReached = false;
    Vector3 targetPosition = Vector3.zero;

    public override void Enter()
    {
        base.Enter();
        targetReached = false;
        updateMoveAction = null;
        targetPosition = Vector3.zero;
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
                updateMoveAction += FuryChargeMove;
                break;

            case LodonState.TridentThrowing:
                TridentThrowingSelectPosition();
                updateMoveAction += TridentThrowingMove;
                break;
        }
    }
    #region Trident Throwing
    private void TridentThrowingSelectPosition()
    {
        List<Transform> positions = lodonBossCharacter.Generator.OuterCenterPositions;
        targetPosition = positions[Random.Range(0, positions.Count)].position;
        lodonBossCharacter.Agent.SetDestination(targetPosition);
    }

    private void TridentThrowingMove()
    {
        if(targetReached) return;

        if (Vector2.Distance(lodonBossCharacter.transform.position, targetPosition) < lodonBossCharacter.TTreachDistance)
        {
            targetReached = true;
            lodonBossCharacter.Agent.isStopped = true;
            lodonBossCharacter.transform.position = targetPosition;
            lodonBossCharacter.Animator.SetTrigger(lodonBossCharacter.EMERGE);
        }

    }
    #endregion

    #region Fury Charge
    private void FuryChargeMove()
    {
        if (targetReached) return;

        if(Vector2.Distance(lodonBossCharacter.transform.position, lodonBossCharacter.target.position) < lodonBossCharacter.FCtargetReachDistance)
        {
            targetReached = true;
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
