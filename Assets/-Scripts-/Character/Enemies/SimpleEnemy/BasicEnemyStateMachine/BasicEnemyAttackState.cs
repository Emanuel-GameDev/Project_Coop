using System.Collections;
using System.Collections.Generic;
using UnityEngine;
   

public class BasicEnemyAttackState : BasicEnemyState
{
    public BasicEnemyAttackState(BasicEnemy basicEnemy, EnemyType enemyType)
    {
        this.basicEnemy = basicEnemy;
        this.enemyType = enemyType;
    }

    public override void Enter()
    {
        base.Enter();

        basicEnemy.canMove = false;
        basicEnemy.canAttack = true;

        //da rimettere dopo
        //basicEnemy.SetTarget(basicEnemy.attackTrigger.GetPlayersDetected()[0].transform);

        basicEnemy.StartCoroutine(basicEnemy.Attack());

        basicEnemy.Agent.enabled = false;
        basicEnemy.obstacle.enabled = true;
    }
    
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

               
            //}

            //basicEnemy.StartCoroutine(basicEnemy.Attack());

        }

        
    }

    public override void Exit()
    {
        base.Exit();
    }
}
