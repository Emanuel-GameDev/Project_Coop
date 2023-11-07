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

    [Tooltip("Icona che appare sopra il personaggio da curare")]
    [SerializeField] GameObject healIcon;
    [Tooltip("Quantità di vita curata dall'abilità di cura singola")]
    [SerializeField] float smallHeal = 5f;
    [Tooltip("Tempo di ricarica dell'abilità di cura singola")]
    [SerializeField] float singleHealCooldown = 5f;
    [Tooltip("Distanza massima a cui può essere un alleato per essere curato")]
    [SerializeField] float smallHealAreaRadius = 1f;


    [Header("Heal mine ability information")]

    [Tooltip("Prefab della mina")]
    [SerializeField] GameObject healMine;
    [Tooltip("Quantità di vita curata dalla mina")]
    [SerializeField] float mineHealQuantity = 1f;
    [Tooltip("Raggio del trigget della mina")]
    [SerializeField] float healMineRadius = 1f;
    [Tooltip("Tempo che la mina impiega prima di attivarsi")]
    [SerializeField] float healMineActivationTime = 1f;
    [Tooltip("Tempo di ricarica per poter piazzare la mina")]
    [SerializeField] float mineAbilityCooldown = 0;


    [Header("Heal area base information")]
    [Tooltip("Prefab dell'area di cura")]
    [SerializeField] GameObject healArea;
    [Tooltip("Durata dell' area di cura")]
    [SerializeField] float areaDuration=1;
    [Tooltip("Raggio dell'area di cura")]
    [SerializeField] float healAreaRadius = 1f;
    [Tooltip("Numero di tik al secondo")]
    [SerializeField] float tikPerSecond = 1;
    [Tooltip("Quantità di vita curata ogni tik")]
    [SerializeField] float healPerTik = 1f;

    [Header("Heal area upgrade stats")]
    [Tooltip("Quantità di vita tolta per tik (Abilità 1)")]
    [SerializeField] float DOTPerTik = 1;
    [Tooltip("Aumento del raggio dell'area di cura (Abilità 2)")]
    [SerializeField] float healAreaIncrementedRadious = 5;
    [Tooltip("Rallentamento (Abilità 4)")]
    [SerializeField] PowerUp slowDown;
    [Tooltip("Riduzione difesa (Abilità 5)")]
    [SerializeField] float damageIncrement = 1;

    [Header("Boss powerUp")]
    [Tooltip("Quantità di vita curata dall'abilità del boss")]
    [SerializeField] float bossPowerUpHeal = 50f;
    [Tooltip("Colpi consecutivi richiesti al boss, senza subire danni, per sbloccare l'abilità del boss")]
    [SerializeField] int bossPowerUpHitToUnlock = 10;

    GameObject instantiatedHealIcon;

    CapsuleCollider smallHealAreaCollider;
    
    List<PlayerCharacter> playerInArea;

    PlayerCharacter nearestPlayer;
    PlayerCharacter lastNearestPlayer;

    private float lastAttackTime;
    private float lastUniqueAbilityUseTime;
    private int bossPowerUpHitCount;

    float uniqueAbilityTimer;
    float mineAbilityTimer;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerCharacter>())
        {
            playerInArea.Add(other.gameObject.GetComponent<PlayerCharacter>());
            nearestPlayer = playerInArea.OrderBy(c => (transform.position - c.transform.position).sqrMagnitude).First();
        }
    }

    private void Start()
    {
        smallHealAreaCollider = gameObject.AddComponent<CapsuleCollider>();
        smallHealAreaCollider.isTrigger = true;
        smallHealAreaCollider.height = 1.5f;
        smallHealAreaCollider.radius = smallHealAreaRadius;


        uniqueAbilityTimer = UniqueAbilityCooldown;
        mineAbilityTimer = mineAbilityCooldown;

        //provvisorio
        instantiatedHealIcon = Instantiate(healIcon);
        MoveIcon(transform);
    }

    private void Update()
    {
        if (uniqueAbilityTimer < UniqueAbilityCooldown)
        {
            uniqueAbilityTimer += Time.deltaTime;
        }

        if (mineAbilityTimer < mineAbilityCooldown)
        {
            mineAbilityTimer += Time.deltaTime;
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

    public override void Inizialize(CharacterData characterData, Character character)
    {
        base.Inizialize(characterData, character);
        transform.position = character.transform.position;
        playerInArea = new List<PlayerCharacter>();
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
        if (uniqueAbilityTimer < UniqueAbilityCooldown)
            return;

        float radius = 1;

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

        uniqueAbilityTimer = 0;
    }


    //ExtraAbility: piazza mina di cura
    public override void UseExtraAbility(Character parent)
    {
        if (/*upgradeStatus[AbilityUpgrade.Ability3]*/ true)
        {
            if (mineAbilityTimer < mineAbilityCooldown)
                return;

            HealMine mine = Instantiate(healMine, new Vector3(parent.transform.position.x, 0.1f, parent.transform.position.z), Quaternion.identity).GetComponent<HealMine>();
            mine.Initialize(mineHealQuantity, healMineRadius, healMineActivationTime);

            mineAbilityTimer = 0;
        }
    }

    public override void TakeDamage(float damage, Damager dealer)
    {
        base.TakeDamage(damage, dealer);
        currentHp -= damage;
        bossPowerUpHitCount = 0;
    }


    //Cura tutti i player
    public void BossAbility()
    {
        if(bossfightPowerUpUnlocked)
        {

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
