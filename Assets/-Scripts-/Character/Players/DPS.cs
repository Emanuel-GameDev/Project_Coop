using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DPS : PlayerCharacter
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
    float perfectDodgeDurarion = 0.5f;
    [Header("Unique Ability")]
    [SerializeField, Tooltip("Durata dell'invulnerabilit�.")]
    float invulnerabilityDuration = 5f;
    [SerializeField, Tooltip("Aumento di velocit� in % durante l'invulnerabilit�."), Range(0, 1)]
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
    [SerializeField, Tooltip("Totale dei danni da fare al boss per sbloccare il potenziamento.")]
    float bossPowerUpTotalDamageToUnlock = 1000f;
    [SerializeField, Tooltip("Danno extra in % per colpo conferito dal potenziamento del boss."), Range(0, 1)]
    float bossPowerUpExtraDamagePerHit = 0.02f;
    [SerializeField, Tooltip("Limite massimo in % del danno extra conferito dal potenziamento del boss."), Range(0, 1)]
    float bossPowerUpExtraDamageCap = 0.16f;
    [SerializeField, Tooltip("Durata del danno extra conferito dal potenziamento del boss dopo l'ultimo colpo inferto.")]
    float bossPowerUpExtraDamageDuration = 2.5f;
    [Header("Other")]
    [SerializeField, Tooltip("I Layer da guardare quando ha sbloccato il power up per deflettere i proiettili")]
    LayerMask projectileLayer;


    private float ExtraSpeed => immortalitySpeedUpUnlocked && isInvulnerable ? invulnerabilitySpeedUp : 0;

    private float ExtraDamage()
    {
        float perfectDodgeDamage = perfectDodgeExtraDamageUnlocked && Time.time < lastPerfectDodgeTime + perfectDodgeExtraDamageDuration ? perfectDodgeExtraDamage : 0;
        float bossPowerUpDamage = bossfightPowerUpUnlocked ? MathF.Min(bossPowerUpExtraDamagePerHit * (consecutiveHitsCount > 1 ? consecutiveHitsCount - 1 : 0), bossPowerUpExtraDamageCap) : 0;

        Debug.Log($"ExtraDamage Multi TOT: {perfectDodgeDamage + bossPowerUpDamage + 1}, dodge: {perfectDodgeDamage}, boss: {bossPowerUpDamage}");

        return perfectDodgeDamage + bossPowerUpDamage + 1;
    }

    private float lastAttackTime;
    private float lastDodgeTime;
    private float lastUniqueAbilityUseTime;
    private float lastPerfectDodgeTime;
    private float lastDashAttackTime;
    private float lastHitTime;
    private float perfectDodgeCounter = 0;
    private float dashAttackStartTime;
    private float dashAttackDamageMultiplier;
    private float currentBossfightTotalDamageDone = 0;
    private Vector3 startPosition;


    private int currentComboState;
    private int nextComboState;
    private int comboStateMax = 3;
    private int consecutiveHitsCount;

    private bool dashAttackUnlocked => upgradeStatus[AbilityUpgrade.Ability1];
    private bool unlimitedComboUnlocked => upgradeStatus[AbilityUpgrade.Ability2];
    private bool projectileDeflectionUnlocked => upgradeStatus[AbilityUpgrade.Ability3];
    private bool immortalitySpeedUpUnlocked => upgradeStatus[AbilityUpgrade.Ability4];
    private bool perfectDodgeExtraDamageUnlocked => upgradeStatus[AbilityUpgrade.Ability5];

    private bool isInvulnerable;
    private bool isDodging;
    private bool isDashingAttack;
    private bool isDashingAttackStarted;
    private bool canMove => !isDodging && !IsAttacking && !isDashingAttack;

    private bool IsAttacking
    {
        get => _isAttacking;
        set
        {
            _isAttacking = value;
            animator.SetBool("isAttacking", _isAttacking);
            if (!value)
            {
                nextComboState = 0;
                currentComboState = 0;
            }
        }
    }
    private bool _isAttacking;

    private ChargeVisualHandler chargeHandler;
    private PerfectTimingHandler perfectTimingHandler;


    #region Animation Variable
    private static string ATTACK = "Attack";
    private static string DODGESTART = "DodgeStart";
    private static string DODGEEND = "DodgeEnd";
    private static string HIT = "Hit";
    //private static string UNIQUE_ABILITY = "UniqueAbility";
    private static string STARTDASHATTACK = "StartDashAttack";
    private static string MOVEDASHATTACK = "MoveDashAttack";
    private static string ENDDASHATTACK = "EndDashAttack";
    //private static string DEATH = "Death";
    private static string ISMOVING = "IsMoving";
    #endregion

    public override float AttackSpeed => base.AttackSpeed + ExtraSpeed;
    public override float MoveSpeed => base.MoveSpeed + ExtraSpeed;
    public override float Damage => base.Damage * ExtraDamage();

    public override void Inizialize()
    {
        base.Inizialize();
        lastDodgeTime = -dodgeCooldown;
        lastAttackTime = -timeBetweenCombo;
        lastUniqueAbilityUseTime = -UniqueAbilityCooldown;
        lastDashAttackTime = -dashAttackCooldown;
        consecutiveHitsCount = 0;
        currentComboState = 0;
        nextComboState = 0;
        isInvulnerable = false;
        isDodging = false;
        IsAttacking = false;
        isDashingAttack = false;
        isDashingAttackStarted = false;
        chargeHandler = GetComponentInChildren<ChargeVisualHandler>();
        chargeHandler.Inizialize(minDashAttackDistance, maxDashAttackDistance, dashAttackMaxLoadUpTime, this);
        perfectTimingHandler = GetComponentInChildren<PerfectTimingHandler>();
        perfectTimingHandler.gameObject.SetActive(false);
        character = ePlayerCharacter.Brutus;

        //Debug
        //UnlockUpgrade(AbilityUpgrade.Ability1);

    }


    //Attack: combo rapida di tre attacchi melee, ravvicinati. 
    #region Attack
    public override void AttackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (canMove && CanStartCombo())
            {
                StartCombo();
            }
            else if (IsAttacking)
                ContinueCombo();
            Utility.DebugTrace($"Attacking: {IsAttacking}, AbiliyUpgrade2: {unlimitedComboUnlocked}, CooldownEnded: {Time.time > lastAttackTime + timeBetweenCombo} \n CurrentComboState: {currentComboState}, NextComboState: {nextComboState}");
        }
    }
    private void StartCombo()
    {
        currentComboState = 1;
        nextComboState = currentComboState;
        DoMeleeAttack();
        rb.velocity = Vector3.zero;
    }
    private void ContinueCombo()
    {
        if (currentComboState == nextComboState)
        {
            nextComboState = ++nextComboState > comboStateMax ? 0 : nextComboState;
            if (CanContinueCombo())
                DoMeleeAttack();
        }
    }
    private void DoMeleeAttack()
    {
        IsAttacking = true;
        string triggerName = ATTACK + (nextComboState).ToString();
        animator.SetTrigger(triggerName);

    }
    public void OnAttackAnimationEnd()
    {
        AdjustLastAttackTime();

        if (currentComboState == nextComboState || nextComboState == 0)
            IsAttacking = false;

        currentComboState = nextComboState;
    }

    private void AdjustLastAttackTime()
    {
        float comboCompletionValue = (float)currentComboState / (float)comboStateMax;
        float reductionFactor = (1 - comboCompletionValue) * timeBetweenCombo;
        lastAttackTime = Time.time - reductionFactor;
    }

    private bool CanStartCombo() => (unlimitedComboUnlocked || Time.time > lastAttackTime + timeBetweenCombo) && currentComboState != 1;
    private bool CanContinueCombo() => nextComboState != 0;
    private void ResetAttack()
    {
        IsAttacking = false;
        currentComboState = 0;
    }

    #endregion

    //Defense: fa una schivata, si sposta di tot distanza verso la direzione decisa dal giocatore con uno scatto
    #region Defense
    public override void DefenseInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Utility.DebugTrace($"Executed: {Time.time > lastDodgeTime + dodgeCooldown} ");
            if (Time.time > lastDodgeTime + dodgeCooldown && !isDodging && !isDashingAttack)
            {
                ResetAttack();
                lastDodgeTime = Time.time + dodgeDuration;
                StartCoroutine(Dodge(lastNonZeroDirection, rb));
            }
        }
    }

    protected IEnumerator Dodge(Vector2 dodgeDirection, Rigidbody2D rb)
    {
        isDodging = true;
        animator.SetTrigger(DODGESTART);
        PubSub.Instance.Notify(EMessageType.dodgeExecuted, this);
        yield return StartCoroutine(Move(dodgeDirection, rb, dodgeDuration, dodgeDistance));

        isDodging = false;
        animator.SetTrigger(DODGEEND);
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

    protected IEnumerator PerfectDodgeHandler(DamageData data)
    {
        perfectTimingHandler.gameObject.SetActive(true);
        yield return new WaitForSeconds(perfectDodgeDurarion);
        if (isDodging)
        {
            perfectDodgeCounter++;
            lastPerfectDodgeTime = Time.time;
            PubSub.Instance.Notify(EMessageType.perfectDodgeExecuted, this);
        }
        else
        {
            base.TakeDamage(data);
            if (!isDashingAttack)
            {
                animator.SetTrigger(HIT);

            }
                
        }
        perfectTimingHandler.gameObject.SetActive(false);
        
        Debug.Log($"PerfectDodge: {isDodging}, Count: {perfectDodgeCounter}");
    }


    #endregion

    //UniqueAbility: immortalit� per tot secondi
    #region UniqueAbility
    public override void UniqueAbilityInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            Utility.DebugTrace($"Executed: {!isInvulnerable && Time.time > lastUniqueAbilityUseTime + UniqueAbilityCooldown}");
            if (!isInvulnerable && Time.time > lastUniqueAbilityUseTime + UniqueAbilityCooldown)
            {
                lastUniqueAbilityUseTime = Time.time;
                uniqueAbilityUses++;
                StartCoroutine(UseUniqueAbilityCoroutine());
            }
        }
    }

    private IEnumerator UseUniqueAbilityCoroutine()
    {
        isInvulnerable = true;

        yield return new WaitForSeconds(invulnerabilityDuration);

        isInvulnerable = false;
    }
    #endregion

    //ExtraAbility: � l'ability upgrade 1
    #region ExtraAbility
    public override void ExtraAbilityInput(InputAction.CallbackContext context)
    {

        if (context.performed && dashAttackUnlocked && canMove && (Time.time - lastDashAttackTime > dashAttackCooldown))
        {
            Utility.DebugTrace("Performed");
            isDashingAttack = true;
            dashAttackStartTime = Time.time;
            rb.velocity = Vector2.zero;
            animator.SetTrigger(STARTDASHATTACK);
            chargeHandler.StartCharging(dashAttackStartTime);
        }

        if (context.canceled && dashAttackUnlocked && isDashingAttack && !isDashingAttackStarted)
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
        if (canMove)
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
    }


    public override void TakeDamage(DamageData data)
    {
        if (!isInvulnerable || !isDodging)
        {
            StartCoroutine(PerfectDodgeHandler(data));
        }
    }


    public override void UnlockUpgrade(AbilityUpgrade abilityUpgrade)
    {
        base.UnlockUpgrade(abilityUpgrade);
        if (abilityUpgrade == AbilityUpgrade.Ability3)
            AddDeflect();
        Debug.Log("Unlock" + abilityUpgrade.ToString());
    }

    

    public override void LockUpgrade(AbilityUpgrade abilityUpgrade)
    {
        base.LockUpgrade(abilityUpgrade);
        if (abilityUpgrade == AbilityUpgrade.Ability3)
            RemoveDeflect();
    }

    public void DeflectProjectile(Collider2D collider)
    {
        if (Utility.IsInLayerMask(collider.gameObject.layer, projectileLayer))
        {
            //Defletti il proiettile
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

    #region Damage

    public override DamageData GetDamageData()
    {
        BossDamageCheck();

        float damage = isDashingAttack ? base.Damage * dashAttackDamageMultiplier : Damage;

        TotalDamageUpdate(damage);

        Debug.Log($"Damage Done: {damage}");

        return new DamageData(damage, this);

    }


    private void BossDamageCheck()
    {
        if (isInBossfight && bossfightPowerUpUnlocked)
        {
            if (Time.time > lastHitTime + bossPowerUpExtraDamageDuration)
                consecutiveHitsCount = 0;

            consecutiveHitsCount++;
            lastHitTime = Time.time;
        }
    }

    private void TotalDamageUpdate(float damage)
    {
        currentBossfightTotalDamageDone += damage;
        if (currentBossfightTotalDamageDone > bossPowerUpTotalDamageToUnlock)
            bossfightPowerUpUnlocked = true;
    }


    #endregion
    //Potenziamento boss fight: gli attacchi consecutivi aumentano il danno del personaggio a ogni colpo andato a segno.
    //Dopo tot tempo (es: 1.5 secondi) senza colpire, il danno torna al valore standard.


    //Ottenimento potenziamento Boss fight: sbloccabile automaticamente dopo tot danni fatti al boss


    //Ability Upgrade:
    //1: Sblocca uno scatto in avanti + attacco
    //2: Annulla il tempo di ricarica tra le combo di attacchi
    //3: Il personaggio pu� respingere certi tipi di colpi(es: proiettili) con il suo attacco
    //4: quando il personaggio usa l�abilit� unica(i secondi di immortalit�) i suoi movimenti diventano pi� rapidi(attacchi, schivate e spostamenti)
    //5: Effettuare una schivata perfetta aumenta i danni per tot tempo(cumulabile con il bonus ai danni del potenziamento).

}
