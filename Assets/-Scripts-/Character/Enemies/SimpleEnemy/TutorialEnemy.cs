using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialEnemy : BasicEnemy
{
    [HideInInspector] public event Action OnHit;

    public override void TakeDamage(DamageData data)
    {
        base.TakeDamage(data);
        OnHit?.Invoke();
        Debug.Log("Hit");
    }

    
}
