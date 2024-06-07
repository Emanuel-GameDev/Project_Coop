using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    [SerializeField] public float stunTime = 1;


    [Header("Variabili di base")]
    [SerializeField] public float viewRange = 2f;
    [SerializeField] public float attackRange = 1;
    [SerializeField] public float changeTargetRange = 2f;
    [SerializeField] internal float attackDelay = 1;
    [SerializeField] public float despawnTime = 1;

    [SerializeField] internal EnemyType enemyType;
    internal bool readyToAttack = false; 
    

    NavMeshPath path;

    [Header("Pivot")]
    [SerializeField] internal Transform pivot;

    public StateMachine<BasicEnemyState> stateMachine { get; } = new();

    //di prova
    //[SerializeField] Transform tryTarget;

    //[SerializeField] public float closeRange = 1;

    internal Coroutine actionCourotine;

    Vector2 lastNonZeroDirection;



    //[HideInInspector] public BasicEnemyIdleState idleState;
    //[HideInInspector] public BasicEnemyMoveState moveState;
    //[HideInInspector] public BasicEnemyActionState actionState;
    [HideInInspector] public BasicEnemyDeathState deathState;
    [HideInInspector] public BasicEnemyStunState stunState;
    [HideInInspector] public BasicEnemyParriedState parriedState;
    //[HideInInspector] public BasicRangedEnemyEscapeState escapeState;

    [HideInInspector] public bool AIActive = true;

    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool isActioning = false;

    [HideInInspector] public bool canSee = true;
    [HideInInspector] public bool canMove = false;
    [HideInInspector] public bool canAction = false;

    [Header("Detectors")]
    [SerializeField] public Detector viewTrigger;
    [SerializeField] public Detector AttackRangeTrigger;
    [SerializeField] public Detector ChangeTargetTrigger;
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


    [Header("Context steering")]
    Coroutine movementCountDownCoroutine;

    Vector2 move;

    internal Coroutine fleeCoroutine;
    internal bool flee = false;

    internal bool isRunning = false;
    [SerializeField] float repathInterval = 1;

    public float strenghtMultiplier = 1;
    [SerializeField] float steeringSpeed = 1;

    //strafe
    public bool strafe = false;
    [SerializeField] float minStrafe = 0;
    [SerializeField] float maxStrafe = 1;

    [SerializeField] float minStrafeInterval = 2;
    [SerializeField] float maxStrafeInterval = 2;

    [SerializeField] float minStrafeTime = 4;
    [SerializeField] float maxStrafeTime = 4;

    bool strafeAllowed = true;

    float moveStrafeIntervall = 3;
    float moveAllowedTime = 3;

    //maps
    internal ContextSteeringDirection[] chaseBehaviour;
    internal ContextSteeringDirection[] avoidBehaviour;
    internal ContextSteeringDirection[] finalBehaviour;

    //directions
    [SerializeField] int contextSteeringDirectionCount = 8;
    internal Vector2 targetDirection;
    internal Vector2 finalDirection;
    internal Vector2 lastFinalDirection;
    internal float finalStrenght;

    //follow
    [SerializeField] float csChaseDistance = 5;
    [SerializeField] LayerMask csChaseLayerMask;

    //avoidance
    [SerializeField] float csAvoidDistance = 5;
    [SerializeField] LayerMask csAvoidLayerMask;
    [SerializeField] float obstacleAvoidanceCloseRange = 1;
    [SerializeField] float obstacleAvoidanceFarRange = 1;

    float obstacleAvoidancePriority = 1;



    //bool gizmo
    public bool showDirections = false;

    public bool showLikedDirections = false;
    public bool showUnlikedDirections = false;
    public bool showCounterAvoid = false;
    public bool showFinalDirections = false;

    public bool showBestDirections = false;
    public bool showMoveDirections = false;



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


        chaseBehaviour = new ContextSteeringDirection[contextSteeringDirectionCount];
        avoidBehaviour = new ContextSteeringDirection[contextSteeringDirectionCount];
        finalBehaviour = new ContextSteeringDirection[contextSteeringDirectionCount];

        //finalDirection = new ContextSteeringDirection();

        lastFinalDirection = Vector2.zero;

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


    protected override void Awake()
    {
        base.Awake();

        obstacle = GetComponent<NavMeshObstacle>();

        obstacle.enabled = false;
        obstacle.carveOnlyStationary = false;
        obstacle.carving = true;

        stunState = new BasicEnemyStunState(this);
        parriedState = new BasicEnemyParriedState(this);
        deathState = new BasicEnemyDeathState(this);

        path = new NavMeshPath();
    }


    protected virtual void Start()
    {
        viewTrigger.GetComponent<CircleCollider2D>().radius = viewRange;
        AttackRangeTrigger.GetComponent<CircleCollider2D>().radius = attackRange;
        ChangeTargetTrigger.GetComponent<CircleCollider2D>().radius = changeTargetRange;

        move = Vector2.zero;

        //if(canGoIdle)
        //stateMachine.SetState(idleState);       
    }

    protected virtual void Update()
    {
        if (AIActive)
            stateMachine.StateUpdate();
    }


    //public void GoToPosition(Transform pos)
    //{
    //    //if (!canMove)
    //    //{
    //    //    rb.velocity = Vector2.zero;
    //    //    return;
    //    //}
    //    ActivateAgent();
       
    //    if (pos != null)
    //    {         
    //         SetTarget(pos);
    //        //Move((Vector3)pos -transform.position,rb);
    //        FollowPath();
    //        Debug.Log("one");
    //    }
    //    else
    //    {
    //        rb.velocity = Vector2.zero;
    //    }
    //}

    //IEnumerator ReachPosition(Transform positionToReach)
    //{
    //        FollowPath();
    //}

   
    public virtual void AwayPath()
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (!flee)
        {
            fleeCoroutine = StartCoroutine(CalculateRunAwayPathAndSteering());
        }

        move = Vector2.zero;
        finalDirection.Normalize();
        move = (finalDirection/**(finalStrenght*inc)*/)/* + targetDirection*/;

        Move(move, rb);
       
    }
    public virtual void FollowPath()
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }


        if (!isRunning)
        {
            if(!strafe)
                StartCoroutine(CalculateChasePathAndSteering());
            else
            {
                StartCoroutine(CalculateStrafePathAndSteering());
            }
        }
       
         move = Vector2.zero;
         move = (finalDirection.normalized/**(finalStrenght*inc)*/)/* + targetDirection*/;

        if(strafe && !strafeAllowed)
        {
            move = Vector2.zero;
        }

        Move(move, rb);
    }

   

    public void StartStopMovementCountdownCoroutine(bool start)
    {
        if (start)
        {
            movementCountDownCoroutine = StartCoroutine(MovementCountdown(
                UnityEngine.Random.Range(minStrafeInterval, maxStrafeInterval),
                UnityEngine.Random.Range(minStrafeTime, maxStrafeTime)));
        }
        else 
        {
            if(movementCountDownCoroutine != null)
                StopCoroutine(movementCountDownCoroutine);
        }
    }

    internal IEnumerator MovementCountdown(float blockInterval,float moveAllowedTime)
    {
        
        strafeAllowed = false;
        yield return new WaitForSeconds(blockInterval);
        strafeAllowed = true;
        yield return new WaitForSeconds(moveAllowedTime);

        StartStopMovementCountdownCoroutine(true);
    }

    internal IEnumerator CalculateStrafePathAndSteering()
    {
        isRunning = true;
        CalculateTargetDirection();
        CalculateTargetDistance();

        CalculateContextSteeringInterestMap(chaseBehaviour,minStrafe, maxStrafe);

        CalculateContextSteeringAvoidMap(avoidBehaviour, csAvoidDistance, csAvoidLayerMask);

        //InvertDirectionsContextSteeringMap(avoidBehaviour);

        SubSteeringMaps(chaseBehaviour, avoidBehaviour);

        InvertNegativeStrenghtContextSteeringMap(finalBehaviour);

        CalculateFinalDirection();

        

        yield return new WaitForSeconds(repathInterval);

        strafe = false;
        isRunning = false;

        //if (!strafe)
        //{
        //    StartCoroutine(CalculateChasePathAndSteering());
        //}
        //else
        //{
        //    StartCoroutine(CalculateStrafePathAndSteering());
        //}
    }

    internal IEnumerator CalculateChasePathAndSteering()
    {
        isRunning = true;
        CalculateTargetDirection();
        CalculateTargetDistance();
        CalculateContextSteeringInterestMap(chaseBehaviour, 0, 1);

        CalculateContextSteeringAvoidMap(avoidBehaviour, csAvoidDistance, csAvoidLayerMask);

        SubSteeringMaps(chaseBehaviour, avoidBehaviour);

        InvertNegativeStrenghtContextSteeringMap(finalBehaviour);

        CalculateFinalDirection();

        yield return new WaitForSeconds(repathInterval);
        strafe = false;
        isRunning = false;
        //if (!strafe)
        //    StartCoroutine(CalculateChasePathAndSteering());
        //else
        //{
        //    StartCoroutine(CalculateStrafePathAndSteering());
        //}


    }

    internal IEnumerator CalculateRunAwayPathAndSteering()
    {
        isRunning = true;
        flee = true;
        CalculateTargetDirection();

        CalculateContextSteeringInterestMap(chaseBehaviour,0,1);
        InvertDirectionsContextSteeringMap(chaseBehaviour);

        CalculateContextSteeringAvoidMap(avoidBehaviour, csAvoidDistance, csAvoidLayerMask);

        SubSteeringMaps(chaseBehaviour,avoidBehaviour);

        InvertNegativeStrenghtContextSteeringMap(finalBehaviour);

        CalculateFinalDirection();


        yield return new WaitForSeconds(repathInterval);
        //fleeCoroutine = StartCoroutine(CalculateRunAwayPathAndSteering());
        flee = false;
    }


    public float inc = 1;
    public virtual void FollowPosition(Vector2 pos)
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        
        if (!isRunning)
        {
            if (!strafe)
                StartCoroutine(CalculateChasePathAndSteering());
            else
                StartCoroutine(CalculateStrafePathAndSteering());

        }

        move = Vector2.zero;
        move = (finalDirection.normalized/**(finalStrenght*inc)*/)/* + targetDirection*/;

        Move(move, rb);

    }

    #region cs

    
    public virtual void CalculateTargetDirection()
    {
        if (ChangeTargetTrigger.GetPlayersCountInTrigger() > 0)
        {
            if (ChangeTargetTrigger.GetPlayersCountInTrigger() > 1)
            {
                PlayerCharacter[] listCopy = ChangeTargetTrigger.GetPlayersDetected().ToArray();

                Transform tMin = null;
                    float minDist = Mathf.Infinity;
                    Vector3 currentPos = transform.position;
                foreach (PlayerCharacter p in listCopy)
                {

                    float dist = Vector3.Distance(p.transform.position, currentPos);

                    if (dist < minDist)
                    {
                        tMin = p.transform;
                        minDist = dist;
                    }
                }


                SetTarget(tMin);
            }
            else
                SetTarget(ChangeTargetTrigger.GetPlayersDetected()[0].transform);
        }

        Vector2 agentDir = Vector2.zero;
        

        if (currentTarget != null && agent.isActiveAndEnabled)
        {
            if (agent.CalculatePath(target.position, path))
            {
                if (path.corners.Length > 1)
                    agentDir = path.corners[1] - path.corners[0];
                else
                    agentDir = currentTarget.transform.position - groundLevel.position;

            }

        }

        targetDirection = agentDir;
    }

    public virtual void CalculateTargetDistance()
    {
        if (currentTarget != null)
        {
            RaycastHit2D[] raycast = new RaycastHit2D[1];
            Physics2D.RaycastNonAlloc(groundLevel.position, (currentTarget.transform.position - groundLevel.position), raycast, csChaseDistance, csChaseLayerMask);


            if (raycast[0].collider != null)
            {
                if (raycast[0].distance < attackRange + 3)
                {
                    obstacleAvoidancePriority = obstacleAvoidanceCloseRange;
                    strafe = true;
                }
                else
                {
                    strafe = false;
                    obstacleAvoidancePriority = obstacleAvoidanceFarRange;
                }
            }
            else
            {
                strafe = false;
                obstacleAvoidancePriority = obstacleAvoidanceFarRange;
            }
        }
    }

    public virtual void CalculateContextSteeringInterestMap(ContextSteeringDirection[] likedDirections,float minDotSearched, float maxDotSearched)
    {
        if (target == null)
        {
            return;
        }


        for (int i = 0; i < contextSteeringDirectionCount; i++)
        {
            likedDirections[i].SetRelativePos(new Vector2(groundLevel.position.x + likedDirections[i].direction.x, groundLevel.position.y + likedDirections[i].direction.y));

            Vector2 localDir = new Vector2(groundLevel.position.x + targetDirection.x, groundLevel.position.y + targetDirection.y); 

            float currentDot = Vector2.Dot(new Vector2(localDir.x-groundLevel.position.x, localDir.y - groundLevel.position.y).normalized, likedDirections[i].GetRelativeDirection(groundLevel).normalized);


            if (currentDot >= minDotSearched && currentDot <= maxDotSearched)
            {
                float powerDirection = MathF.Sin(currentDot);
                likedDirections[i].SetDirectionStrenght(MathF.Abs(powerDirection));
            }
            else
                likedDirections[i].SetDirectionStrenght(0);

            
        }
    }
    

    public virtual void CalculateContextSteeringAvoidMap(ContextSteeringDirection[] directionsToAvoid,float distanceToConsider,LayerMask layerToAvoid)
    {
       
        for (int i = 0; i < contextSteeringDirectionCount; i++)
        {
            directionsToAvoid[i].SetRelativePos(new Vector2(groundLevel.position.x + directionsToAvoid[i].direction.x, groundLevel.position.y + directionsToAvoid[i].direction.y));
            
            RaycastHit2D[] raycast = new RaycastHit2D[2];

            //if(chaseBehaviour[i].directionStrenght>=0)
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
    
    public virtual void SubSteeringMaps(ContextSteeringDirection[] map, ContextSteeringDirection[] mapToSubtract)
    {
        for (int i = 0; i < contextSteeringDirectionCount; i++)
        {
            finalBehaviour[i].SetRelativePos(new Vector2(groundLevel.position.x + finalBehaviour[i].direction.x, groundLevel.position.y + finalBehaviour[i].direction.y));


            float strenght = map[i].directionStrenght - mapToSubtract[i].directionStrenght;

            finalBehaviour[i].SetDirectionStrenght(strenght);
        }
    }

    public virtual void SumSteeringMaps(ContextSteeringDirection[] map1, ContextSteeringDirection[] map2)
    {
        for (int i = 0; i < contextSteeringDirectionCount; i++)
        {
            finalBehaviour[i].SetRelativePos(new Vector2(groundLevel.position.x + finalBehaviour[i].direction.x, groundLevel.position.y + finalBehaviour[i].direction.y));


            float strenght = map1[i].directionStrenght + map2[i].directionStrenght;

            finalBehaviour[i].SetDirectionStrenght(strenght);
        }
    }

    
    public virtual void InvertNegativeStrenghtContextSteeringMap(ContextSteeringDirection[] mapToInvert)
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

    public virtual void InvertDirectionsContextSteeringMap(ContextSteeringDirection[] mapToInvert)
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
    
    public virtual void CalculateFinalDirection()
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
        finalDirection = avarenge ;
        finalStrenght = avarangeStrenght / div;

        bool allow = false;

        Vector2 localDir = new Vector2(groundLevel.position.x + finalDirection.x, groundLevel.position.y + finalDirection.y);
        float currentDot = Vector2.Dot(new Vector2(localDir.x - groundLevel.position.x, localDir.y - groundLevel.position.y).normalized, finalBehaviour[bestId].GetRelativeDirection(groundLevel).normalized);


        if (currentDot < 0f)
            finalDirection = finalDirection + finalBehaviour[bestId].GetRelativeDirection(groundLevel);
        
        if( lastFinalDirection != Vector2.zero)
        {
            finalDirection += (lastFinalDirection/(steeringSpeed/10));
        }

        lastFinalDirection = finalDirection;
    }
    
    #endregion

    public virtual void Move(Vector2 direction, Rigidbody2D rb)
    {
        if(direction== null) return;
        
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




        float speed = normalMovement ? MoveSpeed : MoveSpeed;
        
        rb.velocity = direction * speed;
        
        isMoving = true;

        if (targetDirection != Vector2.zero)
            lastNonZeroDirection = targetDirection;

        SetSpriteDirection(lastNonZeroDirection);

        //animator.SetBool("isMoving", isMoving);
    }
    bool normalMovement = true;
    public void AccellerateMovement()
    {
        normalMovement = true;
    }

    public void DecellerateMovement()
    {
        normalMovement = false;
    }

    public virtual void SetSpriteDirection(Vector2 direction)
    {
        if (direction.y != 0)
            animator.SetFloat("Y", direction.y);

        if(rb.velocity != Vector2.zero)
        {
            float dot = Vector2.Dot(direction, rb.velocity);

            if(dot>=0)
                animator.SetFloat("X", 1f);
            else
                animator.SetFloat("X", -1f);
        }

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
    public virtual IEnumerator Attack()
    {
        StopCoroutine(CalculateChasePathAndSteering());
        isRunning = false;

        isActioning = true;

        //if (panicAttack)
        //{
        //    panicAttack = false;
        //}

        ActivateObstacle();


        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(attackDelay);
        isActioning = false;
        

    }
        
        
    

    public override void OnParryNotify(Character whoParried)
    {
        base.OnParryNotify(whoParried);
        
        stateMachine.SetState(parriedState);
    }

    //public void Parry()
    //{
    //    //SetHitMaterialColor(_OnParryColor);
    //    stateMachine.SetState(parriedState);
    //}


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

                //if (stateMachine.CurrentState.ToString() == stunState.ToString())
                //{
                //    Debug.Log("son gia stunnato");
                //    return;
                //}

                if (actionCourotine != null)
                {
                    StopCoroutine(actionCourotine);
                    actionCourotine = null;
                }

                stateMachine.SetState(stunState);
            }
        }
       
    }

    public virtual void SetIdleState()
    {

    }



    //public void DamagedAnimationEndedEvent()
    //{
    //    animator.SetTrigger("DamageEnded");

    //    //per ora da problemi
    //    //stateMachine.SetState(moveState);
    //}



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

    public void ResetVelocity()
    {
        rb.velocity = Vector3.zero;
    }


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

                if (showCounterAvoid)
                {
                    Gizmos.color = Color.yellow; ;
                    Gizmos.DrawLine(groundLevel.position, new Vector2(
                        groundLevel.position.x + (rb.velocity.x),
                        groundLevel.position.y + rb.velocity.y));

                }

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
