using System;
using UnityEngine;

public class DPS : CharacterClass
{
    [SerializeField] float timeBetweenCombo = 1f;
    [SerializeField] float dodgeDistance = 10f;
    [SerializeField] float dodgeSpeed = 10f;
    [SerializeField] float dodgeCooldown = 5f;
    [SerializeField] float perfectDodgeExtraDamageDuration = 5f;
    [SerializeField] float perfectDodgeExtraDamage = 10;
    [SerializeField] float invulnerabilityDuration = 5f;
    [SerializeField] float invulnerabilitySpeedUp = 5f;
    [SerializeField] float bossPowerUpTotalDamageToUnlock = 1000f;
    [SerializeField] float bossPowerUpExtraDamagePerHit = 2f;
    [SerializeField] float bossPowerUpExtraDamageCap = 16f;
    [SerializeField] float bossPowerUpExtraDamagePermanence = 2.5f;
    private float extraSpeed => upgradeStatus[AbilityUpgrade.Ability4] && isInvulnerable ? invulnerabilitySpeedUp : 0;
    private float extraDamage => (upgradeStatus[AbilityUpgrade.Ability5] && Time.time < lastPerfectDodgeTime + perfectDodgeExtraDamageDuration ? perfectDodgeExtraDamage : 0) + (bossfightPowerUpUnlocked ? MathF.Min(bossPowerUpExtraDamagePerHit * consecutiveHitsCount, bossPowerUpExtraDamageCap) : 0);
    private float lastAttackTime;
    private float lastDashTime;
    private float lastUniqueAbilityUseTime;
    private float lastPerfectDodgeTime;
    private float lastHitTime;
    private int consecutiveHitsCount = 0;
    private bool isInvulnerable = false;
    private float totalDamageDone = 0;

    public override float AttackSpeed => base.AttackSpeed + extraSpeed;
    public override float MoveSpeed => base.MoveSpeed + extraSpeed;
    public override float Damage => base.Damage + extraDamage;

    //Attack: combo rapida di tre attacchi melee, ravvicinati. 
    public override void Attack(Character parent)
    {
        if (upgradeStatus[AbilityUpgrade.Ability2] || Time.time > lastAttackTime + timeBetweenCombo)
        {
            //DoMeleeAttacks();
        }
    }

    //Defense: fa una schivata, si sposta di tot distanza verso la direzione decisa dal giocatore con uno scatto
    public override void Defence(Character parent)
    {
        if (Time.time > lastDashTime + dodgeCooldown)
        {
            //Fai schivata
        }
    }

    //UniqueAbility: immortalità per tot secondi
    public override void UseUniqueAbility(Character parent)
    {
        if (!isInvulnerable && Time.time > lastUniqueAbilityUseTime + UniqueAbilityCooldown)
        {
            isInvulnerable = true;
            uniqueAbilityUses++;
        }
    }

    //ExtraAbility: è l'ability upgrade 1
    public override void UseExtraAbility(Character parent)
    {
        if (upgradeStatus[AbilityUpgrade.Ability1])
        {
            //Scatto in avanti più attacco
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
            if (Time.time > lastHitTime + bossPowerUpExtraDamagePermanence)
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
    //3: Il personaggio può respingere certi tipi di colpi(es: proiettili) con il suo attacco
    //4: quando il personaggio usa l’abilità unica(i secondi di immortalità) i suoi movimenti diventano più rapidi(attacchi, schivate e spostamenti)
    //5: Effettuare una schivata perfetta aumenta i danni per tot tempo(cumulabile con il bonus ai danni del potenziamento).

}
