using UnityEngine;
using UnityEngine.AI;

public class LabirintEnemy : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f;
    public float maxFollowDistance = 15f;
    public Grid grid;

    private EnemyTargetDetection targetDetection;
    private NavMeshAgent agent;
    private Vector2 finalDestination;
    private Vector2 currentDestination;
    private NavMeshPath navMeshPath;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private readonly int debugCountMax = 50;
    private int debugCount = 0;

    private Transform target;
    private Transform Target
    {
        get => target;
        set
        {
            target = value;
            if (target)
            {
                alert.SetActive(true);
            }
            else
            {
                alert.SetActive(false);
            }
        }
     }

    public float minRandomDestinationDistance = 2f;
    public float maxRandomDestinationDistance = 10f;

    [SerializeField]
    private GameObject alert;

    public float MaxDistance => maxRandomDestinationDistance * grid.cellSize.x;
    public float MinDistance => minRandomDestinationDistance * grid.cellSize.x;
    public float MaxFollowDistance => maxFollowDistance * grid.cellSize.x;

    protected void Start()
    {
        targetDetection = GetComponentInChildren<EnemyTargetDetection>();
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("isMoving", true);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        grid = LabirintManager.Instance.Grid;
        currentDestination = transform.position;
        finalDestination = transform.position;
        InizializeAgent();
    }

    private void InizializeAgent()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        navMeshPath = new NavMeshPath();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        
        SetRandomDestination();
        CalculateCurrentDestination();
        if(transform.rotation != Quaternion.identity)
            transform.rotation = Quaternion.identity;
    }

    protected void FixedUpdate()
    {
        HandleAnimationAndFlip();
        CheckTarget();
        HandlePathfindingAndMovement();
    }

    private void CheckTarget()
    {
        if (Target != null)
        {
            if (NavMesh.SamplePosition(Target.position, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
            {
                finalDestination = hit.position;
            }

            if (Vector2.Distance(transform.position, Target.position) > MaxFollowDistance)
            {
                Target = null;
                SetRandomDestination();
            }
        }
    }

    private void SetRandomDestination()
    {
        bool founded = false;

        while(!founded || debugCount < debugCountMax)
        {
            float randomDistance = Random.Range(MinDistance, MaxDistance);
            Vector2 randomPoint = transform.position + Random.onUnitSphere * randomDistance;
            founded = NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, randomDistance, NavMesh.AllAreas);
            if(founded)
                finalDestination = grid.GetCellCenterWorld(grid.WorldToCell(hit.position));
            debugCount++;
        }

        if(debugCount > debugCountMax)
        {
            Debug.LogError("Error in SetRandomDestination");
        }

        debugCount = 0;
    }

    private void HandlePathfindingAndMovement()
    {
        bool hasReachFinalDestination = Vector2.Distance(transform.position, AgentGridDestination(finalDestination)) < 0.01f;
        bool hasReachCurrentDestination = Vector2.Distance(transform.position, currentDestination) < 0.01f;

        if (hasReachFinalDestination)
        {
            SetRandomDestination();
            //Debug.Log("Reach Final Destination");
        }

        if (hasReachCurrentDestination || Target != null)
        {
            CalculateCurrentDestination();
            //Debug.Log("Reach Current Destination");
        }

        transform.position = Vector2.MoveTowards(transform.position, currentDestination, moveSpeed * Time.deltaTime);

        //Debug.Log($"Has Reach Destination: {hasReachFinalDestination}, Has Reach Center: {hasReachCurrentDestination}, count: {debugCount}");
    }

    private void CalculateCurrentDestination()
    {
        NavMesh.CalculatePath(transform.position, finalDestination, NavMesh.AllAreas, navMeshPath);
        if (navMeshPath.corners.Length > 1)
        {
            currentDestination = AgentGridDestination(navMeshPath.corners[1]);
        }
    }

    private Vector2 AgentGridDestination(Vector2 destination)
    {
        Vector2 cellDestination = grid.GetCellCenterWorld(grid.WorldToCell(destination));

        return cellDestination;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.TryGetComponent<LabirintPlayer>(out var player))
        {
            player.Killed();
            Target = null;
        }
    }


    public void SetTarget(Transform target)
    {
        if (this.Target == null)
        {
            this.Target = target;
        }
           
        //Debug.Log($"thisTarget: {this.target}, newTarget: {target}");
    }

    private void HandleAnimationAndFlip()
    {
        if (animator != null && spriteRenderer != null)
        {
            //bool isMoving = (Vector2.Distance(transform.position, currentDestination) > 0.01f);

            //animator.SetBool("isMoving", isMoving);

            if (transform.position.x > currentDestination.x)
            {
                spriteRenderer.flipX = false;
            }
            else if (transform.position.x < currentDestination.x)
            {
                spriteRenderer.flipX = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(grid != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(finalDestination, 1.2f * grid.cellSize.x);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(currentDestination, 1 * grid.cellSize.x);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, maxFollowDistance * grid.cellSize.x);
        }      
    }


}
