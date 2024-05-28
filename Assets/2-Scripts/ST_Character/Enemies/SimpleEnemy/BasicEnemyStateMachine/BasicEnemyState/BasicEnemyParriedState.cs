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

        //mettere Parried al posto di damaged se le animazioni di stun e danno sono diverse
        basicEnemy.GetAnimator().SetTrigger("Damaged");

        basicEnemy.GetRigidBody().velocity = Vector3.zero;

        basicEnemy.ActivateObstacle();
    }

    public override void Update()
    {
        base.Update();

        stunTime -= Time.deltaTime;

        if (stunTime <= 0f)
        {
            basicEnemy.SetIdleState();
        }

    }

    public override void Exit()
    {
        basicEnemy.GetAnimator().SetTrigger("DamageEnded");
        basicEnemy.GetAnimator().SetTrigger("ParryEnded");
        base.Exit();
    }

}
