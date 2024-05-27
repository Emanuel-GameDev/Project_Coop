using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialEnemy : BasicMeleeEnemy
{
    [HideInInspector] public event Action OnHitAction;
    [SerializeField] float invincibilitySeconds = 0.2f;
    [HideInInspector] public bool focus = false;


    [HideInInspector] public TutorialEnemyMovementState moveState;
    [HideInInspector] public TutorialEnemyAttackState actionState;

    bool invincible=false;
    protected override void Awake()
    {
        base.Awake();

        obstacle.enabled = false;
        obstacle.carveOnlyStationary = false;
        obstacle.carving = true;


        idleState = new BasicMeleeEnemyIdleState(this);
        moveState = new TutorialEnemyMovementState(this);
        actionState = new TutorialEnemyAttackState(this);
       // entryState = new BasicEnemyEntryState(this);
    }

    public override void TakeDamage(DamageData data)
    {
        if (!invincible)
        {
            base.TakeDamage(data);
            stateMachine.SetState(stunState);
            OnHitAction?.Invoke();
            if(data.dealer is Projectile)
            {
                Projectile projectile = (Projectile) data.dealer;
                PubSub.Instance.Notify(EMessageType.characterDamaged, projectile);
            }
            
            StartCoroutine(Invincibility());

        }

    }

    public override void SetTarget(Transform newTarget)
    {
        if (!focus)
        {
            base.SetTarget(newTarget);
            stateMachine.SetState(moveState);
        }
    }

    IEnumerator Invincibility()
    {
        invincible = true;
        yield return new WaitForSeconds(invincibilitySeconds);
        invincible = false;
    }
    public override void OnParryNotify(Character whoParried)
    {
        base.OnParryNotify(whoParried);
    }
}
