using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyEntryState : BasicEnemyState
{
    public BasicEnemyEntryState(BasicEnemy basicEnemy)
    {
        this.basicEnemy = basicEnemy;
    }

    public override void Enter()
    {
        base.Enter();
        basicEnemy.canMove = true;
        basicEnemy.canAction = false;


        basicEnemy.GetAnimator().SetBool("isMoving", true);

    }

    public override void Update()
    {
        base.Update();

        basicEnemy.GoToPosition(basicEnemy.entryDestination);
        if(Vector2.Distance(basicEnemy.transform.position,basicEnemy.entryDestination) <= 0.2)
        {
            basicEnemy.stateMachine.SetState(basicEnemy.idleState);
        }
        //if(basicEnemy.entryDestination == new Vector2( basicEnemy.transform.position.x, basicEnemy.transform.position.y))
        //{
        //    basicEnemy.stateMachine.SetState(basicEnemy.moveState);
        //}

    }

    public override void Exit()
    {
        base.Exit();
        basicEnemy.GetAnimator().SetBool("isMoving", false);
    }
}
