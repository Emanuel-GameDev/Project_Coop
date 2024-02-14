using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyDeathState : BasicEnemyState
{
    float timerForDespawn = 0;

    public BasicEnemyDeathState(BasicEnemy basicEnemy)
    {
        this.basicEnemy = basicEnemy;
    }

    public override void Enter()
    {
        base.Enter();

        basicEnemy.canSee = false;
        basicEnemy.canMove = false;
        basicEnemy.canAction = false;

        basicEnemy.GetAnimator().SetTrigger("Dead");
        
        basicEnemy.GetRigidBody().velocity = Vector3.zero;
    }

    public override void Update()
    {
        base.Update();

        if (timerForDespawn < basicEnemy.despawnTime)
            timerForDespawn += Time.deltaTime;
        else
            basicEnemy.Despawn();
    }

    public override void Exit() 
    { 
        base.Exit();
    }
}
