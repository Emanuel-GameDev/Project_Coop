using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeEnemyAttackState : BasicEnemyActionState
{
    public BasicMeleeEnemyAttackState(BasicEnemy basicEnemy) : base(basicEnemy)
    {
    }


    public override void Enter()
    {
        base.Enter();

        basicEnemy.StartCoroutine(basicEnemy.Attack());

    }

    public override void Update()
    {
        base.Update();


        if (!basicEnemy.isActioning)
        {
            if (basicEnemy.closeRangeTrigger.GetPlayersCountInTrigger() == 0)
            {
                stateMachine.SetState(basicEnemy.moveState);
            }
            else
                basicEnemy.StartCoroutine(basicEnemy.Attack());

        }


    }

    public override void Exit()
    {
        base.Exit();
    }
}
