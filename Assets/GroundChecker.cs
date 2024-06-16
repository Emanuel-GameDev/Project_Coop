using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private LayerMask platformLayerMask; 
    public bool isGrounded;

   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Utility.IsInLayerMask(collision.gameObject, platformLayerMask))
        {
            isGrounded = true;
        }
       
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (Utility.IsInLayerMask(collision.gameObject, platformLayerMask))
        {
            isGrounded = false;
        }
        
    }


}
