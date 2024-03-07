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

        stunTime = 1f;

        Debug.Log(stunTime);

        basicEnemy.canSee = false;
        basicEnemy.canAction = false;
        basicEnemy.canMove = false;
        basicEnemy.isActioning = false;

        basicEnemy.GetAnimator().SetTrigger("Damaged");

        basicEnemy.GetRigidBody().velocity = Vector3.zero;
    }

    public override void Update()
    {
        base.Update();

        stunTime -= Time.deltaTime ;

        Debug.Log(stunTime);
        if(stunTime <= 0f)
        {
            
            stateMachine.SetState(basicEnemy.moveState);
        }
       
    }

    public override void Exit()
    {
        base.Exit();
    }
}
