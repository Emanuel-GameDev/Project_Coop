using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LabirintMovment : MonoBehaviour
{
    public float velocitaMovimento = 5f;
    public Grid grid;
    Vector2 moveDir;
    Vector2 destination;

    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 direzioneMovimento = moveDir;
        direzioneMovimento.Normalize();

        if (moveDir != Vector2.zero && CheckDirection())
        {
            destination = grid.GetCellCenterWorld(grid.WorldToCell(transform.position + direzioneMovimento * grid.cellSize.x));
            Debug.Log($"Destination: {destination}");
        }
            

        transform.position = Vector3.MoveTowards(transform.position, destination, velocitaMovimento * Time.deltaTime);
    }

    private bool CheckDirection()
    {
        bool hasReachCenter = Vector3.Distance(transform.position, destination) < 0.01f;
        Vector2 directionToDestination = (destination - (Vector2)transform.position).normalized;
        bool isSameAxis = false;//DaCompletare
        Debug.Log($"MoveDir: {moveDir}");
        Debug.Log($"has reac Center: {hasReachCenter}, Is Same Axis: {isSameAxis}");
        return hasReachCenter || isSameAxis;
    }

    public void MoveInput(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>().normalized;
        if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
        {
            moveDir.y = 0;
            moveDir.x =  moveDir.x > 0 ? 1 : -1;
        }
        else
        {
            moveDir.x = 0;
            moveDir.y = moveDir.y > 0 ? 1 : -1;
        }
    }

    public void StartInput(InputAction.CallbackContext context)
    {
        
    }

    public void SelectInput(InputAction.CallbackContext context)
    {
       
    }

}

