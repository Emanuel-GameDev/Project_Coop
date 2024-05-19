using System;
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

    NavMeshPath path;

    [Header("Pivot")]
    [SerializeField] Transform pivot;

    public StateMachine<BasicEnemyState> stateMachine { get; } = new();

    //di prova
    //[SerializeField] Transform tryTarget;

    //[SerializeField] public float closeRange = 1;
   
    

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

   
    public struct ContextSteeringDirection
    {
        public Vector2 direction;
        public Vector2 relativeDirection;
        public Vector2 relativePos;
        public float directionStrenght;

        public void Initialize()
        {
            this.direction = new Vector2();
            directionStrenght = 0;
        }

        public void SetDirection(Vector2 direction)
        {
            this.direction = direction;
        }

        public void SetRelativePos(Vector2 pos)
        {
            relativePos = pos;
        }

        public void SetDirectionStrenght(float strenght)
        {
            directionStrenght = strenght;
        }

        public Vector2 GetRelativeDirection(Transform relativeTransform)
        {
            return relativeTransform.InverseTransformVector(direction);
        }
    }

    protected override void InitialSetup()
    {
        base.InitialSetup();
        animator = GetComponent<Animator>();

        if(GetComponent<NavMeshAgent>() != null)
             agent = GetComponent<NavMeshAgent>();

        currentHp = maxHp;

        chaseBehaviour = new ContextSteeringDirection[contextSteeringDirectionCount];
        avoidBehaviour = new ContextSteeringDirection[contextSteeringDirectionCount];
        finalBehaviour = new ContextSteeringDirection[contextSteeringDirectionCount];

        //finalDirection = new ContextSteeringDirection();



        for (int i = 0; i < contextSteeringDirectionCount; i++)
        {
            chaseBehaviour[i].Initialize();
            avoidBehaviour[i].Initialize();

            double radians = 2 * Math.PI / contextSteeringDirectionCount * i;

            float vertical = MathF.Sin((float)radians);
            float horizontal = MathF.Cos((float)radians);
            

            Vector2 dir = new Vector2(horizontal, vertical);
            
            Vector2 localDir = groundLevel.InverseTransformDirection(dir);
            chaseBehaviour[i].SetDirection(localDir);
            avoidBehaviour[i].SetDirection(localDir);
            finalBehaviour[i].SetDirection(localDir);


            
        }
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
            //Move((Vector3)pos -transform.position,rb);          
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
        if (AIActive)
            stateMachine.StateUpdate();

        
    }



    Vector2 move;
    public void FollowPath()
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        //ActivateAgent();

        //if (currentTarget != null)
        //{
        //    CalculateContextSteering();
        //    //if (agent.CalculatePath(target.position, path))
        //    //{
        //    //    //    if (path.corners.Length > 1)
        //    //    //Move(path.corners[1] - path.corners[0], rb);
        //    //    //else
        //    //    //    Move(target.position - transform.position, rb);

        //    //}
        //    //else
        //    //{
        //    //    rb.velocity = Vector2.zero;
        //    //}
        //}
        //else
        //{
        //    rb.velocity = Vector2.zero;
        //}

        if(!isRunning )
        StartCoroutine(CalculateChasePathAndSteering());

         move = Vector2.zero;
         move = (finalDirection*(finalStrenght*inc))/* + targetDirection*/;
        Move(move, rb);
    }
    bool isRunning = false;
    public float delay = 1;
    IEnumerator CalculateChasePathAndSteering()
    {
        isRunning = true;
        CalculateTargetDirection();

        CalculateContextSteeringInterestMap(chaseBehaviour);

        CalculateContextSteeringAvoidMap(avoidBehaviour, csAvoidDistance, csAvoidLayerMask);

        CalculateContextSteeringMapSummary();

        InvertNegativeStrenghtContextSteeringMap(finalBehaviour);

        CalculateFinalDirection();

        yield return new WaitForSeconds(delay);
        StartCoroutine(CalculateChasePathAndSteering());
    }

    IEnumerator CalculateRunAwayPathAndSteering()
    {
        isRunning = true;

        CalculateTargetDirection();

        CalculateContextSteeringInterestMap(chaseBehaviour);
        InvertDirectionsContextSteeringMap(chaseBehaviour);

        CalculateContextSteeringAvoidMap(avoidBehaviour, csAvoidDistance, csAvoidLayerMask);

        CalculateContextSteeringMapSummary();

        InvertNegativeStrenghtContextSteeringMap(finalBehaviour);

        CalculateFinalDirection();


        yield return new WaitForSeconds(delay);
        StartCoroutine(CalculateRunAwayPathAndSteering());
    }


    public float inc = 1;
    public void FollowPosition(Vector2 pos)
    {
        //if (!canMove)
        //{
        //    rb.velocity = Vector2.zero;
        //    return;
        //}

        //ActivateAgent();

        //if (pos != null)
        //{
        //    if (agent.CalculatePath(pos, path))
        //    {

        //        //if (path.corners.Length > 1)
        //           // Move(path.corners[1] - path.corners[0], rb);
        //        //else
        //        //    Move((Vector3)pos - transform.position, rb);

        //    }
        //    else
        //    {
        //        rb.velocity = Vector2.zero;
        //    }
        //}
        //else
        //{
        //    rb.velocity = Vector2.zero;
        //}

    }

    #region cs

    public bool showDirections = false;
    public int contextSteeringDirectionCount = 8;

    internal ContextSteeringDirection[] chaseBehaviour;
    internal ContextSteeringDirection[] avoidBehaviour;
    internal ContextSteeringDirection[] finalBehaviour;

    internal Vector2 targetDirection;
    internal Vector2 finalDirection;
    internal float finalStrenght;

    public float csChaseDistance = 5;
    public LayerMask csChaseLayerMask;

    public float obstacleAvoidancePriority = 1;
    public float csAvoidDistance = 5;
    public LayerMask csAvoidLayerMask;


    public void CalculateTargetDirection()
    {
        Vector2 agentDir = Vector2.zero;

        if (currentTarget != null)
        {
            if (agent.CalculatePath(target.position, path))
            {
                //    if (path.corners.Length > 1)
                agentDir = path.corners[1] - path.corners[0];
                //else
                //    Move(target.position - transform.position, rb);

            }

        }

        targetDirection = agentDir;
    }

    public void CalculateContextSteeringInterestMap(ContextSteeringDirection[] likedDirections)
    {
        if (target == null)
        {
            return;
        }

        int indexToSubstitute = -1;
        float bestDot = 0;
        

        for (int i = 0; i < contextSteeringDirectionCount; i++)
        {
            likedDirections[i].SetRelativePos(new Vector2(groundLevel.position.x + likedDirections[i].direction.x, groundLevel.position.y + likedDirections[i].direction.y));

            Vector2 localDir = new Vector2(groundLevel.position.x + targetDirection.x, groundLevel.position.y + targetDirection.y); 

            float currentDot = Vector2.Dot(new Vector2(localDir.x-groundLevel.position.x, localDir.y - groundLevel.position.y).normalized, likedDirections[i].GetRelativeDirection(groundLevel).normalized);


            //forse non serve
            if (indexToSubstitute < 0)
            {
                indexToSubstitute = i;
                bestDot = currentDot;
            }
            else
            {
                if (bestDot < currentDot)
                {
                    indexToSubstitute = i;
                    bestDot = currentDot;
                }
            }

            if (currentDot >= 0)
            {
                float powerDirection = MathF.Sin(currentDot);
                likedDirections[i].SetDirectionStrenght(powerDirection);
            }
            else
                likedDirections[i].SetDirectionStrenght(0);
        }
        favorite = new ContextSteeringDirection();
        favorite = likedDirections[indexToSubstitute];
        //Move(chaseBehaviour[indexToSubstitute].GetRelativeDirection(groundLevel), rb);
        //RaycastHit2D raycast = Physics2D.Raycast(groundLevel.position, chaseBehaviour[i].GetRelativeDirection(groundLevel), csChaseDistance, csChaseLayerMask);
        //Debug.Log(chaseBehaviour[indexToSubstitute].GetRelativeDirection(groundLevel));
        //Debug.Log(bestDot);
        //if (raycast.collider != null)
        //    Debug.Log(raycast.distance);

    }
    ContextSteeringDirection favorite;
    public void CalculateContextSteeringAvoidMap(ContextSteeringDirection[] directionsToAvoid,float distanceToConsider,LayerMask layerToAvoid)
    {
        for (int i = 0; i < contextSteeringDirectionCount; i++)
        {
            directionsToAvoid[i].SetRelativePos(new Vector2(groundLevel.position.x + directionsToAvoid[i].direction.x, groundLevel.position.y + directionsToAvoid[i].direction.y));
            
            RaycastHit2D[] raycast = new RaycastHit2D[2];
            Physics2D.RaycastNonAlloc(groundLevel.position, directionsToAvoid[i].GetRelativeDirection(groundLevel), raycast, distanceToConsider, layerToAvoid);


            if (raycast[1].collider != null)
            {
                float dis = distanceToConsider - raycast[1].distance;
                float per = dis / distanceToConsider;

                if (per >= 0)
                {
                    directionsToAvoid[i].SetDirectionStrenght(per * obstacleAvoidancePriority);
                }
                else
                    directionsToAvoid[i].SetDirectionStrenght(-per * obstacleAvoidancePriority);
                
            }
            else
                directionsToAvoid[i].SetDirectionStrenght(0);
        }
    }
    
    public void CalculateContextSteeringMapSummary()
    {
        for (int i = 0; i < contextSteeringDirectionCount; i++)
        {
            finalBehaviour[i].SetRelativePos(new Vector2(groundLevel.position.x + finalBehaviour[i].direction.x, groundLevel.position.y + finalBehaviour[i].direction.y));

            float strenght = chaseBehaviour[i].directionStrenght - avoidBehaviour[i].directionStrenght;

            finalBehaviour[i].SetDirectionStrenght(strenght);
        }
    }

    public float strenghtMultiplier = 1;
    private void InvertNegativeStrenghtContextSteeringMap(ContextSteeringDirection[] mapToInvert)
    {
        ContextSteeringDirection[] mapCopy = new ContextSteeringDirection[contextSteeringDirectionCount];
        mapCopy = (ContextSteeringDirection[])mapToInvert.Clone();


        int half = contextSteeringDirectionCount / 2;
        for (int i = 0; i < contextSteeringDirectionCount; i++)
        {
            float strenght = 0;
            if (mapCopy[i].directionStrenght < 0)
            {

                if (i < half)
                {
                    strenght = (MathF.Abs(mapCopy[i].directionStrenght) + mapCopy[i + half].directionStrenght) * strenghtMultiplier;
                    mapToInvert[i + half].SetDirectionStrenght(strenght);
                }
                else
                {
                    strenght = (MathF.Abs(mapCopy[i].directionStrenght) + mapCopy[i - half].directionStrenght) * strenghtMultiplier;
                    mapToInvert[i - half].SetDirectionStrenght(strenght);
                }
            }
        }
    }

    private void InvertDirectionsContextSteeringMap(ContextSteeringDirection[] mapToInvert)
    {
        ContextSteeringDirection[] mapCopy = new ContextSteeringDirection[contextSteeringDirectionCount];
        mapCopy = (ContextSteeringDirection[])mapToInvert.Clone();


        int half = contextSteeringDirectionCount / 2;
        for (int i = 0; i < contextSteeringDirectionCount; i++)
        {
            float strenght = 0;
            //if (mapCopy[i].directionStrenght < 0)
            //{

                if (i < half)
                {
                    strenght = mapCopy[i].directionStrenght;
                    mapToInvert[i + half].SetDirectionStrenght(strenght);
                }
                else
                {
                    strenght = mapCopy[i].directionStrenght;
                    mapToInvert[i - half].SetDirectionStrenght(strenght);
                }
            //}
        }
    }
    
    public void CalculateFinalDirection()
    {

        Vector2 avarenge = Vector2.zero;
        float avarangeStrenght = 0;
        int div = 0;


        Vector2 vec = Vector2.zero;
        int bestId = 0;
        float currentBest=0;

        // best
        for (int i = 0; i < contextSteeringDirectionCount; i++)
        {
            if (finalBehaviour[i].directionStrenght > currentBest)
            {
                bestId = i;
                currentBest = finalBehaviour[i].directionStrenght;
            }

        }

        ////finalDirection = finalBehaviour[bestId].GetRelativeDirection(groundLevel);
        ////finalStrenght = finalBehaviour[bestId].directionStrenght;
        //vec = finalBehaviour[bestId].GetRelativeDirection(groundLevel);


        //Avarange
        for (int i = 0; i < contextSteeringDirectionCount; i++)
        {
            if (finalBehaviour[i].directionStrenght > 0.1f)
            {
                avarenge += finalBehaviour[i].GetRelativeDirection(groundLevel);
                div++;

                avarangeStrenght += finalBehaviour[i].directionStrenght;
            }
        }
        finalDirection = avarenge / div;
        finalStrenght = avarangeStrenght / div;

        bool allow = false;

        Vector2 localDir = new Vector2(groundLevel.position.x + finalDirection.x, groundLevel.position.y + finalDirection.y);
        float currentDot = Vector2.Dot(new Vector2(localDir.x - groundLevel.position.x, localDir.y - groundLevel.position.y).normalized, finalBehaviour[bestId].GetRelativeDirection(groundLevel).normalized);


        //if (currentDot > 0.5f)
        //    allow = true;
        //else
        //{
            //finalDirection = finalDirection + finalBehaviour[bestId].GetRelativeDirection(groundLevel);
      
        //}
        //float bestDot = 0;
        //int bestDotId = -1;

        //check if good or stop
        //for (int i = 0; i < contextSteeringDirectionCount; i++)
        //{
        //    if (chaseBehaviour[i].relativeDirection == vec)
        //    {
        //        if (finalBehaviour[i].directionStrenght > 0)
        //            allow = true;
        //    }
        //}

        //if (!allow)
        //{
        //    finalDirection = Vector2.zero;
        //    finalStrenght *= 0.2f;
        //}
    }

    #endregion

    public virtual void Move(Vector2 direction, Rigidbody2D rb)
    {
        if (obstacle.enabled)
            return;
       

        //if (!direction.normalized.Equals(direction))
            direction = direction.normalized;

        rb.velocity = direction * MoveSpeed;

        isMoving = true;

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
        StopCoroutine(CalculateChasePathAndSteering());
        isRunning = false;

        isActioning = true;

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

                    selectedPlayerInRange = playerInRange[UnityEngine.Random.Range(0, playerInRange.Count)];



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
        Agent.enabled = true;
        obstacle.enabled = false;
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }

    public void SetActionCoroutine(Coroutine coroutine) 
    {
        actionCourotine=coroutine;
    }
    public bool showLikedDirections = false;
    public bool showUnlikedDirections = false;
    public bool showCounterAvoid = false;
    public bool showFinalDirections = false;

    public bool showBestDirections = false;
    public bool showMoveDirections = false;
    private void OnDrawGizmos()
    {
        Vector2 vec = transform.position;

        Gizmos.DrawWireSphere(groundLevel.position, csAvoidDistance);

        if (showDirections)
        {
            Gizmos.color = Color.yellow;

            //if(target!=null)
            //    Gizmos.DrawLine(groundLevel.position, target.position);

            Gizmos.color = Color.white;

            for (int i = 0; i < contextSteeringDirectionCount; i++)
            {
                if(showLikedDirections)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(groundLevel.position, new Vector2(
                        groundLevel.position.x + (chaseBehaviour[i].direction.x * chaseBehaviour[i].directionStrenght), 
                        groundLevel.position.y + (chaseBehaviour[i].direction.y * chaseBehaviour[i].directionStrenght)));


                }

                if (showUnlikedDirections)
                {
                    Gizmos.color = Color.red; ;
                    Gizmos.DrawLine(groundLevel.position, new Vector2(
                        groundLevel.position.x + (avoidBehaviour[i].direction.x * avoidBehaviour[i].directionStrenght),
                        groundLevel.position.y + (avoidBehaviour[i].direction.y * avoidBehaviour[i].directionStrenght)));

                }

                //if (showCounterAvoid)
                //{
                //    Gizmos.color = Color.yellow; ;
                //    Gizmos.DrawLine(groundLevel.position, new Vector2(
                //        groundLevel.position.x + (avoidBehaviour[i].direction.x * finalCopy[i].directionStrenght),
                //        groundLevel.position.y + (avoidBehaviour[i].direction.y * finalCopy[i].directionStrenght)));

                //}

                if (showFinalDirections)
                {
                    Gizmos.color = Color.white; ;
                    Gizmos.DrawLine(groundLevel.position, new Vector2(
                        groundLevel.position.x + (finalBehaviour[i].direction.x * finalBehaviour[i].directionStrenght),
                        groundLevel.position.y + (finalBehaviour[i].direction.y * finalBehaviour[i].directionStrenght)));

                }


            }
            if (showBestDirections)
            {
                    Gizmos.color = Color.cyan;
                Gizmos.DrawLine(groundLevel.position, new Vector2(
                            groundLevel.position.x + (move.x * (finalStrenght*inc)),
                            groundLevel.position.y + (move.y * (finalStrenght*inc))));

            }
            if (showMoveDirections)
            {
                Gizmos.color = Color.blue;
            Gizmos.DrawLine(groundLevel.position, new Vector2(
                        groundLevel.position.x + (finalDirection.x*finalStrenght),
                        groundLevel.position.y + (finalDirection.y*finalStrenght)));

            }



            Gizmos.color = Color.white;

        }

    }

}
