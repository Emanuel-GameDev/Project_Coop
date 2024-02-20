using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

public enum EnemyType
{
    melee,
    ranged
}

public class BasicEnemy : EnemyCharacter
{

    public Transform groundLevel;
    [Header("Variabili di base")]
    [SerializeField] public float viewRange = 2f;
    [SerializeField] public float attackRange = 1;
    [SerializeField] float attackDelay = 1;

    [SerializeField] EnemyType enemyType;

    [Header("Variabili ranged")]
    [SerializeField] public float escapeRange = 0f;
    [SerializeField] int numberOfConsecutiveShoot;
    [SerializeField] float projectileSpeed;
    [SerializeField] float projectileRange;

    List<PlayerCharacter> playerInRange;
    PlayerCharacter selectedPlayerInRange;


    //di prova
    //[SerializeField] Transform tryTarget;

    NavMeshPath path;

    [Header("Pivot")]
    [SerializeField] Transform pivot;

    public StateMachine<BasicEnemyState> stateMachine { get; } = new();
    

    Vector2 lastNonZeroDirection;


    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool isAttacking = false;

    [HideInInspector] public BasicEnemyIdleState idleState;
    [HideInInspector] public BasicEnemyMoveState moveState;
    [HideInInspector] public BasicEnemyAttackState attackState;

    [HideInInspector] public bool AIActive = true;

    [HideInInspector] public bool canSee = true;
    [HideInInspector] public bool canMove = false;
    [HideInInspector] public bool canAttack = false;

    [Header("Detectors")]
    [SerializeField] public Detector viewTrigger;
    [SerializeField] public Detector attackTrigger;
    [SerializeField] public Detector EscapeTrigger;

    [Header("navMesh carving value")]
    [SerializeField] float CarvingTime = 0.5f;
    [SerializeField] float CarvingMoveThreshold = 0.1f;

    [HideInInspector] public NavMeshObstacle obstacle;



    protected override void Awake()
    {
        base.Awake();

        obstacle = GetComponent<NavMeshObstacle>();

        obstacle.enabled = false;
        obstacle.carveOnlyStationary = false;
        obstacle.carving = true;


        idleState = new BasicEnemyIdleState(this);
        moveState = new BasicEnemyMoveState(this);
        attackState = new BasicEnemyAttackState(this,enemyType);


        path = new NavMeshPath();
    }

    private void Start()
    {
        viewTrigger.GetComponent<CapsuleCollider>().radius = viewRange;
        attackTrigger.GetComponent<CapsuleCollider>().radius = attackRange;

        if(enemyType == EnemyType.ranged)
        {
            EscapeTrigger.GetComponent<CapsuleCollider>().radius = escapeRange;
        }

        stateMachine.SetState(idleState);
    }

    private void Update()
    {
        if(AIActive)
        stateMachine.StateUpdate();

    }

    public void FollowPath()
    {
        if (!canMove)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        obstacle.enabled = false;

        agent.enabled = true;

        if (target != null)
        {
            if (agent.CalculatePath(target.position, path))
            {

                if (path.corners.Length > 1)
                    Move(path.corners[1] - path.corners[0], rb);
                else
                    Move(target.position - transform.position, rb);
            }
            else
                rb.velocity = Vector3.zero;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    public virtual void Move(Vector3 direction, Rigidbody rb)
    {
        if (obstacle.enabled)
            return;
        //if (Vector3.Distance(transform.position,tryTarget.position) < attackRange)
        //{
        //    rb.velocity = Vector3.zero;


        //    return;
        //}
        
        if (!direction.normalized.Equals(direction))
            direction = direction.normalized;

        rb.velocity = new Vector3(direction.x * MoveSpeed, direction.y, direction.z * MoveSpeed);




        Vector2 direction2D = new Vector2(direction.x, direction.z);

        isMoving = rb.velocity.magnitude > 0.2f;
        

        if (direction2D != Vector2.zero)
            lastNonZeroDirection = direction2D;

        SetSpriteDirection(lastNonZeroDirection);

        animator.SetBool("isMoving", isMoving);
    }

    protected void SetSpriteDirection(Vector2 direction)
    {
        if (direction.y != 0)
            animator.SetFloat("Y", direction.y);

        Vector3 scale = pivot.gameObject.transform.localScale;

        if ((direction.x > 0.5 && scale.x > 0) || (direction.x < -0.5 && scale.x < 0))
            scale.x *= -1;
        
        pivot.gameObject.transform.localScale = scale;
    }

    public Animator GetAnimator()
    {
        return animator;
    }
    
    public IEnumerator Attack()
    {

        isAttacking = true;
        //GetComponentInChildren<SpriteRenderer>().material.color = Color.red;

        switch (enemyType)
        {
            case EnemyType.melee:
                animator.SetTrigger("Attack");
                yield return new WaitForSeconds(attackDelay);
                break;

            case EnemyType.ranged:
                playerInRange = attackTrigger.GetPlayersDetected();

                for (int i = 0; i < numberOfConsecutiveShoot; i++)
                {
                    animator.SetTrigger("Attack");

                    Projectile newProjectile = ProjectilePool.Instance.GetProjectile();

                    newProjectile.transform.position = transform.position;

                    selectedPlayerInRange = playerInRange[Random.Range(0, playerInRange.Count)];



                    Vector3 direction = transform.position - selectedPlayerInRange.transform.position;

                    newProjectile.Inizialize(direction, projectileRange, projectileSpeed, 1, damage, gameObject.layer);
                    yield return new WaitForSeconds(attackDelay);

                    playerInRange.Clear();
                }
                break;
        }


        
        isAttacking = false;
        //GetComponentInChildren<SpriteRenderer>().material.color = Color.white;
    }

    
    

    public override void TargetSelection()
    {
        
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public override void TakeDamage(DamageData data)
    {
        base.TakeDamage(data);

        currentHp -= data.damage ;
    }



}
