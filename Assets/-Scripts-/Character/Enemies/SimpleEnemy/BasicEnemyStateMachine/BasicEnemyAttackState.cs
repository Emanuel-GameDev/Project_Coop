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
    }

    public override void Update()
    {
        base.Update();

        if(!basicEnemy.isAttacking)
            basicEnemy.StartCoroutine(basicEnemy.Attack());

        if (basicEnemy.attackTrigger.GetPlayersDetected().Count == 0)
        {
            if(basicEnemy.viewTrigger.GetPlayersDetected().Count <= 0)
                stateMachine.SetState(basicEnemy.idleState);
            else
                stateMachine.SetState(basicEnemy.moveState);
        }

    }

    public override void Exit()
    {
        base.Exit();
    }
}
