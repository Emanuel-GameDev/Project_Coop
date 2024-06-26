using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DPS : PlayerCharacter, IPerfectTimeReceiver
{
    [Header("Attack")]
    [SerializeField, Tooltip("Tempo tra una combo e l'altra.")]
    float timeBetweenCombo = 1f;

    [Header("Dodge")]
    [SerializeField, Tooltip("Distanza di schivata.")]
    float dodgeDistance = 10f;
    [SerializeField, Tooltip("Durata della schivata.")]
    float dodgeDuration = 0.25f;
    [SerializeField, Tooltip("Tempo di attesa prima di poter usare di nuovo la schivata.")]
    float dodgeCooldown = 5f;
    [SerializeField, Tooltip("Durata del danno extra dopo una schivata perfetta.")]
    float perfectDodgeExtraDamageDuration = 5f;
    [SerializeField, Tooltip("Danno extra in % dopo una schivata perfetta."), Range(0, 1)]
    float perfectDodgeExtraDamage = 0.15f;
    [SerializeField, Tooltip("Durata del tempo utile per poter fare la schivata perfetta")]
    float perfectDodgeDuration = 0.5f;

    [Header("Unique Ability")]
    [SerializeField, Tooltip("Durata dell'invulnerabilità.")]
    float invulnerabilityDuration = 5f;
    [SerializeField, Tooltip("Aumento di velocità in % durante l'invulnerabilità."), Range(0, 1)]
    float invulnerabilitySpeedUp = 0.25f;

    [Header("Extra Ability")]
    [SerializeField, Tooltip("Distanza minima dell'attacco con il dash.")]
    float minDashAttackDistance = 5f;
    [SerializeField, Tooltip("Distanza massima dell'attacco con il dash.")]
    float maxDashAttackDistance = 15f;
    [SerializeField, Tooltip("Durata dell'attaccon don il dash.")]
    float dashAttackDuration = 5f;
    [SerializeField, Tooltip("Massimo tempo di caricamento dell'attacco con il dash.")]
    float dashAttackMaxLoadUpTime = 5f;
    [SerializeField, Tooltip("Tempo di ricarica dell'attacco con il dash.")]
    float dashAttackCooldown = 5f;
    [SerializeField, Tooltip("Moltiplicatore al danno base dell'attacco con il dash durante il movovimento.")]
    float dashAttackRushDamageMultiplier = 0.5f;
    [SerializeField, Tooltip("Moltiplicatore al danno base dell'attacco con il dash nell'attacco.")]
    float dashAttackSlashDamageMultiplier = 1.5f;

    [Header("Boss Power Up")]
    [SerializeField, Tooltip("Percentuale dei danni da fare al boss per sbloccare il potenziamento."), Range(0, 1)]
    float bossPowerUpPercentageDamageToUnlock = 0.4f;
    [SerializeField, Tooltip("Danno extra in % per colpo conferito dal potenziamento del boss."), Range(0, 1)]
    float bossPowerUpExtraDamagePerHit = 0.02f;
    [SerializeField, Tooltip("Limite massimo in % del danno extra conferito dal potenziamento del boss."), Range(0, 1)]
    float bossPowerUpExtraDamageCap = 0.16f;
    [SerializeField, Tooltip("Durata del danno extra conferito dal potenziamento del boss dopo l'ultimo colpo inferto.")]
    float bossPowerUpExtraDamageDuration = 2.5f;

    [Header("Other")]
    [SerializeField, Tooltip("I Layer da guardare quando ha sbloccato il power up per deflettere i proiettili")]
    LayerMask projectileLayer;

    [Header("VFX")]
    [SerializeField]
    TrailRenderer trailDodgeVFX;
    [SerializeField]
    GameObject DodgeTrailVFX;
    [SerializeField]
    GameObject invicibilityVFX;
    [SerializeField, Tooltip("Gli eventi da chiamare in caso di schivata perfetta")]
    UnityEvent onPerfectDodgeExecuted = new();

    [Header("Temporany Effects")]
    [SerializeField]
    GameObject invicibilityBaloon;
    [SerializeField]
    float balonDuration = 0.5f;

    private float ExtraSpeed => ImmortalitySpeedUpUnlocked && isInvulnerable ? invulnerabilitySpeedUp : 0;

    private float ExtraDamage()
    {
        float perfectDodgeDamage = PerfectDodgeExtraDamageUnlocked && Time.time < lastPerfectDodgeTime + perfectDodgeExtraDamageDuration ? perfectDodgeExtraDamage : 0;
        float bossPowerUpDamage = bossfightPowerUpUnlocked ? MathF.Min(bossPowerUpExtraDamagePerHit * (consecutiveHitsCount > 1 ? consecutiveHitsCount - 1 : 0), bossPowerUpExtraDamageCap) : 0;

        Debug.Log($"ExtraDamage Multi TOT: {perfectDodgeDamage + bossPowerUpDamage + 1}, dodge: {perfectDodgeDamage}, boss: {bossPowerUpDamage}");

        return perfectDodgeDamage + bossPowerUpDamage + 1;
    }

    private float lastAttackTime;
    private float lastDodgeTime;
    private float lastPerfectDodgeTime;
    private float lastDashAttackTime;
    private float lastLandedHitTime;
    private int perfectDodgeCounter = 0;
    private float dashAttackStartTime;
    private float dashAttackDamageMultiplier;
    private float currentBossfightTotalDamageDone = 0;
    private float currentBossDamageToReach = float.MaxValue;
    private Vector3 startPosition;

    private int consecutiveHitsCount;

    private bool mustContinueCombo = false;
    private bool alreadyCalled = false;
    private bool uniqueAbilityAvaiable = true;
    private bool uniqueAbilityIsInAnimation = false;

    private AttackComboState currentAttackComboState;
    private AttackComboState NextAttackComboState
    {
        get
        {
            return currentAttackComboState switch
            {
                AttackComboState.NotAttaking => AttackComboState.Attack1,
                AttackComboState.Attack1 => AttackComboState.Attack2,
                AttackComboState.Attack2 => AttackComboState.Attack3,
                AttackComboState.Attack3 => UnlimitedComboUnlocked ? AttackComboState.Attack1 : AttackComboState.NotAttaking,
                _ => AttackComboState.NotAttaking
            };
        }
    }

    private bool DashAttackUnlocked => upgradeStatus[AbilityUpgrade.Ability1];
    private bool UnlimitedComboUnlocked => upgradeStatus[AbilityUpgrade.Ability2];
    private bool ProjectileDeflectionUnlocked => upgradeStatus[AbilityUpgrade.Ability3];
    private bool ImmortalitySpeedUpUnlocked => upgradeStatus[AbilityUpgrade.Ability4];
    private bool PerfectDodgeExtraDamageUnlocked => upgradeStatus[AbilityUpgrade.Ability5];

    private bool isInvulnerable;
    private bool isRessing = false;
    private bool isDodging;
    private bool isDashingAttack;
    private bool isDashingAttackStarted;
    private bool perfectTimingEnabled;

    private bool CanMove => !isDodging && !IsAttacking && !isDashingAttack && !uniqueAbilityIsInAnimation && !isRessing;
    private bool CanUseUniqueAbility => CanMove && uniqueAbilityAvaiable;

    private bool IsAttacking
    {
        get => _isAttacking;
        set
        {
            _isAttacking = value;
            animator.SetBool("isAttacking", _isAttacking);
        }
    }
    private bool _isAttacking;

    private ChargeVisualHandler chargeHandler;
    private PerfectTimingHandler perfectTimingHandler;
    private ParticleSystem.EmissionModule emissionModule;

    #region Animation Variable
    private static string ATTACK = "Attack";
    private static string DODGESTART = "DodgeStart";
    private static string DODGEEND = "DodgeEnd";
    private static string HIT = "Hit";
    private static string STARTDASHATTACK = "StartDashAttack";
    private static string MOVEDASHATTACK = "MoveDashAttack";
    private static string ENDDASHATTACK = "EndDashAttack";
    private static string DEATH = "Death";
    private static string ISMOVING = "IsMoving";
    private static string RESSING = "Ressing";
    private static string RESS = "Ress";
    private static string BERSERKER = "Berserker";
    #endregion

    public override float AttackSpeed => base.AttackSpeed + ExtraSpeed;
    public override float MoveSpeed => base.MoveSpeed + ExtraSpeed;
    public override float Damage => base.Damage * ExtraDamage();

    public override void Inizialize()
    {
        base.Inizialize();
        ResetVariables();
        chargeHandler = GetComponentInChildren<ChargeVisualHandler>();
        chargeHandler.Inizialize(minDashAttackDistance, maxDashAttackDistance, dashAttackMaxLoadUpTime, this);
        perfectTimingHandler = GetComponentInChildren<PerfectTimingHandler>();
        character = ePlayerCharacter.Brutus;
        emissionModule = _walkDustParticles.emission;
    }

    private void ResetVariables()
    {
        lastDodgeTime = -dodgeCooldown;
        lastAttackTime = -timeBetweenCombo;
        uniqueAbilityAvaiable = true;
        lastDashAttackTime = -dashAttackCooldown;
        consecutiveHitsCount = 0;
        isInvulnerable = false;
        isRessing = false;
        isDodging = false;
        IsAttacking = false;
        isDashingAttack = false;
        isDashingAttackStarted = false;
        perfectTimingEnabled = false;
        invicibilityVFX.SetActive(false);
        invicibilityBaloon.SetActive(false);
        trailDodgeVFX.gameObject.SetActive(false);
        DodgeTrailVFX.SetActive(false);
    }


    //Attack: combo rapida di tre attacchi melee, ravvicinati. 
    #region Attack
    public override void AttackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            alreadyCalled = false;

            if (IsAttacking)
            {
                ContinueCombo();
            }
            else if (CanStartCombo())
            {
                ResetAttack();
                StartCombo();
            }

            Utility.DebugTrace($"Attacking: {IsAttacking}, AbiliyUpgrade2: {UnlimitedComboUnlocked}, CooldownEnded: {Time.time > lastAttackTime + timeBetweenCombo} \n MustContinueCombo: {mustContinueCombo},  CurrentComboState: {currentAttackComboState}, NextComboState: {NextAttackComboState}");
        }
    }

    #region OldComboSystem

    private void StartCombo()
    {
        currentAttackComboState = AttackComboState.Attack1;
        DoMeleeAttack();
        rb.velocity = Vector3.zero;
    }

    private void ContinueCombo()
    {
        mustContinueCombo = true;
    }
    private void DoMeleeAttack()
    {
        IsAttacking = true;
        string triggerName = currentAttackComboState.ToString();
        animator.SetTrigger(triggerName);

        //PlayAttackSound();
    }

    public void OnAttackAnimationEnd()
    {
        if (!alreadyCalled)
        {
            AdjustLastAttackTime();

            if (mustContinueCombo)
            {
                mustContinueCombo = false;
                currentAttackComboState = NextAttackComboState;

                if (currentAttackComboState == AttackComboState.NotAttaking)
                    ResetAttack();
                else
                    DoMeleeAttack();
            }
            else
            {
                ResetAttack();
            }

            Utility.DebugTrace($"EndAttakMustContinue: {mustContinueCombo}, Current State: {currentAttackComboState}");
            alreadyCalled = true;
        }
    }

    public void OnAttackAnimationStart()
    {
        alreadyCalled = false;
    }


    private void AdjustLastAttackTime()
    {
        lastAttackTime = Time.time;
    }

    private bool CanStartCombo() => (UnlimitedComboUnlocked || (CanMove && Time.time > lastAttackTime + timeBetweenCombo && currentAttackComboState == AttackComboState.NotAttaking));

    private void ResetAttack()
    {
        IsAttacking = false;
        currentAttackComboState = AttackComboState.NotAttaking;
        mustContinueCombo = false;
        alreadyCalled = false;
    }
    #endregion

    #endregion

    //Defense: fa una schivata, si sposta di tot distanza verso la direzione decisa dal giocatore con uno scatto
    #region Defense
    public override void DefenseInput(InputAction.CallbackContext context)
    {
        //CONTROLLARE
        if (context.performed)
        {
            Utility.DebugTrace($"Executed: {Time.time > lastDodgeTime + dodgeCooldown} ");
            if (Time.time > lastDodgeTime + dodgeCooldown && !isDodging && !isDashingAttack)
            {
                ResetAttack();
                lastDodgeTime = Time.time + dodgeDuration;

                if (perfectTimingEnabled)
                {
                    perfectDodgeCounter++;
                    lastPerfectDodgeTime = Time.time;
                    PubSub.Instance.Notify(EMessageType.perfectDodgeExecuted, this);
                    PerfectTimeEnded();
                    onPerfectDodgeExecuted?.Invoke();
                    Utility.DebugTrace($"PerfectDodge: {true}, Count: {perfectDodgeCounter}");
                    PlayPerfectDodgeSound();
                }

                StartCoroutine(Dodge(lastNonZeroDirection, rb));
            }
        }
    }

    protected IEnumerator Dodge(Vector2 dodgeDirection, Rigidbody2D rb)
    {
        isDodging = true;
        animator.SetTrigger(DODGESTART);
        PubSub.Instance.Notify(EMessageType.dodgeExecuted, this);
        onDefenceAbility?.Invoke();
        //trailDodgeVFX.gameObject.SetActive(true);

        DodgeTrailVFX.transform.rotation = Quaternion.FromToRotation(Vector3.down, dodgeDirection);
        DodgeTrailVFX.SetActive(true);

        yield return StartCoroutine(Move(dodgeDirection, rb, dodgeDuration, dodgeDistance * powerUpData.DodgeDistanceIncrease));

        isDodging = false;
        animator.SetTrigger(DODGEEND);
        trailDodgeVFX.gameObject.SetActive(false);
        DodgeTrailVFX.SetActive(false);
    }

    private IEnumerator Move(Vector2 direction, Rigidbody2D rb, float duration, float distance)
    {
        Vector2 startPosition = transform.position;
        rb.velocity = Vector2.zero;

        Vector2 destination = startPosition + direction.normalized * distance;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            rb.MovePosition(Vector2.Lerp(startPosition, destination, elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;
    }

    #endregion

    //UniqueAbility: immortalità per tot secondi
    #region UniqueAbility
    public override void UniqueAbilityInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Utility.DebugTrace($"Executed: {!isInvulnerable && CanUseUniqueAbility}");
            if (!isInvulnerable && CanUseUniqueAbility)
            {
                uniqueAbilityAvaiable = false;
                base.UniqueAbilityInput(context);
                StartCoroutine(UseUniqueAbilityCoroutine());
                StartCoroutine(BaloonDuration());
                animator.SetTrigger(BERSERKER);
                uniqueAbilityIsInAnimation = true;
                rb.velocity = Vector3.zero;
            }
        }
    }

    public void UniqueAnimationEndend() => uniqueAbilityIsInAnimation = false;

    //temp
    private IEnumerator BaloonDuration()
    {
        invicibilityBaloon.SetActive(true);
        yield return new WaitForSeconds(balonDuration);
        invicibilityBaloon.SetActive(false);
    }


    private IEnumerator UseUniqueAbilityCoroutine()
    {
        isInvulnerable = true;
        PubSub.Instance.Notify(EMessageType.uniqueAbilityActivated, this);
        invicibilityVFX.SetActive(true);

        yield return new WaitForSeconds(invulnerabilityDuration);

        isInvulnerable = false;
        PubSub.Instance.Notify(EMessageType.uniqueAbilityExpired, this);
        invicibilityVFX.SetActive(false);

        yield return new WaitForSeconds(UniqueAbilityCooldown - invulnerabilityDuration);
        uniqueAbilityUses++;
        uniqueAbilityAvaiable = true;
    }
    #endregion

    //ExtraAbility: è l'ability upgrade 1
    #region ExtraAbility
    public override void ExtraAbilityInput(InputAction.CallbackContext context)
    {

        if (context.started && DashAttackUnlocked && CanMove && (Time.time - lastDashAttackTime > dashAttackCooldown))
        {
            Utility.DebugTrace("Performed");
            isDashingAttack = true;
            dashAttackStartTime = Time.time;
            rb.velocity = Vector2.zero;
            animator.SetTrigger(STARTDASHATTACK);
            chargeHandler.StartCharging(dashAttackStartTime);
        }

        if (context.canceled && DashAttackUnlocked && isDashingAttack && !isDashingAttackStarted)
        {
            isDashingAttackStarted = true;
            Utility.DebugTrace("Canceled");
            chargeHandler.StopCharging();
            StartCoroutine(DashAttack(lastNonZeroDirection, rb));
        }

    }

    protected IEnumerator DashAttack(Vector2 attackDirection, Rigidbody2D rb)
    {
        animator.SetTrigger(MOVEDASHATTACK);
        dashAttackDamageMultiplier = dashAttackRushDamageMultiplier;
        float pressDuration = Time.time - dashAttackStartTime;
        float dashAttackDistance = Mathf.Lerp(minDashAttackDistance, maxDashAttackDistance, pressDuration / dashAttackMaxLoadUpTime);

        yield return StartCoroutine(Move(attackDirection, rb, dashAttackDuration, dashAttackDistance));

        dashAttackDamageMultiplier = dashAttackSlashDamageMultiplier;
        animator.SetTrigger(ENDDASHATTACK);
    }

    public void DashAttackTeleport()
    {
        rb.MovePosition(startPosition);
        Debug.Log($"Teleport at: {startPosition}, current position: {transform.position}");
    }

    public void EndDashAttack()
    {
        isDashingAttack = false;
        isDashingAttackStarted = false;
        lastDashAttackTime = Time.time;
    }

    #endregion

    public override void Move(Vector2 direction)
    {
        if (CanMove)
        {
            base.Move(direction);
        }
        else if (isDashingAttack)
        {
            if (direction != Vector2.zero)
                lastNonZeroDirection = direction;
            SetSpriteDirection(lastNonZeroDirection);
        }
        animator.SetBool(ISMOVING, isMoving);
        emissionModule.enabled = isMoving;
    }


    public override void TakeDamage(DamageData data)
    {
        PubSub.Instance.Notify(EMessageType.characterHitted, this);
        if (!isInvulnerable && !isDodging)
        {
            base.TakeDamage(data);
            //if (!isDashingAttack)
            //    animator.SetTrigger(HIT);

            //if(IsAttacking)
            //    ResetAttack();
        }

        if (perfectTimingEnabled)
            PerfectTimeEnded();

    }


    public override void UnlockUpgrade(AbilityUpgrade abilityUpgrade)
    {
        base.UnlockUpgrade(abilityUpgrade);
        if (abilityUpgrade == AbilityUpgrade.Ability3)
            AddDeflect();
        Utility.DebugTrace("Unlock " + abilityUpgrade.ToString());
    }



    public override void LockUpgrade(AbilityUpgrade abilityUpgrade)
    {
        base.LockUpgrade(abilityUpgrade);
        if (abilityUpgrade == AbilityUpgrade.Ability3 && ProjectileDeflectionUnlocked)
            RemoveDeflect();
    }

    public void DeflectProjectile(Collider2D collider)
    {
        if (Utility.IsInLayerMask(collider.gameObject.layer, projectileLayer))
        {
            if (TryGetComponent(out Projectile projectile))
            {
                projectile.ReflectProjectile(this.gameObject, 1);
            }
        }

    }
    private void AddDeflect()
    {
        damager.AssignFunctionToOnTrigger(DeflectProjectile);
    }

    private void RemoveDeflect()
    {
        damager.RemoveFunctionFromOnTrigger(DeflectProjectile);
    }

    public override void ResetAllAnimatorAndTriggers()
    {
        base.ResetAllAnimatorAndTriggers();
        ResetVariables();
    }


    #region Damage

    public override DamageData GetDamageData()
    {
        BossDamageCheck();

        float damage = isDashingAttack ? base.Damage * dashAttackDamageMultiplier : Damage;

        TotalDamageUpdate(damage);

        Utility.DebugTrace($"Damage Done: {damage}");

        return new DamageData(damage, this);

    }

    public override void Die()
    {
        base.Die();
        animator.SetTrigger(DEATH);
    }

    public override void Ress()
    {
        base.Ress();
        animator.SetTrigger(RESS);
    }

    private void BossDamageCheck()
    {
        if (isInBossfight && bossfightPowerUpUnlocked)
        {
            if (Time.time > lastLandedHitTime + bossPowerUpExtraDamageDuration)
                consecutiveHitsCount = 0;

            consecutiveHitsCount++;
            lastLandedHitTime = Time.time;
        }
    }

    private void TotalDamageUpdate(float damage)
    {
        if (isInBossfight && bossFightHandler != null)
        {
            currentBossfightTotalDamageDone += damage;
            if (currentBossfightTotalDamageDone > currentBossDamageToReach)
                bossfightPowerUpUnlocked = true;
        }
    }

    public override void SetIsInBossfight(bool value, BossFightHandler bossFightHandler)
    {
        base.SetIsInBossfight(value, bossFightHandler);
        currentBossfightTotalDamageDone = 0;
        if (value)
        {
            if (bossFightHandler != null && bossFightHandler.BossCharacter != null)
                currentBossDamageToReach = bossFightHandler.BossCharacter.MaxHp * bossPowerUpPercentageDamageToUnlock;
            else
            {
                currentBossDamageToReach = MaxHp;
                Debug.LogWarning("BOSS NOT FOUND!");
            }
        }
        else
            currentBossDamageToReach = float.MaxValue;
    }

    public void SetPerfectTimingHandler(PerfectTimingHandler handler) => perfectTimingHandler = handler;


    public void PerfectTimeStarted(IDamager damager)
    {
        if (!isDodging)
        {
            perfectTimingHandler.ActivateAlert();
            perfectTimingEnabled = true;
        }

        Debug.Log(!isDodging);
        StartCoroutine(DisablePerfectTimeAfter(perfectDodgeDuration));
    }

    public void PerfectTimeEnded()
    {
        perfectTimingHandler.DeactivateAlert();
        perfectTimingEnabled = false;
        Utility.DebugTrace("Perfect Time Ended");
    }


    protected IEnumerator DisablePerfectTimeAfter(float time)
    {
        yield return new WaitForSeconds(time);
        if (perfectTimingEnabled)
            PerfectTimeEnded();
    }


    #endregion

    #region Audio
    public void PlayAttackSound()
    {
        if (soundsDatabase != null)
        {
            int value = (int)currentAttackComboState - 1;
            if (value >= 0 && soundsDatabase.attackSounds.Count > value)
            {
                AudioManager.Instance.PlayAudioClip(soundsDatabase.attackSounds[value], transform);
            }
        }
    }

    private void PlayPerfectDodgeSound()
    {
        if (soundsDatabase.specialEffectsSounds.Count > 0)
        {
            if (soundsDatabase.specialEffectsSounds.Count >= perfectDodgeCounter)
            {
                AudioManager.Instance.PlayAudioClip(soundsDatabase.specialEffectsSounds[perfectDodgeCounter - 1]);
            }
            else
            {
                AudioManager.Instance.PlayAudioClip(soundsDatabase.specialEffectsSounds[soundsDatabase.specialEffectsSounds.Count - 1]);
            }
        }
    }


    #endregion

    protected override void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (canInteract)
            {
                InteractWith(activeInteractable);
                if (activeInteractable is RessInteractable)
                {
                    isRessing = true;
                    animator.SetBool(RESSING, isRessing);
                }

            }
        }
        if (context.canceled)
        {
            if (canInteract)
            {
                AbortInteraction(activeInteractable);
                if (activeInteractable is RessInteractable)
                {
                    isRessing = false;
                    animator.SetBool(RESSING, isRessing);
                }
            }
        }
    }
    public override void DisableInteraction(IInteractable interactable)
    {
        base.DisableInteraction(interactable);
        if (interactable is RessInteractable)
        {
            isRessing = false;
            animator.SetBool(RESSING, isRessing);
        }
    }
    //Potenziamento boss fight: gli attacchi consecutivi aumentano il danno del personaggio a ogni colpo andato a segno.
    //Dopo tot tempo (es: 1.5 secondi) senza colpire, il danno torna al valore standard.


    //Ottenimento potenziamento Boss fight: sbloccabile automaticamente dopo tot danni fatti al boss


    //Ability Upgrade:
    //1: Sblocca uno scatto in avanti + attacco
    //2: Annulla il tempo di ricarica tra le combo di attacchi
    //3: Il personaggio può respingere certi tipi di colpi(es: proiettili) con il suo attacco
    //4: quando il personaggio usa l’abilità unica(i secondi di immortalità) i suoi movimenti diventano più rapidi(attacchi, schivate e spostamenti)
    //5: Effettuare una schivata perfetta aumenta i danni per tot tempo(cumulabile con il bonus ai danni del potenziamento).
}

public enum AttackComboState
{
    NotAttaking,
    Attack1,
    Attack2,
    Attack3
}
