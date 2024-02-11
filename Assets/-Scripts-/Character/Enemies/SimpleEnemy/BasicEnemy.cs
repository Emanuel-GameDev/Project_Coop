using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Localization.Settings;
using UnityEngine.TextCore.Text;

public class BasicEnemy : EnemyCharacter
{


    [Tooltip("Serve per gli effetti visivi dell' editor")]
    public Transform groundLevel;

    [SerializeField] public float viewRange = 2f;
    [SerializeField] public float closeRange = 1;
   


    NavMeshPath path;

    [SerializeField] Transform pivot;

    public StateMachine<BasicEnemyState> stateMachine { get; } = new();

    Vector2 lastNonZeroDirection;

    [SerializeField] public float despawnTime = 1;

    [HideInInspector] public BasicEnemyIdleState idleState;
    [HideInInspector] public BasicEnemyMoveState moveState;
    [HideInInspector] public BasicEnemyActionState actionState;
    [HideInInspector] public BasicEnemyDeathState deathState;
    [HideInInspector] public BasicEnemyStunState stunState;
    [HideInInspector] public BasicEnemyParriedState parriedState;

    [HideInInspector] public bool AIActive = true;

    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool isActioning = false;

    [HideInInspector] public bool canSee = true;
    [HideInInspector] public bool canMove = false;
    [HideInInspector] public bool canAction = false;

    [SerializeField] public Detector viewTrigger;
    [SerializeField] public Detector closeRangeTrigger;

    [HideInInspector] public NavMeshObstacle obstacle;



    protected override void Awake()
    {
        base.Awake();

        obstacle = GetComponent<NavMeshObstacle>();
        path = new NavMeshPath();


        //obstacle.enabled = false;
        //obstacle.carveOnlyStationary = false;
        //obstacle.carving = true;

        

    }

    protected virtual void Start()
    {
        viewTrigger.GetComponent<CapsuleCollider>().radius = viewRange;
        closeRangeTrigger.GetComponent<CapsuleCollider>().radius = closeRange;

        
    }

    protected virtual void Update()
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

        ActivateAgent();

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
    
    //forse nei figli
    public IEnumerator Attack()
    {

        isActioning = true;

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackSpeed);
        isActioning = false;

    }

    public override void TakeDamage(DamageData data)
    {
        base.TakeDamage(data);

        if (currentHp <= 0)
        {

            stateMachine.SetState(deathState);
        }
        else 
        {
            SetTarget(data.dealer.dealerTransform);
            stateMachine.SetState(stunState);
        }
       
    }

    public void DamagedAnimationEndedEvent()
    {
        animator.SetTrigger("DamageEnded");

        stateMachine.SetState(moveState);
    }



    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

   
    public void ActivateAgent()
    {
        obstacle.enabled = false;
        Agent.enabled = true;
    }

    public void ActivateObstacle()
    {
        Agent.enabled = false;
        obstacle.enabled = true;
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }

}
