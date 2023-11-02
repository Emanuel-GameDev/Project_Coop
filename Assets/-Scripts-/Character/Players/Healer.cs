using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : CharacterClass
{
    [SerializeField] float attackDelay = 1f;

    [SerializeField] float singleHealCooldown = 5f;

    [SerializeField] GameObject healMine;

    [Header("Heal area base information")]
    [SerializeField] GameObject healArea;
    
    [SerializeField] float areaDuration=1;
    [SerializeField] float healAreaRadius = 1f;
    [SerializeField] float tikPerSecond = 1;
    [SerializeField] float healPerTik = 1f;

    [Header("Heal area upgrade stats")]
    //abilità danno
    [SerializeField] float DOTPerTik = 1;
    //abilità raggio
    [SerializeField] float healAreaIncrementedRadious = 5;
    //abilità slow
    [SerializeField] float speedPenalty = -1;
    //abilità difesa
    [SerializeField] float damageIncrement = 1;

    [Header("Boss powerUp")]
    [SerializeField] float bossPowerUpHeal = 50f;
    [SerializeField] int bossPowerUpHitToUnlock = 10;


    private float lastAttackTime;
    private float lastUniqueAbilityUseTime;
    private int bossPowerUpHitCount = 0;

    //Attack: colpo singolo, incremento colpi consecutivi senza subire danni contro boss
    public override void Attack(Character parent)
    {
        
    }

    //Defense: cura ridotta singola
    public override void Defence(Character parent)
    {
        
    }

    //UniqueAbility: lancia area di cura
    public override void UseUniqueAbility(Character parent)
    {
        float radius = 0;

        //calcolo area
        if (upgradeStatus[AbilityUpgrade.Ability2])
            radius = healAreaRadius + healAreaIncrementedRadious;
        else
            radius = healAreaRadius;
        

        HealArea areaSpawned = Instantiate(healArea,new Vector3(parent.transform.position.x,0,parent.transform.position.z),Quaternion.identity).GetComponent<HealArea>();
       


        areaSpawned.Initialize(
            areaDuration,
            tikPerSecond,
            radius,
            upgradeStatus[AbilityUpgrade.Ability1],
            upgradeStatus[AbilityUpgrade.Ability4],
            upgradeStatus[AbilityUpgrade.Ability5]);

        

    }

    //ExtraAbility: piazza mina di cura
    public override void UseExtraAbility(Character parent)
    {
        if (upgradeStatus[AbilityUpgrade.Ability3])
        {
            Instantiate(healMine, new Vector3(parent.transform.position.x, 0.1f, parent.transform.position.z), Quaternion.identity);
        }
    }

    public override void TakeDamage(float damage, Damager dealer)
    {
        base.TakeDamage(damage, dealer);
        currentHp -= damage;
        bossPowerUpHitCount = 0;
    }

    

    //Potenziamento boss fight: cura istantanea in tutta la mappa


    //Ottenimento potenziamento Boss fight: Colpire il boss tot volte senza subire danni


    //Ability Upgrade:
    //1: L’area di cura intorno al personaggio danneggia i nemici al suo interno
    //2: L’area di cura diventa più ampia
    //3: Il personaggio lascia a terra una “mina” che cura i giocatori che ci passano sopra
    //4: L’area di cura intorno al personaggio rallenta i nemici al suo interno (cumulabile con altre abilità simili)
    //5: L’area di cura intorno al personaggio riduce la difesa dei nemici al suo interno (cumulabile con altre abilità simili)
}
