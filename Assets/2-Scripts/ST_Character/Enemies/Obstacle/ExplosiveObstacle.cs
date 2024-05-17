using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveObstacle : ObstacleEnemy
{

    [Header("Variabili escusive Ostacolo esplosivo")]

    [SerializeField, Tooltip("Il raggio dell'esplosione")]
    [Min(1)]
    protected float explosionArea = 1;

    Detector areaExplosionDetector;

    private void Awake()
    {
        active=true;
        
        areaExplosionDetector = GetComponentInChildren<Detector>();

        GetComponentInChildren<CircleCollider2D>().radius = explosionArea;
        
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active)
        {
            if (collision.gameObject.TryGetComponent(out PlayerCharacter player))
            {
                Debug.Log("sono entrato nelle spine");


                foreach (PlayerCharacter playerCharacter in areaExplosionDetector.GetPlayersDetected())
                {
                    
                    StartCoroutine(PushPlayerExplosion(playerCharacter, explosionArea));
                }

                //animazione player che si fa male

                //esplosione visiva
                
                active = false;
            }
        }
    }
}
