using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermittenceObstacle : ObstacleEnemy
{
    [Header("Variabili esclusive Ostacolo intermittente")]

    [SerializeField, Tooltip("Intervallo di tempo di idle")]
    [Min(0)]
    protected float idleTime = 5;
    [SerializeField, Tooltip("Intervallo di tempo di action")]
    [Min(0)]
    protected float actionTime = 5;

    [SerializeField] Detector disableDetector;

  

    private void Start()
    {
        active = false;
        StartCoroutine(WaitingForAction());
    }

    private IEnumerator WaitingForAction()
    {
        yield return new WaitForSeconds(idleTime);

        StartCoroutine(WaitingForSleep());

        active = true;

        //momentaneo
        GetComponentInChildren<SpriteRenderer>().color = Color.red;

        foreach(PlayerCharacter character in disableDetector.GetPlayersDetected())
        {
            StartCoroutine(character.PushCharacter(transform.position, pushStrength, 1));
            character.GetComponent<IDamageable>().TakeDamage(GetDamageData());
        }
    }

    private IEnumerator WaitingForSleep()
    {
        yield return new WaitForSeconds(actionTime);

        StartCoroutine(WaitingForAction());

        active = false;

        //momentaneo
        GetComponentInChildren<SpriteRenderer>().color = Color.gray;

        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active)
        {
            if (collision.gameObject.TryGetComponent(out PlayerCharacter player))
            {
                Debug.Log("sono entrato nelle spine");


                StartCoroutine(player.PushCharacter(transform.position, pushStrength, 1));
                player.GetComponent<IDamageable>().TakeDamage(GetDamageData());

                //animazione player che si fa male
            }
        }
       
    }

    

    
}
