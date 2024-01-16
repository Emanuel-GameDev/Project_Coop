using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : EnemyCharacter
{

    [SerializeField] float stoppingDistance = 1;



    //di prova
    [SerializeField] Transform tryTarget;

    NavMeshPath path;

    [SerializeField] Transform pivot;
    Vector2 lastNonZeroDirection;
    bool isMoving;
    bool isAttacking = false;

    private void Start()
    {
        path = new NavMeshPath();
    }

    private void Update()
    {
        if (agent.CalculatePath(tryTarget.position, path))
        {
            if (path.corners.Length > 1)
                Move(path.corners[1] - path.corners[0], rb);
            else
                Move(tryTarget.position - transform.position, rb);
        }
        else
            rb.velocity = Vector3.zero;
    }

    public virtual void Move(Vector3 direction, Rigidbody rb)
    {
        if (Vector3.Distance(transform.position,tryTarget.position) < stoppingDistance)
        {
            rb.velocity = Vector3.zero;

            //attacca
            if(!isAttacking)
                StartCoroutine(Attack());

            return;
        }
        
        if (!direction.normalized.Equals(direction))
            direction = direction.normalized;

        rb.velocity = new Vector3(direction.x * MoveSpeed, direction.y, direction.z * MoveSpeed);




        Vector2 direction2D = new Vector2(direction.x, direction.z);

        isMoving = rb.velocity.magnitude > 0.2f;
        

        if (direction2D != Vector2.zero)
            lastNonZeroDirection = direction2D;

        SetSpriteDirection(lastNonZeroDirection);

        animator.SetBool("isMooving", isMoving);
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

    IEnumerator Attack()
    {
        isAttacking = true;
        GetComponentInChildren<SpriteRenderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
        GetComponentInChildren<SpriteRenderer>().material.color = Color.white;
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
