using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pickable : MonoBehaviour
{
    Character character;

    public UnityEvent OnEnter;
    public UnityEvent OnExit;

    private void OnTriggerEnter2D(Collider2D other)
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

    private void OnTriggerExit2D(Collider2D other)
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
