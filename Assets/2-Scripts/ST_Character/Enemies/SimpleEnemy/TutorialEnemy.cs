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

    [HideInInspector] public new TutorialEnemyIdleState idleState;
    [HideInInspector] public new TutorialEnemyMovementState moveState;
    [HideInInspector] public new TutorialEnemyAttackState actionState;

    bool invincible=false;

    AudioSource idleAudioSource;
    AudioSource walkAudioSource;

    protected override void Awake()
    {
        base.Awake();

        focus = false;
        idleState = new TutorialEnemyIdleState(this);
        moveState = new TutorialEnemyMovementState(this);
        actionState = new TutorialEnemyAttackState(this);
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

    public override IEnumerator Attack()
    {
        StopCoroutine(CalculateChasePathAndSteering());
        isRunning = false;

        isActioning = true;

        //if (panicAttack)
        //{
        //    panicAttack = false;
        //}

        readyToAttack = false;

        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(attackDelay);
        isActioning = false;
        readyToAttack = true;

    }

    public override void SetIdleState()
    {
        base.SetIdleState();
        stateMachine.SetState(idleState);
    }

    public override void SetTarget(Transform newTarget)
    {
        if (!focus)
        {
            base.SetTarget(newTarget);
            //stateMachine.SetState(moveState);
        }
    }
    public override void SetSpriteDirection(Vector2 direction)
    {
        if (direction.y != 0)
            animator.SetFloat("Y", direction.y);

       

        Vector3 scale = pivot.gameObject.transform.localScale;

        if ((direction.x > 0.5 && scale.x > 0) || (direction.x < -0.5 && scale.x < 0))
            scale.x *= -1;

        pivot.gameObject.transform.localScale = scale;
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
    
    public  void PlayIdleSound()
    {
        if (soundsDatabase != null)
        {
            idleAudioSource = AudioManager.Instance.PlayLoopAudioClip(soundsDatabase.specialEffectsSounds[0], transform, soundsDatabase.specialEffectsSoundsVolume);
        }
    }

    public  void StopIdleSound()
    {
        if (soundsDatabase != null)
        {
            idleAudioSource.Stop();
            StartCoroutine(AudioManager.Instance.ReturnAudioSourceToPool(idleAudioSource, 0.01f));
        }
    }

    public override void PlayWalkSound()
    {
        if (soundsDatabase != null)
        {
            walkAudioSource = AudioManager.Instance.PlayLoopAudioClip(soundsDatabase.walkSounds[0], transform,soundsDatabase.walkSoundsVolume);
        }
    }

    public void StopWalkSound()
    {
        if (soundsDatabase != null)
        {
            walkAudioSource.Stop();
            StartCoroutine(AudioManager.Instance.ReturnAudioSourceToPool(walkAudioSource, 0.01f));
        }
    }
}
