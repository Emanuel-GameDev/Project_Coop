using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyIdleState : BasicEnemyState
{
    
    public BasicEnemyIdleState(BasicEnemy basicEnemy)
    {
        this.basicEnemy = basicEnemy;
    }

    public override void Enter()
    {
        base.Enter();

        basicEnemy.canSee = true;
        basicEnemy.canMove = false;
        basicEnemy.canAttack = false;

    }

    public override void Update()
    {
        base.Update();

        if (basicEnemy.viewTrigger.GetPlayersDetected().Count > 0 && basicEnemy.canSee)
        {
           
            stateMachine.SetState(basicEnemy.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
