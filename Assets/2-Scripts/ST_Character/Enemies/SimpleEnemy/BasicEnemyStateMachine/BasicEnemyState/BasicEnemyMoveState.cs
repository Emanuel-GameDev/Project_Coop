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
        basicEnemy.canMove = true;
        basicEnemy.canAction = false;

        Debug.Log("Bo");
        basicEnemy.GetAnimator().SetBool("isMoving", true);

    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
        basicEnemy.GetAnimator().SetBool("isMoving", false);
    }
}
