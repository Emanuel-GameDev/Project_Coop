using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : BasicEnemy
{
    [Header("Variabili ranged")]
    [SerializeField] public float escapeRange = 0f;
    [SerializeField] int numberOfConsecutiveShoot;
    [SerializeField] float projectileSpeed;
    [SerializeField] float projectileRange;
    public float searchRadious = 3f;
    [HideInInspector] public bool panicAttack = false;

    [HideInInspector] public BasicRangedEnemyIdleState idleState;
    [HideInInspector] public BasicRangedEnemyMoveState moveState;
    [HideInInspector] public BasicRangedEnemyAttackState actionState;
    [HideInInspector] public BasicRangedEnemyEscapeState escapeState;

    List<PlayerCharacter> playerInRange;
    PlayerCharacter selectedPlayerInRange;


    protected override void Awake()
    {
        base.Awake();

        obstacle.enabled = false;
        obstacle.carveOnlyStationary = false;
        obstacle.carving = true;


        idleState = new BasicRangedEnemyIdleState(this);
        moveState = new BasicRangedEnemyMoveState(this);
        actionState = new BasicRangedEnemyAttackState(this);
        //stunState = new BasicEnemyStunState(this);
        //parriedState = new BasicEnemyParriedState(this);
        //deathState = new BasicEnemyDeathState(this);
        escapeState = new BasicRangedEnemyEscapeState(this);


    }


    public override void FollowPath()
    {
        base.FollowPath();
    }


    protected override void Start()
    {
        base.Start();

        if (canGoIdle)
            stateMachine.SetState(idleState);

        EscapeTrigger.GetComponent<CircleCollider2D>().radius = escapeRange;

    }

    public override IEnumerator Attack()
    {
        StopCoroutine(CalculateChasePathAndSteering());
        StopCoroutine(CalculateRunAwayPathAndSteering());
        isRunning = false;

        isActioning = true;

        if (panicAttack)
        {
            panicAttack = false;
        }

        ActivateObstacle();


        playerInRange = new List<PlayerCharacter>(AttackRangeTrigger.GetPlayersDetected());

        for (int i = 0; i < numberOfConsecutiveShoot; i++)
        {


            animator.SetTrigger("Attack");

            Projectile newProjectile = ProjectilePool.Instance.GetProjectile();

            newProjectile.transform.position = transform.position;

            selectedPlayerInRange = currentTarget;



            Vector2 direction = selectedPlayerInRange.transform.position - transform.position;

            newProjectile.Inizialize(direction, projectileRange, projectileSpeed, 1, damage, gameObject.layer);
            yield return new WaitForSeconds(attackDelay);


        }



        yield return new WaitForSeconds(attackSpeed);

        actionCourotine = null;

        isActioning = false;






    }

}

