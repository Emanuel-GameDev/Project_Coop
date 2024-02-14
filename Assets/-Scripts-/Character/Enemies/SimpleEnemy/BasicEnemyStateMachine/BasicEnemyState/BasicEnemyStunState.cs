using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyStunState : BasicEnemyState
{
    public BasicEnemyStunState(BasicEnemy basicEnemy)
    {
        this.basicEnemy = basicEnemy;
    }

    public override void Enter()
    {
        base.Enter();

        basicEnemy.canSee = false;
        basicEnemy.canAction = false;
        basicEnemy.canMove = false;

        basicEnemy.GetAnimator().SetTrigger("Damaged");

        basicEnemy.GetRigidBody().velocity = Vector3.zero;
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
