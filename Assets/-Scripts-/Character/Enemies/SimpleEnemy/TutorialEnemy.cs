using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialEnemy : BasicMeleeEnemy
{
    [HideInInspector] public event Action OnHit;

    [HideInInspector] public bool focus = false;
 
    public override void TakeDamage(DamageData data)
    {
        base.TakeDamage(data);
        OnHit?.Invoke();
    }

    public override void SetTarget(Transform newTarget)
    {
        if(!focus)
            base.SetTarget(newTarget);
    }

}
