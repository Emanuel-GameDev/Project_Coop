using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRangedEnemyMoveState : BasicEnemyMoveState
{
    public BasicRangedEnemyMoveState(BasicEnemy basicEnemy) : base(basicEnemy)
    {

    }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Update()
    {
        base.Update();

        if (basicEnemy.viewTrigger.GetPlayersCountInTrigger() > 0)
        {
            if (basicEnemy.target == null)
                basicEnemy.SetTarget(basicEnemy.viewTrigger.GetPlayersDetected()[0].transform);

            stateMachine.SetState(basicEnemy.actionState);
        }
        basicEnemy.FollowPath();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
