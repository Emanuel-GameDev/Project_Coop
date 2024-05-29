using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LodonPlatform : MonoBehaviour
{
    public void Shake()
    {
        Debug.Log("Shake");
    }

    public void BreakFromUnder()
    {
        Debug.Log("BreakFromUnder");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<LodonBoss>() != null)
        {
            Shake();
        } 
    }
}
