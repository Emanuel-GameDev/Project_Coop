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

    public override void Move(Vector2 direction, Rigidbody2D rb)
    {
        if (direction == null) return;
        

        if (!direction.normalized.Equals(direction))
            direction = direction.normalized;

        //fix momentaneo
        if (direction == Vector2.up || direction == -Vector2.up)
            direction = Quaternion.Euler(0, 0, 1) * direction;


        if (direction == Vector2.zero)
        {
            GetAnimator().SetBool("isMoving", false);
        }
        else
        {
            animator.SetBool("isMoving", isMoving);

            if (!GetAnimator().GetBool("isMoving"))
                GetAnimator().SetBool("isMoving", true);
        }




        float speed = MoveSpeed;

        rb.velocity = direction * speed;

        isMoving = true;

        //if (targetDirection != Vector2.zero)
        //lastNonZeroDirection = targetDirection;

        if (flee)
        {
            SetSpriteDirection(direction);
        }
        else
            SetSpriteDirection(targetDirection-new Vector2(groundLevel.position.x, groundLevel.position.y));

        //animator.SetBool("isMoving", isMoving);
    }

    public override void SetSpriteDirection(Vector2 direction)
    {
        if (direction.y != 0)
            animator.SetFloat("Y", direction.y);

        //if (rb.velocity != Vector2.zero)
        //{
        //    float dot = Vector2.Dot(direction, rb.velocity);

        //    if (dot >= 0)
        //        animator.SetFloat("X", -1f);
        //    else
        //        animator.SetFloat("X", 1f);
        //}
        
        Vector3 scale = pivot.gameObject.transform.localScale;

        if ((direction.x > 0.5 && scale.x > 0) || (direction.x < -0.5 && scale.x < 0))
            scale.x *= -1;

        pivot.gameObject.transform.localScale = scale;
    }
    
    public override IEnumerator Attack()
    {

        

        StopCoroutine(CalculateChasePathAndSteering());
        StopCoroutine(CalculateRunAwayPathAndSteering());
        isRunning = false;

        isActioning = true;

        readyToAttack = false;


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

            SetSpriteDirection(direction);

            newProjectile.Inizialize(direction, projectileRange, projectileSpeed, 1, damage, gameObject.layer);
            yield return new WaitForSeconds(attackDelay);


        }



        yield return new WaitForSeconds(attackSpeed);

        actionCourotine = null;

        isActioning = false;


        readyToAttack = true;




    }

}

