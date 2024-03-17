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
            Vector2 forceVector= (player.transform.position- transform.position).normalized;

            
            player.gameObject.GetComponentInChildren<Rigidbody2D>().AddForce(forceVector * pushStrength,ForceMode2D.Force);

            

            //animazione player che si fa male
        }
    }

    void AddExplosionForce2D(PlayerCharacter player,Vector3 explosionOrigin, float explosionForce, float explosionRadius)
    {
        Vector2 direction = player.transform.position - explosionOrigin;
        float forceFalloff = 1 - (direction.magnitude / explosionRadius);
        player.gameObject.GetComponent<Rigidbody2D>().AddForce(direction.normalized * (forceFalloff <= 0 ? 0 : explosionForce) * forceFalloff);
    }
}
