using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.GetComponent<Trash>() != null)
        {
            //Spawn particelle/effetti
            Destroy(collision.gameObject);
        }
    }
}
