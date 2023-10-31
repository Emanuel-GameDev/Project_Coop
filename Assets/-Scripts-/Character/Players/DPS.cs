using System;
using System.Collections;
using UnityEngine;

public class DPS : CharacterClass
{
    [SerializeField, Tooltip("Tempo tra una combo e l'altra.")]
    float timeBetweenCombo = 1f;
    [SerializeField, Tooltip("Distanza di schivata.")]
    float dodgeDistance = 10f;
    [SerializeField, Tooltip("Velocit� di schivata.")]
    float dodgeSpeed = 10f;
    [SerializeField, Tooltip("Durata della schivata.")]
    float dodgeDuration = 0.25f;
    [SerializeField, Tooltip("Tempo di attesa prima di poter usare di nuovo la schivata.")]
    float dodgeCooldown = 5f;
    [SerializeField, Tooltip("Durata del danno extra dopo una schivata perfetta.")]
    float perfectDodgeExtraDamageDuration = 5f;
    [SerializeField, Tooltip("Danno extra dopo una schivata perfetta.")]
    float perfectDodgeExtraDamage = 10;
    [SerializeField, Tooltip("Durata dell'invulnerabilit�.")]
    float invulnerabilityDuration = 5f;
    [SerializeField, Tooltip("Aumento di velocit� durante l'invulnerabilit�.")]
    float invulnerabilitySpeedUp = 5f;
    [SerializeField, Tooltip("Totale dei danni da fare al boss per sbloccare il potenziamento.")]
    float bossPowerUpTotalDamageToUnlock = 1000f;
    [SerializeField, Tooltip("Danno extra per colpo conferito dal potenziamento del boss.")]
    float bossPowerUpExtraDamagePerHit = 2f;
    [SerializeField, Tooltip("Limite massimo del danno extra conferito dal potenziamento del boss.")]
    float bossPowerUpExtraDamageCap = 16f;
    [SerializeField, Tooltip("Durata del danno extra conferito dal potenziamento del boss dopo l'ultimo colpo inferto.")]
    float bossPowerUpExtraDamageDuration = 2.5f;

    private float extraSpeed => upgradeStatus[AbilityUpgrade.Ability4] && isInvulnerable ? invulnerabilitySpeedUp : 0;
    private float extraDamage => (upgradeStatus[AbilityUpgrade.Ability5] && Time.time < lastPerfectDodgeTime + perfectDodgeExtraDamageDuration ? perfectDodgeExtraDamage : 0) + (bossfightPowerUpUnlocked ? MathF.Min(bossPowerUpExtraDamagePerHit * consecutiveHitsCount, bossPowerUpExtraDamageCap) : 0);
    private float lastAttackTime;
    private float lastDodgeTime;
    private float lastUniqueAbilityUseTime;
    private float lastPerfectDodgeTime;
    private float lastHitTime;
    private float totalDamageDone = 0;
    private int consecutiveHitsCount;
    private bool isInvulnerable;
    private bool isDodging;

    private Vector2 lastDirection;


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
        isInvulnerable = false;
        isDodging = false;
    }


    //Attack: combo rapida di tre attacchi melee, ravvicinati. 
    public override void Attack(Character parent)
    {
        Utility.DebugTrace($"Executed: {upgradeStatus[AbilityUpgrade.Ability2] || Time.time > lastAttackTime + timeBetweenCombo}");
        if (upgradeStatus[AbilityUpgrade.Ability2] || Time.time > lastAttackTime + timeBetweenCombo)
        {
            lastAttackTime = Time.time;
        }
    }

    //Defense: fa una schivata, si sposta di tot distanza verso la direzione decisa dal giocatore con uno scatto
    public override void Defence(Character parent)
    {
        Utility.DebugTrace($"Executed: {Time.time > lastDodgeTime + dodgeCooldown} ");
        if (Time.time > lastDodgeTime + dodgeCooldown)
        {
            lastDodgeTime = Time.time;
            Dodge(lastDirection, parent.GetRigidBody());
        }
    }

    protected IEnumerator Dodge(Vector3 dodgeDirection, Rigidbody rb)
    {
        if (!isDodging)
        {
            isDodging = true;
            float startTime = Time.time;
            float distanceCovered = 0f;

            while (Time.time - startTime < dodgeDuration && distanceCovered < dodgeDistance)
            {
                float currentDodgeSpeed = dodgeDistance / dodgeDuration;
                rb.velocity = dodgeDirection * currentDodgeSpeed;
                distanceCovered += currentDodgeSpeed * Time.deltaTime;
                yield return null;
            }


            rb.velocity = Vector3.zero;

            isDodging = false;
        }
    }



    //UniqueAbility: immortalit� per tot secondi
    public override void UseUniqueAbility(Character parent)
    {
        Utility.DebugTrace($"Executed: {!isInvulnerable && Time.time > lastUniqueAbilityUseTime + UniqueAbilityCooldown}");
        if (!isInvulnerable && Time.time > lastUniqueAbilityUseTime + UniqueAbilityCooldown)
        {
            lastUniqueAbilityUseTime = Time.time;
            isInvulnerable = true;
            uniqueAbilityUses++;
        }
    }

    //ExtraAbility: � l'ability upgrade 1
    public override void UseExtraAbility(Character parent)
    {
        if (upgradeStatus[AbilityUpgrade.Ability1])
        {
            //Scatto in avanti pi� attacco
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
        InvulnerabilityCheck();
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

    private void InvulnerabilityCheck()
    {
        if (isInvulnerable && Time.time > lastUniqueAbilityUseTime + invulnerabilityDuration)
            isInvulnerable = false;
    }


    //Potenziamento boss fight: gli attacchi consecutivi aumentano il danno del personaggio a ogni colpo andato a segno.
    //Dopo tot tempo (es: 1.5 secondi) senza colpire, il danno torna al valore standard.


    //Ottenimento potenziamento Boss fight: sbloccabile automaticamente dopo tot danni fatti al boss


    //Ability Upgrade:
    //1: Sblocca uno scatto in avanti + attacco
    //2: Annulla il tempo di ricarica tra le combo di attacchi
    //3: Il personaggio pu� respingere certi tipi di colpi(es: proiettili) con il suo attacco
    //4: quando il personaggio usa l�abilit� unica(i secondi di immortalit�) i suoi movimenti diventano pi� rapidi(attacchi, schivate e spostamenti)
    //5: Effettuare una schivata perfetta aumenta i danni per tot tempo(cumulabile con il bonus ai danni del potenziamento).

    //Debug:

}
