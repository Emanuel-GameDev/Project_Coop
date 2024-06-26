using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Localization.Settings;
using UnityEngine.TextCore.Text;

public enum EnemyType
{
    melee,
    ranged
}

public class BasicEnemy : EnemyCharacter
{


    [Tooltip("Serve per gli effetti visivi dell' editor")]
    public Transform groundLevel;

    [Header("Variabili generali")]
    [SerializeField] public float parriedTime = 1;


    [Header("Variabili di base")]
    [SerializeField] public float viewRange = 2f;
    [SerializeField] public float attackRange = 1;
    [SerializeField] float attackDelay = 1;
    [SerializeField] public float despawnTime = 1;

    [SerializeField] EnemyType enemyType;

    [Header("Variabili ranged")]
    [SerializeField] public float escapeRange = 0f;
    [SerializeField] int numberOfConsecutiveShoot;
    [SerializeField] float projectileSpeed;
    [SerializeField] float projectileRange;
    public float searchRadious = 3f;
    [HideInInspector] public bool panicAttack=false;

    private Coroutine actionCourotine;

    List<PlayerCharacter> playerInRange;
    PlayerCharacter selectedPlayerInRange;


    //di prova
    //[SerializeField] Transform tryTarget;

    //[SerializeField] public float closeRange = 1;
   
    NavMeshPath path;

    [Header("Pivot")]
    [SerializeField] Transform pivot;

    public StateMachine<BasicEnemyState> stateMachine { get; } = new();
    

    Vector2 lastNonZeroDirection;


    
    [HideInInspector] public BasicEnemyIdleState idleState;
    [HideInInspector] public BasicEnemyMoveState moveState;
    [HideInInspector] public BasicEnemyActionState actionState;
    [HideInInspector] public BasicEnemyDeathState deathState;
    [HideInInspector] public BasicEnemyStunState stunState;
    [HideInInspector] public BasicEnemyParriedState parriedState;
    [HideInInspector] public BasicRangedEnemyEscapeState escapeState;

    [HideInInspector] public bool AIActive = true;

    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool isActioning = false;

    [HideInInspector] public bool canSee = true;
    [HideInInspector] public bool canMove = false;
    [HideInInspector] public bool canAction = false;

    [Header("Detectors")]
    [SerializeField] public Detector viewTrigger;
    [SerializeField] public Detector AttackRangeTrigger;
    [SerializeField] public Detector EscapeTrigger;

    [Header("navMesh carving value")]
    [SerializeField] float CarvingTime = 0.5f;
    [SerializeField] float CarvingMoveThreshold = 0.1f;


    [HideInInspector] public NavMeshObstacle obstacle;
    [HideInInspector] public PlayerCharacter currentTarget;

    //CONTROLLARE
    [HideInInspector] public BasicEnemyEntryState entryState;
    [HideInInspector] public Vector2 entryDestination;
    [HideInInspector] public bool canGoIdle = true;


    protected override void InitialSetup()
    {
        base.InitialSetup();
        animator = GetComponent<Animator>();

        if(GetComponent<NavMeshAgent>() != null)
             agent = GetComponent<NavMeshAgent>();

        currentHp = maxHp;
    }

    public void GoToPosition(Vector2 pos)
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        ActivateAgent();

        if (pos != null)
        {         
            Move((Vector3)pos -transform.position,rb);          
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        obstacle = GetComponent<NavMeshObstacle>();

        obstacle.enabled = false;
        obstacle.carveOnlyStationary = false;
        obstacle.carving = true;


        idleState = new BasicEnemyIdleState(this);
        moveState = new BasicEnemyMoveState(this);
        actionState = new BasicEnemyActionState(this);
        entryState = new BasicEnemyEntryState(this);

        path = new NavMeshPath();



        //obstacle.enabled = false;
        //obstacle.carveOnlyStationary = false;
        //obstacle.carving = true;

        

    }

    protected virtual void Start()
    {
        viewTrigger.GetComponent<CircleCollider2D>().radius = viewRange;
        AttackRangeTrigger.GetComponent<CircleCollider2D>().radius = attackRange;
        //closeRangeTrigger.GetComponent<CircleCollider2D>().radius = closeRange;
        if(enemyType == EnemyType.ranged)
        {
            EscapeTrigger.GetComponent<CircleCollider2D>().radius = escapeRange;
        }

        if(canGoIdle)
        stateMachine.SetState(idleState);       
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
            rb.velocity = Vector2.zero;
            return;
        }

        ActivateAgent();

