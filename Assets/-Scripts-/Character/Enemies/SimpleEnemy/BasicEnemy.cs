using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : EnemyCharacter
{

    [SerializeField] float stoppingDistance = 1;


    [SerializeField] private ContextSolver moveDirectionSolver;

    //di prova
    [SerializeField] Transform tryTarget;


    private void Update()
    {

        Move(FindDirection(), rb);
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
    }
}
