using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyActionState : BasicEnemyState
{
    public BasicEnemyActionState(BasicEnemy basicEnemy)
    {
        this.basicEnemy = basicEnemy;
    }

    public override void Enter()
    {
        base.Enter();

        basicEnemy.canMove = false;
        basicEnemy.canAction = true;


        basicEnemy.ActivateObstacle();
        basicEnemy.StartStopMovementCountdownCoroutine(false);
    }
    
    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
