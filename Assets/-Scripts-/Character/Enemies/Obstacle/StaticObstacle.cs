using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObstacle : ObstacleEnemy
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerCharacter>(out PlayerCharacter player))
        {
            Debug.Log("sono entrato nelle spine");
            Vector2 forceVector= (collision.transform.position- transform.position).normalized;
            collision.gameObject.GetComponentInChildren<Rigidbody2D>().AddForce(forceVector * pushStrength);

            //animazione player che si fa male
        }
    }
}
