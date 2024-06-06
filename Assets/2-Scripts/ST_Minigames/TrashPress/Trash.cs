using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{  
    Rigidbody2D rb;
    internal void Drop(float speed)
    {
       rb.velocity = Vector2.down * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<TrashPressPlayer>() != null)
        {
            Debug.Log("PLAYER COLPITO");
        }
        if(collision.GetComponent<Floor>() != null)
        {
            //Spawn particelle/effetti
            Destroy(this.gameObject);
        }
    }

}
