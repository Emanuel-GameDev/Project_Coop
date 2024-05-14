using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyParriedState : BasicEnemyState
{
    float stunTime = 1f;

    public BasicEnemyParriedState(BasicEnemy basicEnemy)
    {
        this.basicEnemy = basicEnemy;
    }

    public override void Enter()
    {
        base.Enter();

        stunTime = basicEnemy.parriedTime;


        basicEnemy.canSee = false;
        basicEnemy.canAction = false;
        basicEnemy.canMove = false;
        basicEnemy.isActioning = false;

        basicEnemy.GetAnimator().SetTrigger("Parried");

        basicEnemy.GetRigidBody().velocity = Vector3.zero;

        basicEnemy.ActivateObstacle();
    }

    public override void Update()
    {
        base.Update();

        stunTime -= Time.deltaTime;

        if (stunTime <= 0f)
        {

            stateMachine.SetState(basicEnemy.idleState);
        }

    }

    public override void Exit()
    {
        basicEnemy.GetAnimator().SetTrigger("ParryEnded");
        base.Exit();
    }

}
