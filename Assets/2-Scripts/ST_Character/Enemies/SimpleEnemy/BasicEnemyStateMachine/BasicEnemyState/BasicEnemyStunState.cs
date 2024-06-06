using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyStunState : BasicEnemyState
{
    float stunTime=1f;
    public BasicEnemyStunState(BasicEnemy basicEnemy)
    {
        this.basicEnemy = basicEnemy;
        
    }

    public override void Enter()
    {
        base.Enter();

        stunTime = basicEnemy.stunTime;


        basicEnemy.canSee = false;
        basicEnemy.canAction = false;
        basicEnemy.canMove = false;
        basicEnemy.isActioning = false;

        basicEnemy.GetAnimator().SetTrigger("Damaged");

        basicEnemy.GetRigidBody().velocity = Vector3.zero;

        basicEnemy.ActivateObstacle();
    }

    public override void Update()
    {
        base.Update();

        stunTime -= Time.deltaTime ;

        if(stunTime <= 0f)
        {
            basicEnemy.SetIdleState();
        }
       
    }

    public override void Exit()
    {
        basicEnemy.GetAnimator().SetTrigger("DamageEnded");
        base.Exit();
    }
}
