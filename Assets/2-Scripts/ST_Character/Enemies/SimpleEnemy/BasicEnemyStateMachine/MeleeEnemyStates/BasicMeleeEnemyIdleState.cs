using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BasicMeleeEnemyIdleState : BasicMeleeEnemyState
{
    public BasicMeleeEnemyIdleState(BasicMeleeEnemy basicEnemy) : base(basicEnemy)
    {
    }


    public override void Enter()
    {

        basicEnemy.canSee = true;
        basicEnemy.canMove = false;
        basicEnemy.canAction = false;

        basicEnemy.StartStopMovementCountdownCoroutine(false);
    }

    public override void Update()
    {
        if (meleeEnemy.viewTrigger.GetPlayersCountInTrigger() > 0 && basicEnemy.canSee)
        {
            if (basicEnemy.target == null)
            {
                basicEnemy.SetTarget(basicEnemy.viewTrigger.GetPlayersDetected()[0].transform);
            }

            stateMachine.SetState(meleeEnemy.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        
    }
}