        if (currentTarget != null)
        {
            if (agent.CalculatePath(target.position, path))
            {
            //    if (path.corners.Length > 1)
                    Move(path.corners[1] - path.corners[0], rb);
                //else
                //    Move(target.position - transform.position, rb);

            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void FollowPosition(Vector2 pos)
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        ActivateAgent();

        if (pos != null)
        {
            if (agent.CalculatePath(pos, path))
            {

                //if (path.corners.Length > 1)
                    Move(path.corners[1] - path.corners[0], rb);
                //else
                //    Move((Vector3)pos - transform.position, rb);
                
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
    
    public virtual void Move(Vector2 direction, Rigidbody2D rb)
    {
        if (obstacle.enabled)
            return;

        if (!direction.normalized.Equals(direction))
            direction = direction.normalized;

        rb.velocity = direction * MoveSpeed;

        isMoving = rb.velocity.magnitude > 0.2f;

        if (direction != Vector2.zero)
            lastNonZeroDirection = direction;

        SetSpriteDirection(lastNonZeroDirection);

        animator.SetBool("isMoving", isMoving);
    }

    public void SetSpriteDirection(Vector2 direction)
    {
        if (direction.y != 0)
            animator.SetFloat("Y", direction.y);

        Vector3 scale = pivot.gameObject.transform.localScale;

        if ((direction.x > 0.5 && scale.x > 0) || (direction.x < -0.5 && scale.x < 0))
            scale.x *= -1;
        
        pivot.gameObject.transform.localScale = scale;
    }

    public void ResetSpriteDirection()
    {
        lastNonZeroDirection = new Vector2(0, 0);
    }

    public Animator GetAnimator()
    {
        return animator;
    }
    
    //forse nei figli
    public IEnumerator Attack()
    {

        isActioning= true;

        if (panicAttack)
        {
            panicAttack = false;
        }

        ActivateObstacle();

        switch (enemyType)
        {
            case EnemyType.melee:
                animator.SetTrigger("Attack");
                yield return new WaitForSeconds(attackDelay);
                isActioning = false;
                break;

            case EnemyType.ranged:
                playerInRange =  new List<PlayerCharacter>(AttackRangeTrigger.GetPlayersDetected());

                for (int i = 0; i < numberOfConsecutiveShoot; i++)
                {

                    
                    animator.SetTrigger("Attack");

                    Projectile newProjectile = ProjectilePool.Instance.GetProjectile();

                    newProjectile.transform.position = transform.position;

                    selectedPlayerInRange = playerInRange[Random.Range(0, playerInRange.Count)];



                    Vector2 direction =  selectedPlayerInRange.transform.position-transform.position ;

                    newProjectile.Inizialize(direction, projectileRange, projectileSpeed, 1, damage, gameObject.layer);
                    yield return new WaitForSeconds(attackDelay);

                    
                }

                

                yield return new WaitForSeconds(attackSpeed);

                actionCourotine = null;

                isActioning = false;

                
                break;
        }


        
        
    }

    public override void OnParryNotify(Character whoParried)
    {
        base.OnParryNotify(whoParried);
        
        stateMachine.SetState(parriedState);
    }

    public void Parry()
    {
        //SetHitMaterialColor(_OnParryColor);
        //stateMachine.SetState(parriedState);
    }


    public override void TargetSelection()
    {
        base.TargetSelection();
    }

    //public void SetTarget(Transform newTarget)
    //{
    //    target = newTarget;

    //    isActioning = true;

    //    animator.SetTrigger("Attack");

    //    yield return new WaitForSeconds(attackSpeed);
    //    isActioning = false;
    //}

    public override void TakeDamage(DamageData data)
    {
        if (!isDead)
        {
            base.TakeDamage(data);
            if (currentHp <= 0)
            {
                isDead = true;
                
                stateMachine.SetState(deathState);
            }
            else
            {
                Character dealer = data.dealer as Character;

                if (dealer != null)
                {
                    SetTarget(dealer.dealerTransform);
                }

                if (stateMachine.CurrentState.ToString() == stunState.ToString())
                {
                    Debug.Log("son gia stunnato");
                    return;
                }

                if (actionCourotine != null)
                {
                    StopCoroutine(actionCourotine);
                    actionCourotine = null;
                }

                stateMachine.SetState(stunState);
            }
        }
       
    }



    public void DamagedAnimationEndedEvent()
    {
        animator.SetTrigger("DamageEnded");

        //per ora da problemi
        //stateMachine.SetState(moveState);
    }



    public override void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if(newTarget.TryGetComponent<PlayerCharacter>(out PlayerCharacter player))
            currentTarget = player;
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

    public void SetActionCoroutine(Coroutine coroutine) 
    {
        actionCourotine=coroutine;
    }

   

}
