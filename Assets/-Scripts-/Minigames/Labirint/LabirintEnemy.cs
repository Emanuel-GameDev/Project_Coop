using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LabirintEnemy : LabirintMovment
{
    private NavMeshAgent agent;
    private Vector3 agentDestination;
    private NavMeshPath navMeshPath;

    public float maxRandomDestinationDistance = 10f;
    public float minRandomDestinationDistance = 2f;
    public float MaxDistance => maxRandomDestinationDistance * grid.cellSize.x;
    public float MinDistance => minRandomDestinationDistance * grid.cellSize.x;

    protected override void Start()
    {
        base.Start();
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

        agent.updatePosition = false;
        agent.updateRotation = false;
        SetRandomDestination();
    }

    protected override void Update()
    {
        HandlePathfindingAndMovement();
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
    }

    private void HandlePathfindingAndMovement()
    {
        NavMesh.CalculatePath(transform.position, agentDestination, NavMesh.AllAreas, navMeshPath);
        if (navMeshPath.corners.Length > 1)
        {
            moveDir = SetVector01(navMeshPath.corners[1] - transform.position);
            Move();
            Debug.Log($"Corners: {navMeshPath.corners.Length}");
        }
        else
        {
            SetRandomDestination();
        }
    }

}
