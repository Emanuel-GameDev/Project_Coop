using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealMine : MonoBehaviour
{
    [SerializeField] float heal = 10;

    [SerializeField] float activationTime = 1;

    float timer = 0;

    private List<PlayerCharacter> characterInArea;

    private void Awake()
    {
        characterInArea = new List<PlayerCharacter>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerCharacter>() != null)
        {
            characterInArea.Add(other.GetComponent<PlayerCharacter>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        characterInArea.Remove(other.GetComponent<PlayerCharacter>());
    }

    private void Update()
    {
        if (timer >= activationTime)
        {
            if (characterInArea.Count > 1)
            {
                characterInArea[0].TakeDamage(-heal, null);
                Destroy(gameObject);
            }
        }
        else
            timer += Time.deltaTime;
        
    }
}
