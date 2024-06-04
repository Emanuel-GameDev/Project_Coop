using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicMeleeEnemy : BasicEnemy
{
    [HideInInspector] public BasicMeleeEnemyIdleState idleState;
    [HideInInspector] public BasicMeleeEnemyMoveState moveState;
    [HideInInspector] public BasicMeleeEnemyAttackState actionState;
    protected override void Awake()
    {
        base.Awake();

        obstacle.enabled = false;
        obstacle.carveOnlyStationary = false;
        obstacle.carving = true;

        //agent = GetComponentInChildren<NavMeshAgent>(true);

        
        idleState = new BasicMeleeEnemyIdleState(this);
        moveState = new BasicMeleeEnemyMoveState(this);
        actionState = new BasicMeleeEnemyAttackState(this);
        //stunState = new BasicEnemyStunState(this);
        //parriedState = new BasicEnemyParriedState(this);
        //deathState = new BasicEnemyDeathState(this);

    }

    protected override void Start()
    {
        base.Start();
        if (canGoIdle)
            stateMachine.SetState(idleState);
        //CONTROLLARE
        //stateMachine.SetState(idleState);
    }

    public override void FollowPath()
    {
        base.FollowPath();
    }

    public override IEnumerator Attack()
    {
        StopCoroutine(CalculateChasePathAndSteering());
        isRunning = false;

        isActioning = true;


        ActivateObstacle();
        readyToAttack = false;

        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(attackDelay);
        isActioning = false;
        readyToAttack = true;

    }

    public override void SetIdleState()
    {
        base.SetIdleState();
        stateMachine.SetState(idleState);
    }
}
