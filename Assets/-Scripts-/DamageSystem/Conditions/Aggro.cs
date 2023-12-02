using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aggro : Condition
{
    private CharacterClass target;
    private float duration;

    public Aggro(CharacterClass target, float duration)
    {
        this.target = target;
        this.duration = duration;
    }

    public override void AddCondition(CharacterClass parent)
    {
        
    }

    public override void RemoveCondition(CharacterClass parent)
    {
        
    }

    private void Update()
    {
        if (target != null)
        {
            Debug.Log($" {this.transform.gameObject.name } aggro contro {target.name}");
        }
    }

}
