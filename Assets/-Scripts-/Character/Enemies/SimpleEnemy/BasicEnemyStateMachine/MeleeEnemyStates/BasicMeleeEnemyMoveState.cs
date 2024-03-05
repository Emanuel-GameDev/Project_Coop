using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeEnemyMoveState : BasicEnemyMoveState
{
    public BasicMeleeEnemyMoveState(BasicEnemy basicEnemy) : base(basicEnemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Update()
    {
        base.Update();

        if (basicEnemy.AttackRangeTrigger.GetPlayersCountInTrigger() > 0)
        {
            basicEnemy.SetTarget(basicEnemy.AttackRangeTrigger.GetPlayersDetected()[0].transform);
            stateMachine.SetState(basicEnemy.actionState);
        }
        basicEnemy.FollowPath();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
