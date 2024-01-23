using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyAttackState : BasicEnemyState
{
    public BasicEnemyAttackState(BasicEnemy basicEnemy)
    {
        this.basicEnemy = basicEnemy;
    }

    public override void Enter()
    {
        base.Enter();

        basicEnemy.canMove = false;
        basicEnemy.canAttack = true;

        basicEnemy.SetTarget(basicEnemy.attackTrigger.GetPlayersDetected()[0].transform);

        basicEnemy.StartCoroutine(basicEnemy.Attack());

        basicEnemy.Agent.enabled = false;
        basicEnemy.obstacle.enabled = true;
    }
    bool change=false;
    public override void Update()
    {
        base.Update();


        if (!basicEnemy.isAttacking)
        {
            //if (basicEnemy.attackTrigger.GetPlayersDetected().Count == 0)
            //{
                if(basicEnemy.viewTrigger.GetPlayersDetected().Count <= 0)
                    stateMachine.SetState(basicEnemy.idleState);
                else
                    stateMachine.SetState(basicEnemy.moveState);

                change = true;
            //}

            //basicEnemy.StartCoroutine(basicEnemy.Attack());

        }

        
    }

    public override void Exit()
    {
        base.Exit();
        change = false;
    }
}
