using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTest : MonoBehaviour
{
    public float increaseFactor = 0.001f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<PlayerCharacter>())
            Debug.Log("Triggered");
    }

    private void Update()
    {
        transform.localScale += new Vector3(increaseFactor, increaseFactor, increaseFactor);
    }
}
