using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialEnemy : BasicMeleeEnemy
{
    [HideInInspector] public event Action OnHit;
    [SerializeField] float invincibilitySeconds = 0.1f;
    [HideInInspector] public bool focus = false;
    bool invincible=false;
    protected override void Awake()
    {
        base.Awake();

        obstacle.enabled = false;
        obstacle.carveOnlyStationary = false;
        obstacle.carving = true;


        idleState = new BasicEnemyIdleState(this);
        moveState = new TutorialEnemyMovementState(this);
        actionState = new TutorialEnemyAttackState(this);
        stunState = new BasicEnemyStunState(this);
        parriedState = new BasicEnemyParriedState(this);
        deathState = new BasicEnemyDeathState(this);

    }

    public override void TakeDamage(DamageData data)
    {
        if (!invincible)
        {
        base.TakeDamage(data);
        OnHit?.Invoke();
            StartCoroutine(Invincibility());
        }

    }

    public override void SetTarget(Transform newTarget)
    {
        if(!focus)
            base.SetTarget(newTarget);
    }

    IEnumerator Invincibility()
    {
        invincible = true;
        yield return new WaitForSeconds(invincibilitySeconds);
        invincible = false;
    }
}
