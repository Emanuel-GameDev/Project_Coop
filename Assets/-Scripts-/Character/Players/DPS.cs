using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DPS : CharacterClass
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
    [SerializeField, Tooltip("Danno extra dopo una schivata perfetta.")]
    float perfectDodgeExtraDamage = 10;
    [Header("Unique Ability")]
    [SerializeField, Tooltip("Durata dell'invulnerabilità.")]
    float invulnerabilityDuration = 5f;
    [SerializeField, Tooltip("Aumento di velocità durante l'invulnerabilità.")]
    float invulnerabilitySpeedUp = 5f;
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
    [Header("Boss Power Up")]
    [SerializeField, Tooltip("Totale dei danni da fare al boss per sbloccare il potenziamento.")]
    float bossPowerUpTotalDamageToUnlock = 1000f;
    [SerializeField, Tooltip("Danno extra per colpo conferito dal potenziamento del boss.")]
    float bossPowerUpExtraDamagePerHit = 2f;
    [SerializeField, Tooltip("Limite massimo del danno extra conferito dal potenziamento del boss.")]
    float bossPowerUpExtraDamageCap = 16f;
    [SerializeField, Tooltip("Durata del danno extra conferito dal potenziamento del boss dopo l'ultimo colpo inferto.")]
    float bossPowerUpExtraDamageDuration = 2.5f;
    [Header("Other")]
    [SerializeField, Tooltip("I Layer da guardare quando ha sbloccato il power up per deflettere i proiettili")]
    LayerMask projectileLayer;


    private float extraSpeed => immortalitySpeedUpUnlocked && isInvulnerable ? invulnerabilitySpeedUp : 0;
    private float extraDamage => (perfectDodgeExtraDamageUnlocked && Time.time < lastPerfectDodgeTime + perfectDodgeExtraDamageDuration ? perfectDodgeExtraDamage : 0) + (bossfightPowerUpUnlocked ? MathF.Min(bossPowerUpExtraDamagePerHit * consecutiveHitsCount, bossPowerUpExtraDamageCap) : 0);
    private float lastAttackTime;
    private float lastDodgeTime;
    private float lastUniqueAbilityUseTime;
    private float lastPerfectDodgeTime;
    private float lastDashAttackTime;
    private float lastHitTime;
    private float totalDamageDone = 0;
    private float dashAttackStartTime;
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
        set { _isAttacking = value; animator.SetBool("isAttacking", _isAttacking); }
    }
    private bool _isAttacking;



    #region Animation Variable
    private static string ATTACK = "Attack";
    private static string DODGESTART = "DodgeStart";
    private static string DODGEEND = "DodgeEnd";
    //private static string HIT = "Hit";
    //private static string UNIQUE_ABILITY = "UniqueAbility";
    private static string STARTDASHATTACK = "StartDashAttack";
    private static string MOVEDASHATTACK = "MoveDashAttack";
    private static string ENDDASHATTACK = "EndDashAttack";
    //private static string DEATH = "Death";
    private static string ISMOVING = "IsMoving";
    #endregion

    public override float AttackSpeed => base.AttackSpeed + extraSpeed;
    public override float MoveSpeed => base.MoveSpeed + extraSpeed;
    public override float Damage => base.Damage + extraDamage;

    public override void Inizialize(CharacterData characterData, Character character)
    {
        base.Inizialize(characterData, character);
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
    }


    //Attack: combo rapida di tre attacchi melee, ravvicinati. 
    #region Attack
    public override void Attack(Character parent, InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!IsAttacking)
            {
                if (CanStartCombo())
                    StartCombo();
            }
            else
                ContinueCombo();
            Utility.DebugTrace($"Attacking: {IsAttacking}, AbiliyUpgrade2: {unlimitedComboUnlocked}, CooldownEnded: {Time.time > lastAttackTime + timeBetweenCombo} \n CurrentComboState: {currentComboState}, NextComboState: {nextComboState}");

        }

    }
    private void StartCombo()
    {
        currentComboState = 1;
        nextComboState = currentComboState;
        DoMeleeAttack();
        character.GetRigidBody().velocity = Vector3.zero;
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
        string triggerName = ATTACK + (nextComboState).ToString();
        animator.SetTrigger(triggerName);
        IsAttacking = true;
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

    private bool CanStartCombo() => unlimitedComboUnlocked || Time.time > lastAttackTime + timeBetweenCombo;
    private bool CanContinueCombo() => nextComboState != 0;

    #endregion

    //Defense: fa una schivata, si sposta di tot distanza verso la direzione decisa dal giocatore con uno scatto
    #region Defense
    public override void Defence(Character parent, InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Utility.DebugTrace($"Executed: {Time.time > lastDodgeTime + dodgeCooldown} ");
            if (Time.time > lastDodgeTime + dodgeCooldown && !isDodging)
            {
                lastDodgeTime = Time.time + dodgeDuration;
                StartCoroutine(Dodge(lastNonZeroDirection, parent.GetRigidBody()));
            }
        }
    }
    
    protected IEnumerator Dodge(Vector2 dodgeDirection, Rigidbody rb)
    {
        isDodging = true;
        animator.SetTrigger(DODGESTART);

        yield return StartCoroutine(Move(dodgeDirection, rb, dodgeDuration, dodgeDistance));

        isDodging = false;
        animator.SetTrigger(DODGEEND);
    }

    private IEnumerator Move(Vector2 direction, Rigidbody rb, float duration, float distance)
    {
        startPosition = character.transform.position;
        rb.velocity = Vector3.zero;

        Vector3 destination = startPosition + new Vector3(direction.x, 0f, direction.y).normalized * distance;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            rb.MovePosition(Vector3.Lerp(startPosition, destination, elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector3.zero;
    }

    #endregion

    //UniqueAbility: immortalità per tot secondi
    #region UniqueAbility
    public override void UseUniqueAbility(Character parent, InputAction.CallbackContext context)
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

    //ExtraAbility: è l'ability upgrade 1
    #region ExtraAbility
    public override void UseExtraAbility(Character parent, InputAction.CallbackContext context)
    {

        if (context.performed && dashAttackUnlocked && canMove && (Time.time - lastDashAttackTime > dashAttackCooldown))
        {
            Utility.DebugTrace("Performed");
            isDashingAttack = true;
            dashAttackStartTime = Time.time;
            animator.SetTrigger(STARTDASHATTACK);
        }

        if (context.canceled && dashAttackUnlocked && isDashingAttack && !isDashingAttackStarted)
        {
            isDashingAttackStarted = true;
            Utility.DebugTrace("Canceled");
            StartCoroutine(DashAttack(lastNonZeroDirection, parent.GetRigidBody()));
        }

    }

    protected IEnumerator DashAttack(Vector2 attackDirection, Rigidbody rb)
    {
        animator.SetTrigger(MOVEDASHATTACK);
        float pressDuration = Time.time - dashAttackStartTime;
        float dashAttackDistance = Mathf.Lerp(minDashAttackDistance, maxDashAttackDistance, pressDuration / dashAttackMaxLoadUpTime);
        
        yield return StartCoroutine(Move(attackDirection, rb, dashAttackDuration, dashAttackDistance));
        
        animator.SetTrigger(ENDDASHATTACK);
    }

    public void DashAttackTeleport()
    {
        character.GetRigidBody().MovePosition(startPosition);
        Debug.Log($"Teleport at: {startPosition}, current position: {character.transform.position}");
    }

    public void EndDashAttack()
    {
        isDashingAttack = false;
        isDashingAttackStarted = false;
        lastDashAttackTime = Time.time;
    }


    #endregion

    public override void Move(Vector2 direction, Rigidbody rb)
    {
        if (canMove)
        {
            base.Move(direction, rb);
        }
        animator.SetBool(ISMOVING, isMoving);
    }


    public override void TakeDamage(DamageData data)
    {
        if (!isInvulnerable)
            base.TakeDamage(data);
    }

    public override void UnlockUpgrade(AbilityUpgrade abilityUpgrade)
    {
        base.UnlockUpgrade(abilityUpgrade);
        if (abilityUpgrade == AbilityUpgrade.Ability3)
            damager.AssignFunctionToOnTrigger(DeflectProjectile);
        Debug.Log("Unlock" + abilityUpgrade.ToString());
    }

    public override void LockUpgrade(AbilityUpgrade abilityUpgrade)
    {
        base.LockUpgrade(abilityUpgrade);
        if (abilityUpgrade == AbilityUpgrade.Ability3)
            RemoveDeflect();
    }

    public void DeflectProjectile(Collider collider)
    {
        if (Utility.IsInLayerMask(collider.gameObject.layer, projectileLayer))
        {
            //Defletti il proiettile
        }

    }

    private void RemoveDeflect()
    {
        damager.RemoveFunctionFromOnTrigger(DeflectProjectile);
    }

    private void Update()
    {
        DamageCheck();
    }

    private void DamageCheck()
    {
        if (bossfightPowerUpUnlocked)
        {
            if (Time.time > lastHitTime + bossPowerUpExtraDamageDuration)
                consecutiveHitsCount = 0;
        }
        else
        {
            if (totalDamageDone > bossPowerUpTotalDamageToUnlock)
                bossfightPowerUpUnlocked = true;
        }
    }

    public override void Disable(Character character)
    {
        //base.Disable(character);
        if (projectileDeflectionUnlocked)
            RemoveDeflect();
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
