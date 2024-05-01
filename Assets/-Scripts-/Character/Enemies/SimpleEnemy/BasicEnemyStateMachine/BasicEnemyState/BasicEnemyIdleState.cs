using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyIdleState : BasicEnemyState
{
    
    public BasicEnemyIdleState(BasicEnemy basicEnemy)
    {
        this.basicEnemy = basicEnemy;
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
            if(basicEnemy.target == null)
                basicEnemy.SetTarget(basicEnemy.viewTrigger.GetPlayersDetected()[0].transform);

            stateMachine.SetState(basicEnemy.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
