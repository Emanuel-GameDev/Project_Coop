using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pickable : MonoBehaviour
{
    Character character;

    public UnityEvent OnEnter;
    public UnityEvent OnExit;

    private void OnTriggerEnter(Collider other)
    {
        Character target= other.GetComponent<Character>();

        if (target != null)
        {
            if (target.gameObject == character.gameObject)
            {
                OnEnter.Invoke();
            }
        }
       
        
    }

    private void OnTriggerExit(Collider other)
    {
        Character target= other.GetComponent<Character>();

        if(target != null)
        {
            if (other.gameObject == character.gameObject)
            {
                OnExit.Invoke();
            }
        }
       
    }

    public void SetCharacter(Character character)
    {
        this.character = character;
    }
}
