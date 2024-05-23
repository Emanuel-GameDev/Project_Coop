using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRangedEnemyIdleState : BasicRangedEnemyState
{
    public BasicRangedEnemyIdleState(RangedEnemy basicEnemy) : base(basicEnemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        basicEnemy.canSee = true;
        basicEnemy.canMove = false;
        basicEnemy.canAction = false;

        basicEnemy.ActivateObstacle();


    }

    public override void Update()
    {
        base.Update();

        if (basicEnemy.viewTrigger.GetPlayersCountInTrigger() > 0 && basicEnemy.canSee)
        {
            if (basicEnemy.target == null)
                basicEnemy.SetTarget(basicEnemy.viewTrigger.GetPlayersDetected()[0].transform);

            stateMachine.SetState(rangedEnemy.actionState);
            return;
        }

        if (rangedEnemy.EscapeTrigger.GetPlayersCountInTrigger() > 0 && rangedEnemy.canSee)
        {
            if (rangedEnemy.target == null)
                rangedEnemy.SetTarget(rangedEnemy.EscapeTrigger.GetPlayersDetected()[0].transform);

            stateMachine.SetState(rangedEnemy.escapeState);
        }

    }

    public override void Exit()
    {
        base.Exit();
    }
}
