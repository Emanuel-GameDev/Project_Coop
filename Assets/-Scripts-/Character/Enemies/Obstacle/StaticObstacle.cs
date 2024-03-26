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
        if (collision.gameObject.TryGetComponent(out PlayerCharacter player))
        {
            Debug.Log("sono entrato nelle spine");
            

            StartCoroutine(PushPlayer(player));

            //animazione player che si fa male
        }
    }

    
    
}
