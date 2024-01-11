using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : EnemyCharacter
{

    [SerializeField] float stoppingDistance = 1;


    [SerializeField] private ContextSolver moveDirectionSolver;

    //di prova
    [SerializeField] Transform tryTarget;

    [SerializeField] NavMeshAgent agent1;
    NavMeshPath path;


    private void Start()
    {
        path = new NavMeshPath();
    }
    private void Update()
    {
        if (agent1.CalculatePath(tryTarget.position, path))
        {
            for(int i = 0; i < path.corners.Length;i++)
            {
                Debug.Log(path.corners[i]);
            }
            Move(path.corners[1] - path.corners[0], rb);

        }
        else
            rb.velocity = Vector3.zero;
    }

    public virtual void Move(Vector3 direction, Rigidbody rb)
    {
        

        if (direction.magnitude < stoppingDistance)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        if (!direction.normalized.Equals(direction))
            direction = direction.normalized;

        rb.velocity = new Vector3(direction.x * MoveSpeed, direction.y, direction.z * MoveSpeed);

        




        //forse non serve
        //Vector2 direction2D = new Vector2(direction.x, direction.z);

        //isMoving = rb.velocity.magnitude > 0.2f;

        //if (direction2D != Vector2.zero)
        //    lastNonZeroDirection = direction2D;
        //SetSpriteDirection(lastNonZeroDirection);
    }

    Vector3 FindDirection()
    {
        //trova direzione con context solver

        Vector3 moveVector = tryTarget.position - transform.position;
        return moveVector;
    }

    public override void TargetSelection()
    {
        //base.TargetSelection();
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
        if(path != null)
        {
            if (path.corners.Length > 0)
            {
                Gizmos.DrawLineList(path.corners);
            }

        }
    }
}
