using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : BasicEnemy
{
    [Header("Variabili ranged")]
    [SerializeField] public Detector EscapeTrigger;
    [Tooltip("Distanza da mantenere dal bersaglio")]
    [SerializeField] public float escapeRange = 0f;
    [Tooltip("Numero di colpi consecutivi in ogni attacco")]
    [SerializeField] int numberOfConsecutiveShoot;
    [Tooltip("Velocità dei proiettili")]
    [SerializeField] float projectileSpeed;
    [Tooltip("Range dei proiettili")]
    [SerializeField] float projectileRange;
    public float searchRadious = 3f;

    [SerializeField] Transform projectileSpawnPoint;
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



        playerInRange = new List<PlayerCharacter>(AttackRangeTrigger.GetPlayersDetected());

        for (int i = 0; i < numberOfConsecutiveShoot; i++)
        {

            animator.SetTrigger("Attack");
            
            yield return new WaitForSeconds(attackDelay);


        }



        yield return new WaitForSeconds(attackSpeed);

        actionCourotine = null;

        isActioning = false;


        readyToAttack = true;




    }

    public void Shoot()
    {
        Projectile newProjectile = ProjectilePool.Instance.GetProjectile();

        newProjectile.transform.position = projectileSpawnPoint.position;

        selectedPlayerInRange = currentTarget;



        Vector2 direction = selectedPlayerInRange.transform.position - transform.position;

        SetSpriteDirection(direction);

        newProjectile.Inizialize(direction, projectileRange, projectileSpeed, 1, damage, gameObject.layer);
    }
}

