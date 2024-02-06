using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunCondition : Condition
{
    private Character characterClass;
    private float duration;
    private bool started;
    private float timer;

    public override void AddCondition(Character parent)
    {
        transform.parent = parent.transform;
        base.AddCondition(parent);
        characterClass = GetComponentInParent<Character>();    
        Debug.Log(transform.parent.name + " sono sotto STUN per " + duration + " secondi");
    }

    public override void RemoveCondition(Character parent)
    {
        characterClass.stunned = false;
        Debug.Log(parent.name + " non sono più sotto STUN");
        Destroy(this.gameObject);

    }

    public void SetVariable(CharacterClass player, float duration)
    {
       
        this.duration = duration;

    }

    private void Update()
    {
        if (characterClass != null)
        {
            if (!started)
            {
                started = true;
                timer = duration;
                characterClass.stunned = true;

            }
        }
        
    }

}
