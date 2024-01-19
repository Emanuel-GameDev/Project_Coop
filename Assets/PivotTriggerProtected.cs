using UnityEngine;

public class PivotTriggerProtected : MonoBehaviour
{
    private float valueToRotate;
    internal void Rotate(Vector2 lastNonZeroDirection)
    {
            
            Vector3 oppositeDirection = new Vector3(-lastNonZeroDirection.x, 0, -lastNonZeroDirection.y);

           
            Quaternion targetRotation = Quaternion.LookRotation(oppositeDirection, Vector3.up);
            targetRotation *= Quaternion.Euler(0, -90, 0);

           
            transform.rotation = targetRotation;       
    }
}
