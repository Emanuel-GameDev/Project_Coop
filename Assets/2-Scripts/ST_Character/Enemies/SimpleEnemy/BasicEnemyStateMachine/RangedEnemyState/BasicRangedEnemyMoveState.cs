using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRangedEnemyMoveState : BasicRangedEnemyState
{
    public BasicRangedEnemyMoveState(RangedEnemy basicEnemy) : base(basicEnemy)
    {

    }

    public override void Enter()
    {
        //base.Enter();
        
        rangedEnemy.canMove = true;
        rangedEnemy.canAction = false;

        rangedEnemy.ActivateAgent();
        rangedEnemy.GetAnimator().SetBool("isMoving", true);

    }

    public override void Update()
    {
        base.Update();

        if (rangedEnemy.EscapeTrigger.GetPlayersCountInTrigger() > 0)
        {
            stateMachine.SetState(rangedEnemy.escapeState);
        }
        else if (rangedEnemy.AttackRangeTrigger.GetPlayersCountInTrigger() > 0)
        {
            //if (rangedEnemy.target == null)
            //    rangedEnemy.SetTarget(rangedEnemy.viewTrigger.GetPlayersDetected()[0].transform);

            stateMachine.SetState(rangedEnemy.actionState);
        }

        rangedEnemy.FollowPath();
    }

    public override void Exit()
    {
        base.Exit();
        basicEnemy.GetAnimator().SetBool("isMoving", false);
    }
}
