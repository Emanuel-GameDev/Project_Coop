using UnityEngine;
using UnityEngine.AI;

public class LabirintEnemy : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f;
    public float maxFollowDistance = 15f;
    public Grid grid;
    protected Vector2 destination;

    private EnemyTargetDetection targetDetection;
    private Transform target;
    private NavMeshAgent agent;
    private Vector2 agentDestination;
    private NavMeshPath navMeshPath;

    private static int debugCountMax = 50;
    private int debugCount = 0;

    public float minRandomDestinationDistance = 2f;
    public float maxRandomDestinationDistance = 10f;

    public float MaxDistance => maxRandomDestinationDistance * grid.cellSize.x;
    public float MinDistance => minRandomDestinationDistance * grid.cellSize.x;
    public float MaxFollowDistance => maxFollowDistance * grid.cellSize.x;

    protected void Start()
    {
        targetDetection = GetComponentInChildren<EnemyTargetDetection>();
        grid = LabirintManager.Instance.Grid;
        destination = transform.position;
        agentDestination = transform.position;
        InizializeAgent();
    }

    private void InizializeAgent()
    {
        agent = GetComponent<NavMeshAgent>();
        navMeshPath = new NavMeshPath();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        agent.updateRotation = false;
        SetRandomDestination();
        Navigate();
    }

    protected void Update()
    {
        CheckTarget();
        HandlePathfindingAndMovement();
    }

    private void CheckTarget()
    {
        if (target != null)
        {
            if (NavMesh.SamplePosition(target.position, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
            {
                agentDestination = hit.position;
            }
        }
        if (Vector2.Distance(transform.position, agentDestination) > MaxFollowDistance)
        {
            target = null;
            SetRandomDestination();
        }
    }

    private void SetRandomDestination()
    {
        bool founded = false;

        while(!founded)
        {
            float randomDistance = Random.Range(MinDistance, MaxDistance);
            Vector2 randomPoint = transform.position + Random.onUnitSphere * randomDistance;
            founded = NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, randomDistance, NavMesh.AllAreas);
            if(founded)
                agentDestination = grid.GetCellCenterWorld(grid.WorldToCell(hit.position));
        }

        debugCount = 0;
    }

    private void HandlePathfindingAndMovement()
    {
        //bool hasReachDestination = Vector2.Distance(transform.position, AgentGridDestination(agentDestination)) < 0.01f;
        bool hasReachCenter = Vector2.Distance(transform.position, destination) < 0.01f;

        //if (hasReachDestination)
        //{
        //    SetRandomDestination();
        //}
        //else
        //{
        if (!hasReachCenter)
            debugCount++;
        if (debugCount > debugCountMax)
        {
            SetRandomDestination();
            hasReachCenter = true;
        }
            
    //}

        if (hasReachCenter)
        {
            Navigate();
        }

        transform.position = Vector2.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

        //Debug.Log($"Has Reach Destination: {hasReachDestination}, Has Reach Center: {hasReachCenter}, count: {debugCount}");
        Debug.Log($"Has Reach Center: {hasReachCenter}, count: {debugCount}");
    }

    private void Navigate()
    {
        NavMesh.CalculatePath(transform.position, agentDestination, NavMesh.AllAreas, navMeshPath);
        if (navMeshPath.corners.Length > 1)
        {
            destination = AgentGridDestination(navMeshPath.corners[1]);
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
            target = null;
        }
    }


    public void SetTarget(Transform target)
    {
        if (this.target == null)
            this.target = target;
    }

}
