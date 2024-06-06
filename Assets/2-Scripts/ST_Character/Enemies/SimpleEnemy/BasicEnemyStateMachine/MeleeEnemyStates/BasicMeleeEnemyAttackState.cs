using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeEnemyAttackState : BasicMeleeEnemyState
{
    public BasicMeleeEnemyAttackState(BasicMeleeEnemy basicEnemy) : base(basicEnemy)
    {
    }


    public override void Enter()
    {
        base.Enter();

        basicEnemy.canMove = false;
        basicEnemy.canAction = true;


        basicEnemy.StartStopMovementCountdownCoroutine(false);
        basicEnemy.StartCoroutine(basicEnemy.Attack());
        basicEnemy.ActivateObstacle();

    }

    public override void Update()
    {
        base.Update();

        
        if (!basicEnemy.isActioning)
        {
            if (basicEnemy.AttackRangeTrigger.GetPlayersCountInTrigger() == 0)
            {
                stateMachine.SetState(meleeEnemy.idleState);
            }
            else
                meleeEnemy.StartCoroutine(meleeEnemy.Attack());

        }


    }

    public override void Exit()
    {
        base.Exit();
    }
}
