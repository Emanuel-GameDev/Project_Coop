using System;
using Unity.Mathematics;
using UnityEngine;

public class Tank : CharacterClass
{
    [Header("Block")]
    [SerializeField, Tooltip("Quantità di danno parabile prima di rottura parata")]
    float staminaBlock;
    [SerializeField, Tooltip("Danno parata perfetta (Potenziamento 4)")]
    float perfectBlockDamage;

    [Header("Unique Ability")]
    [SerializeField, Tooltip("Durata aggro")]
    float aggroDuration;
    [SerializeField, Tooltip("Durata buff difesa")]
    float defenceBuffDuration;
    [SerializeField, Tooltip("Moltiplicatore buff difesa")]
    float defenceMultyplier;

    [Header("Bossfight Upgrade")]
    [SerializeField, Tooltip("Numero attacchi da parare perfettamente per ottenimento potenziamento bossfight")]
    int attacksToBlockForUpgrade = 10;
    [SerializeField, Tooltip("Cooldown attacco potenziamento bossfight")]
    float cooldownExtraAbility;
    [SerializeField, Tooltip("Durata stun attacco potenziamento boss fight")]
    float chargedAttackStunDuration;
    [SerializeField, Tooltip("Moltiplicatore durata stun (Potenziamento 2)")]
    float stunDurationMultyplier;


    private bool canDoubleAttack => upgradeStatus[AbilityUpgrade.Ability1];
    private bool hyperArmorUnlocked => upgradeStatus[AbilityUpgrade.Ability3];
    private bool hyperArmorOn;
    private bool isAttacking = false;
    private int comboIndex = 0;
    private int comboMax = 2;
    private float rangeAggro = math.INFINITY;
    private bool canPressInput;

   
   
    public override void Attack(Character parent)
    {
        isAttacking = true;  
        if(comboIndex == 0)
        {
            animator.SetTrigger("Attack1");
        }
        else if(canPressInput)
        {
            animator.SetTrigger("Attack2");
        }       

        
        //se potenziamento boss fight attacco caricato 
        //se potenziamento 5 attacco caricato (Da decidere)

        Debug.Log($"Attack[{comboIndex}  canDoubleAttack[{canDoubleAttack}  hasHyperArmor[{hyperArmorUnlocked}]");
    }
    public override void Defence(Character parent)
    {
        base.Defence(parent);
        //se potenziamento 4 parata perfetta fa danno
    }
    public override void UseExtraAbility(Character parent)
    {
        base.UseExtraAbility(parent);
        //se potenziamento boss attacco caricato e potenziamento 2 più stun
    }
    public override void UseUniqueAbility(Character parent)
    {       
        //attacco attiro aggro
    }
    public override void TakeDamage(float damage, IDamager dealer)
    {
        if(hyperArmorOn == false)
        {
            DoHitReacion();
        }
        
        
    }

    private void DoHitReacion()
    {
       
    }
    public void ActivateHyperArmor()
    {
        if(hyperArmorUnlocked)
        hyperArmorOn = true;

        Debug.Log("hyper armor on");
    }
    public void DeactivateHyperArmor()
    { 
        hyperArmorOn = false;
    }
    public void IncreaseComboIndex()
    {
        comboIndex++;
        if(comboIndex >1 )
        {
            comboIndex = 0;
        }
    }
}
