using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : BasicEnemy
{
    

    protected override void Awake()
    {
        base.Awake();

        obstacle.enabled = false;
        obstacle.carveOnlyStationary = false;
        obstacle.carving = true;


        idleState = new BasicEnemyIdleState(this);
        moveState = new BasicRangedEnemyMoveState(this);
        actionState = new BasicRangedEnemyAttackState(this);
        stunState = new BasicEnemyStunState(this);
        parriedState = new BasicEnemyParriedState(this);
        deathState = new BasicEnemyDeathState(this);
        escapeState= new BasicRangedEnemyEscapeState(this);
        

    }

    protected override void Start()
    {
        base.Start();

        //stateMachine.SetState(idleState);
    }


}
    
