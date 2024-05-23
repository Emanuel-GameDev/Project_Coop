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

        basicEnemy.canMove = true;
        basicEnemy.canAction = false;

        basicEnemy.ActivateAgent();
        basicEnemy.GetAnimator().SetBool("isMoving", true);

    }

    public override void Update()
    {
        base.Update();

        if (basicEnemy.viewTrigger.GetPlayersCountInTrigger() > 0)
        {
            if (basicEnemy.target == null)
                basicEnemy.SetTarget(basicEnemy.viewTrigger.GetPlayersDetected()[0].transform);

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
