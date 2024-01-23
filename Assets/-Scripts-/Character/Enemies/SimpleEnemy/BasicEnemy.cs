using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : EnemyCharacter
{

    public Transform groundLevel;

    [SerializeField] public float viewRange = 2f;
    [SerializeField] public float attackRange = 1;
    [SerializeField] float attackDelay = 1;

    //di prova
    [SerializeField] Transform tryTarget;

    NavMeshPath path;

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

    [SerializeField] public Detector viewTrigger;
    [SerializeField] public Detector attackTrigger;

    [SerializeField] float CarvingTime = 0.5f;
    [SerializeField] float CarvingMoveThreshold = 0.1f;

    public NavMeshObstacle obstacle;

    float lastMoveTime;
     Vector3 lastPosition;


    protected override void Awake()
    {
        base.Awake();

        obstacle = GetComponent<NavMeshObstacle>();

        obstacle.enabled = false;
        obstacle.carveOnlyStationary = false;
        obstacle.carving = true;

        lastPosition = transform.position;

        idleState = new BasicEnemyIdleState(this);
        moveState = new BasicEnemyMoveState(this);
        attackState = new BasicEnemyAttackState(this);


        path = new NavMeshPath();
    }

    private void Start()
    {
        viewTrigger.GetComponent<CapsuleCollider>().radius = viewRange;
        attackTrigger.GetComponent<CapsuleCollider>().radius = attackRange;

        stateMachine.SetState(idleState);
    }

    private void Update()
    {
        if(AIActive)
        stateMachine.StateUpdate();

        //if(Vector3.Distance(lastPosition, transform.position) > CarvingMoveThreshold)
        //{
        //    lastMoveTime = Time.time;
        //    lastPosition = transform.position;
        //}
        //if (lastMoveTime + CarvingTime < Time.time)
        //{
        //    Agent.enabled=false;
        //    obstacle.enabled=true;
        //}
    }

    public void FollowPath()
    {
        if (!canMove)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        obstacle.enabled = false;
        lastMoveTime = Time.time;
        lastPosition = transform.position;

        agent.enabled = true;

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

    public virtual void Move(Vector3 direction, Rigidbody rb)
    {
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
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackDelay);
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

        
    }

    
}
