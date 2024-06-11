using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{  
    [SerializeField] Rigidbody2D rb;
    
    public void Drop(float speed)
    {
       rb.velocity = Vector2.down * speed;
    }

    

}
