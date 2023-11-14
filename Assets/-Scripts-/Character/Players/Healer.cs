using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class Healer : CharacterClass
{
    [SerializeField] float attackDelay = 1f;


    [Header("Small heal ability information")]
    [SerializeField] GameObject healIcon;
    [SerializeField] float smallHeal = 5f;
    [SerializeField] float singleHealCooldown = 5f;
    [SerializeField] float smallHealAreaRadius = 1f;

    [Header("Heal mine ability information")]
    [SerializeField] GameObject healMine;
    [SerializeField] float mineHealQuantity = 1f;
    [SerializeField] float healMineRadius = 1f;
    [SerializeField] float healMineActivationTime = 1f;

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
    [SerializeField] PowerUp slowDown;
    //abilità difesa
    [SerializeField] float damageIncrement = 1;

    [Header("Boss powerUp")]
    [SerializeField] float bossPowerUpHeal = 50f;
    [SerializeField] int bossPowerUpHitToUnlock = 10;

    GameObject instantiatedHealIcon;

    CapsuleCollider smallHealAreaCollider;
    
    List<PlayerCharacter> playerInArea;

    PlayerCharacter nearestPlayer;
    PlayerCharacter lastNearestPlayer;

    private float lastAttackTime;
    private float lastUniqueAbilityUseTime;
    private int bossPowerUpHitCount = 0;

    public override void Inizialize(CharacterData characterData, Character character)
    {
        base.Inizialize(characterData, character);
        transform.position = character.transform.position;
        playerInArea = new List<PlayerCharacter>();
        smallHealAreaCollider = gameObject.AddComponent<CapsuleCollider>();
        smallHealAreaCollider.isTrigger = true;
        smallHealAreaCollider.height = 1.5f;
        smallHealAreaCollider.radius = smallHealAreaRadius;


        //provvisorio
        instantiatedHealIcon = Instantiate(healIcon);
        MoveIcon(transform);
    }


    private void MoveIcon(Transform newParent)
    {
        instantiatedHealIcon.transform.SetParent(newParent);
        instantiatedHealIcon.transform.localPosition = new Vector3(0, 1, 0);
    }

    
    //Attack: colpo singolo, incremento colpi consecutivi senza subire danni contro boss
    public override void Attack(Character parent)
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerCharacter>())
        {
            playerInArea.Add(other.gameObject.GetComponent<PlayerCharacter>());
            nearestPlayer = playerInArea.OrderBy(c => (transform.position - c.transform.position).sqrMagnitude).First();
        }
    }

    private void FixedUpdate()
    {
        if (lastNearestPlayer != nearestPlayer)
        {
            if (playerInArea.Count == 0)
                MoveIcon(transform);
            else
                MoveIcon(nearestPlayer.transform);
            
            lastNearestPlayer = nearestPlayer;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerCharacter>())
        {
            playerInArea.Remove(other.gameObject.GetComponent<PlayerCharacter>());
            if (playerInArea.Count > 0)
                nearestPlayer = playerInArea.OrderBy(c => (transform.position - c.transform.position).sqrMagnitude).First();
            else
                nearestPlayer = null;
        }
    }

    //Defense: cura ridotta singola
    public override void Defence(Character parent)
    {
        if (nearestPlayer == null)
            TakeDamage(-smallHeal, null);
        else
            nearestPlayer.TakeDamage(-smallHeal, null);
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

        areaSpawned.healPerTik = healPerTik;
        areaSpawned.DOTPerTik = DOTPerTik;
        areaSpawned.slowDown = slowDown;
        areaSpawned.damageIncrement = damageIncrement;
    }

    //ExtraAbility: piazza mina di cura
    public override void UseExtraAbility(Character parent)
    {
        if (upgradeStatus[AbilityUpgrade.Ability3])
        {
            HealMine mine = Instantiate(healMine, new Vector3(parent.transform.position.x, 0.1f, parent.transform.position.z), Quaternion.identity).GetComponent<HealMine>();
            mine.Initialize(mineHealQuantity, healMineRadius, healMineActivationTime);
            
        }
    }

    public override void TakeDamage(float damage, IDamager dealer)
    {
        base.TakeDamage(damage, dealer);
        currentHp -= damage;
        bossPowerUpHitCount = 0;
    }

    public void BossAbility()
    {
        if(bossfightPowerUpUnlocked)
        {
            //Cura tutti i player
        }
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
