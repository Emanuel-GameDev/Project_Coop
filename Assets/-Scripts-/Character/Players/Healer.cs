using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : CharacterClass
{
    [SerializeField] float attackDelay = 1f;

    [SerializeField] float singleHealCooldown = 5f;

    [SerializeField] GameObject healArea;
    [SerializeField] float healAreaRadius = 1f;
    [SerializeField] float healPerSeconds = 1f;

    [SerializeField] float bossPowerUpHeal = 50f;
    [SerializeField] int bossPowerUpHitToUnlock = 10;

    [SerializeField] float healAreaIncrementedRadious = 5;

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
        bool areaDamage = false;
        bool areaSlow = false;
        bool areaDebilitate = false;
        float radius = 0;

        ////Danno
        //if (upgradeStatus[AbilityUpgrade.Ability1])
        //    areaDamage = true;

        ////Rallentamento
        //if (upgradeStatus[AbilityUpgrade.Ability4])
        //    areaSlow = true;

        ////Abbassa difesa
        //if (upgradeStatus[AbilityUpgrade.Ability5])
        //    areaDebilitate = true;

        //calcolo area
        if (true)
            radius = healAreaRadius + healAreaIncrementedRadious;

        GameObject areaSpawned = Instantiate(healArea,new Vector3(parent.transform.position.x,0.1f,parent.transform.position.z),Quaternion.identity);
        areaSpawned.GetComponent<HealArea>().Initialize(
            healPerSeconds,
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
            //Crea mina
        }
    }

    public override void TakeDamage(float damage, Damager dealer)
    {
        base.TakeDamage(damage, dealer);
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
