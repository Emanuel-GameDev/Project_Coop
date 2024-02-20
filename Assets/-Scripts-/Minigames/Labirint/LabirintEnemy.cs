using UnityEngine;
using UnityEngine.AI;

public class LabirintEnemy : MonoBehaviour
{
    public float maxFollowDistance = 15f;
    public Grid grid;
    protected Vector3 destination;

    private EnemyTargetDetection targetDetection;
    private Transform target;
    private NavMeshAgent agent;
    private Vector3 agentDestination;
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
        targetDetection= GetComponentInChildren<EnemyTargetDetection>();
        grid = LabirintManager.Instance.Grid;
        destination = transform.position;
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

    protected  void Update()
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
        if (Vector3.Distance(transform.position, agentDestination) > MaxFollowDistance)
        {
            target = null;
            SetRandomDestination();
        }
    }

    private void SetRandomDestination()
    {
        float randomDistance = Random.Range(MinDistance, MaxDistance);
        Vector3 randomPoint = transform.position + Random.onUnitSphere * randomDistance;
        randomPoint.y = transform.position.y;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, randomDistance, NavMesh.AllAreas))
        {
            agentDestination = hit.position;
        }
        debugCount = 0;
    }

    private void HandlePathfindingAndMovement()
    {
        bool hasReachDestination = Vector3.Distance(transform.position, AgentGridDestination(agentDestination)) < 0.01f;
        bool hasReachCenter = Vector3.Distance(transform.position, destination) < 0.01f;

        if (hasReachDestination)
        {
            SetRandomDestination();
        }
        else
        {
            if(hasReachCenter)
                debugCount++;
            if(debugCount > debugCountMax)
                SetRandomDestination();
        }

        if (hasReachCenter)
        {
            Navigate();
        }

        //Debug.Log($"Has Reach Destination: {hasReachDestination}, Has Reach Center: {hasReachCenter}, count: {debugCount}");
    }

    private void Navigate()
    {
        NavMesh.CalculatePath(transform.position, agentDestination, NavMesh.AllAreas, navMeshPath);
        if (navMeshPath.corners.Length > 1)
        {
            destination = AgentGridDestination(navMeshPath.corners[1]);
        }
        agent.SetDestination(destination);
    }

    private Vector3 AgentGridDestination(Vector3 vector3)
    {
        Vector3 cellDestination = grid.GetCellCenterWorld(grid.WorldToCell(vector3));


        return new Vector3(cellDestination.x, vector3.y, cellDestination.z);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        LabirintPlayer player = other.GetComponent<LabirintPlayer>();

        if (player != null)
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
