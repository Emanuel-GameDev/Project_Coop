using UnityEngine;

public class LabirintMovment : MonoBehaviour
{
    public float moveSpeed = 10f;
    public Grid grid;
    protected Vector3 moveDir;
    protected Vector3 destination;

    protected virtual void Start()
    {
        destination = transform.position;
    }

    protected virtual void Update()
    {
        Move();
    }

    protected void Move()
    {
        Vector3 direzioneMovimento = moveDir;
        direzioneMovimento.Normalize();

        if (moveDir != Vector3.zero && CheckDirection())
        {
            Vector3 nextCellPosition = grid.GetCellCenterWorld(grid.WorldToCell(transform.position + direzioneMovimento * grid.cellSize.x));
            if (!IsCellOccupied(nextCellPosition))
                destination = nextCellPosition;

            //Debug.Log($"Destination: {destination}, MoveDir: {moveDir}");
        }


        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    }

    protected bool CheckDirection()
    {
        bool hasReachCenter = Vector3.Distance(transform.position, destination) < 0.01f;
        Vector3 directionToDestination = (destination - (Vector3)transform.position).normalized;
        bool isSameAxis = false;
        if ((directionToDestination.x == 0 && moveDir.x == 0) || (directionToDestination.z == 0 && moveDir.z == 0))
            isSameAxis = true;
        //Debug.Log($"MoveDir: {moveDir}");
        //Debug.Log($"has reac Center: {hasReachCenter}, Is Same Axis: {isSameAxis}");
        return hasReachCenter || isSameAxis;
    }

    protected Vector2 SetVector01(Vector2 vector)
    {
        vector.Normalize();
        if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
        {
            vector.y = 0;
            if (vector.x != 0)
                vector.x = vector.x > 0 ? 1 : -1;
        }
        else
        {
            vector.x = 0;
            if (vector.y != 0)
                vector.y = vector.y > 0 ? 1 : -1;
        }

        return vector;

    }

    protected bool IsCellOccupied(Vector3 cellPosition)
    {
        Collider[] colliders = Physics.OverlapBox(cellPosition, grid.cellSize / 2.1f);

        foreach (Collider collider in colliders)
        {
            if (!collider.isTrigger)
            {
                return true;
            }
        }

        return false;
    }

}

