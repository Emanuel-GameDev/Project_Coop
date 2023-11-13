using System;
using System.Collections;
using UnityEngine;

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
    [Header("Boss Power Up")]
    [SerializeField, Tooltip("Totale dei danni da fare al boss per sbloccare il potenziamento.")]
    float bossPowerUpTotalDamageToUnlock = 1000f;
    [SerializeField, Tooltip("Danno extra per colpo conferito dal potenziamento del boss.")]
    float bossPowerUpExtraDamagePerHit = 2f;
    [SerializeField, Tooltip("Limite massimo del danno extra conferito dal potenziamento del boss.")]
    float bossPowerUpExtraDamageCap = 16f;
    [SerializeField, Tooltip("Durata del danno extra conferito dal potenziamento del boss dopo l'ultimo colpo inferto.")]
    float bossPowerUpExtraDamageDuration = 2.5f;

    private float extraSpeed => immortalitySpeedUpUnlocked && isInvulnerable ? invulnerabilitySpeedUp : 0;
    private float extraDamage => (perfectDodgeExtraDamageUnlocked && Time.time < lastPerfectDodgeTime + perfectDodgeExtraDamageDuration ? perfectDodgeExtraDamage : 0) + (bossfightPowerUpUnlocked ? MathF.Min(bossPowerUpExtraDamagePerHit * consecutiveHitsCount, bossPowerUpExtraDamageCap) : 0);
    private float lastAttackTime;
    private float lastDodgeTime;
    private float lastUniqueAbilityUseTime;
    private float lastPerfectDodgeTime;
    private float lastHitTime;
    private float totalDamageDone = 0;

    private int comboState;
    private int comboStateMax = 3;
    private int consecutiveHitsCount;

    private bool dashAttackUnlocked => upgradeStatus[AbilityUpgrade.Ability1];
    private bool unlimitedComboUnlocked => upgradeStatus[AbilityUpgrade.Ability2];
    private bool projectileDeflectionUnlocked => upgradeStatus[AbilityUpgrade.Ability3];
    private bool immortalitySpeedUpUnlocked => upgradeStatus[AbilityUpgrade.Ability4];
    private bool perfectDodgeExtraDamageUnlocked => upgradeStatus[AbilityUpgrade.Ability5];

    private bool isInvulnerable;
    private bool isDodging;

    private bool mustContinueCombo;
    private bool IsAttacking
    {
        get => _isAttacking;
        set { _isAttacking = value; animator.SetBool("isAttacking", _isAttacking); }
    }
    private bool _isAttacking;

    private Vector2 lastDirection;

    #region Animation Variable
    private static string ATTACK = "Attack";
    //private static string DODGE = "Dodge";
    //private static string HIT = "Hit";
    //private static string UNIQUE_ABILITY = "UniqueAbility";
    //private static string EXTRA_ABILITY = "ExtraAbility";
    //private static string DEATH = "Death";
    //private static string MOVING = "Moving";
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
        consecutiveHitsCount = 0;
        comboState = 0;
        isInvulnerable = false;
        isDodging = false;
        IsAttacking = false;
    }


    //Attack: combo rapida di tre attacchi melee, ravvicinati. 
    #region Attack
    public override void Attack(Character parent)
    {
        float currentTime = Time.time;
        //Utility.DebugTrace($"Attacking: {IsAttacking}, AbiliyUpgrade2: {unlimitedComboUnlocked}, CooldownEnded: {currentTime > lastAttackTime + timeBetweenCombo}, ComboState: {comboState}");
        if (!IsAttacking)
        {
            if (CanStartCombo(currentTime))
            {// Avvia l'animazione per il primo attacco della combo
                StartCombo(currentTime);
            }
        }
        else
        {
            //// Avvia l'animazione per l'attacco successivo
            //if (IsCurrentAttackAnimation())
            //{
            //    ContinueCombo(currentTime);
            //}
            mustContinueCombo = true;
        }
    }
    private void StartCombo(float currentTime)
    {
        comboState = 1;
        DoMeleeAttack(currentTime);
    }
    //private void ContinueCombo(float currentTime)
    //{
    //    comboState++;
    //    if (comboState > comboStateMax)
    //        comboState = 0;
    //    else
    //        DoMeleeAttack(currentTime);
    //}
    private void DoMeleeAttack(float currentTime)
    {
        string triggerName = ATTACK + (comboState).ToString();
        animator.SetTrigger(triggerName);
        IsAttacking = true;
        lastAttackTime = currentTime;
    }
    

    public void OnAttackAnimationEnd()
    {
        lastAttackTime = Time.time;
        comboState = comboState++ > comboStateMax ? 0 : comboState;
        if (mustContinueCombo && CanContinueCombo())
        {
            mustContinueCombo = false;
            DoMeleeAttack(Time.time);
        }
        else
            IsAttacking = false;
    }
    //private bool IsCurrentAttackAnimation()
    //{
    //    return animator.GetCurrentAnimatorStateInfo(0).IsName(ATTACK + (comboState).ToString());
    //}
    private bool CanStartCombo(float currentTime)
    {
        return unlimitedComboUnlocked || currentTime > lastAttackTime + timeBetweenCombo;
    }
    private bool CanContinueCombo()
    {
        return ((comboState == 0 && unlimitedComboUnlocked) || comboState != 0);
    }

    //private void CheckAttackStatus()
    //{
    //    if (IsAttacking && !animator.GetBool("isAttacking"))
    //        IsAttacking = false;
    //}
    #endregion

    //Defense: fa una schivata, si sposta di tot distanza verso la direzione decisa dal giocatore con uno scatto
    public override void Defence(Character parent)
    {
        Utility.DebugTrace($"Executed: {Time.time > lastDodgeTime + dodgeCooldown} ");
        if (Time.time > lastDodgeTime + dodgeCooldown)
        {
            lastDodgeTime = Time.time + dodgeDuration;
            StartCoroutine(Dodge(lastDirection, parent.GetRigidBody()));
            Debug.Log(lastDirection);
        }
    }

    protected IEnumerator Dodge(Vector2 dodgeDirection, Rigidbody rb)
    {
        if (!isDodging)
        {
            isDodging = true;
            //animator.SetBool(DODGE, isDodging);
            Vector3 dodgeDirection3D = new Vector3(dodgeDirection.x, 0f, dodgeDirection.y).normalized;
            rb.velocity = dodgeDirection3D * (dodgeDistance / dodgeDuration);

            yield return new WaitForSeconds(dodgeDuration);

            rb.velocity = Vector3.zero;

            isDodging = false;
            //animator.SetBool(DODGE, isDodging);
        }
    }

    //UniqueAbility: immortalità per tot secondi
    public override void UseUniqueAbility(Character parent)
    {
        Utility.DebugTrace($"Executed: {!isInvulnerable && Time.time > lastUniqueAbilityUseTime + UniqueAbilityCooldown}");
        if (!isInvulnerable && Time.time > lastUniqueAbilityUseTime + UniqueAbilityCooldown)
        {
            lastUniqueAbilityUseTime = Time.time;
            uniqueAbilityUses++;
            StartCoroutine(UseUniqueAbilityCoroutine());
        }
    }

    private IEnumerator UseUniqueAbilityCoroutine()
    {
        isInvulnerable = true;

        yield return new WaitForSeconds(invulnerabilityDuration);

        isInvulnerable = false;
    }

    //ExtraAbility: è l'ability upgrade 1
    public override void UseExtraAbility(Character parent)
    {
        if (dashAttackUnlocked)
        {
            //Scatto in avanti più attacco
        }
        Utility.DebugTrace();
    }

    public override void Move(Vector2 direction, Rigidbody rb)
    {
        if (!isDodging)
        {
            base.Move(direction, rb);
            if (direction != Vector2.zero)
                lastDirection = direction;
        }
    }


    public override void TakeDamage(float damage, Damager dealer)
    {
        if (!isInvulnerable)
            base.TakeDamage(damage, dealer);
    }

    public override void UnlockUpgrade(AbilityUpgrade abilityUpgrade)
    {
        base.UnlockUpgrade(abilityUpgrade);
        if (abilityUpgrade == AbilityUpgrade.Ability3)
            character.GetDamager().gameObject.AddComponent<DeflectProjectile>();
        Debug.Log("Unlock" + abilityUpgrade.ToString());
    }

    public override void LockUpgrade(AbilityUpgrade abilityUpgrade)
    {
        base.LockUpgrade(abilityUpgrade);
        if (abilityUpgrade == AbilityUpgrade.Ability3)
            RemoveDeflect();
    }

    private void RemoveDeflect()
    {
        DeflectProjectile deflect = character.GetDamager().gameObject.GetComponent<DeflectProjectile>();
        if (deflect != null)
            Destroy(deflect);
    }

    private void Update()
    {
        DamageCheck();
        //CheckAttackStatus();
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
        base.Disable(character);
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
