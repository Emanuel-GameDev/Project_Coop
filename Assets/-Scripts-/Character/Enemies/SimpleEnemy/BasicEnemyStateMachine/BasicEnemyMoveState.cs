using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyMoveState : BasicEnemyState
{
    public BasicEnemyMoveState(BasicEnemy basicEnemy)
    {
        this.basicEnemy = basicEnemy;
    }

    public override void Enter()
    {
        base.Enter();
        basicEnemy.canMove = true;
        basicEnemy.canAttack = false;

        //da rimettere dopo
        //basicEnemy.SetTarget(basicEnemy.viewTrigger.GetPlayersDetected()[0].transform);



        //basicEnemy.obstacle.enabled = false;
        //basicEnemy.Agent.enabled = true;
    }

    public override void Update()
    {
        base.Update();

        if (basicEnemy.attackTrigger.GetPlayersDetected().Count > 0)
        {
            
            stateMachine.SetState(basicEnemy.attackState);
        }

        basicEnemy.FollowPath();
    }

    public override void Exit()
    {
        base.Exit();
        basicEnemy.GetAnimator().SetBool("isMoving", false);
    }
}
