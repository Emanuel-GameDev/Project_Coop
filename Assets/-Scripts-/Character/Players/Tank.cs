using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;


//Unique = tasto nord,Q = Urlo
//BossUpgrade = tasto est,E = attacco bossFight
public class Tank : CharacterClass
{
    [Header("Attack")]
    [SerializeField, Tooltip("Durata tempo pressione prolungata tasto attaco prima per decidere se attacco caricato o non")]
    float timeCheckAttackType = 0.2f;
    [SerializeField, Tooltip("Tempo minimo attacco caricato per essere eseguito")]
    float chargedAttackTimer = 2.5f;

    [Header("Block")]
    [SerializeField, Tooltip("Quantità di danno parabile prima di rottura parata")]
    float staminaBlock;
    [SerializeField, Tooltip("Danno parata perfetta")]
    float perfectBlockDamage;

    [Header("Unique Ability")]
    [SerializeField, Tooltip("Durata aggro")]
    float aggroDuration;
    [SerializeField, Tooltip("Durata buff difesa")]
    float defenceBuffDuration;
    [SerializeField, Tooltip("Moltiplicatore buff difesa")]
    float defenceMultyplier;
    [SerializeField, Tooltip("Moltiplicatore buff stamina")]
    float staminaMultyplier;

    [Header("Bossfight Upgrade")]
    [SerializeField, Tooltip("Numero attacchi da parare perfettamente per ottenimento potenziamento bossfight")]
    int attacksToBlockForUpgrade = 10;
    [SerializeField, Tooltip("Cooldown attacco potenziamento bossfight")]
    float cooldownExtraAbility;
    [SerializeField, Tooltip("Durata stun attacco potenziamento boss fight")]
    float chargedAttackStunDuration = 5;
    [SerializeField, Tooltip("Moltiplicatore durata stun")]
    float stunDurationMultyplier;


    private bool doubleAttack => upgradeStatus[AbilityUpgrade.Ability1];
    private bool maximizedStun => upgradeStatus[AbilityUpgrade.Ability2];
    private bool implacableAttack => upgradeStatus[AbilityUpgrade.Ability3];
    private bool damageOnParry => upgradeStatus[AbilityUpgrade.Ability4];
    private bool chargedAttack => upgradeStatus[AbilityUpgrade.Ability5];


    private bool hyperArmorOn;
    private bool isAttacking = false;
    private bool bossfightUpgradeUnlocked;
    private bool canPressInput;
    private bool pressed;
    private bool chargedAttackReady;
    private bool canCancelChargedAttck;

    private int comboIndex = 0;
    private int comboMax = 2;
    private int perfectBlockCount;
    private float rangeAggro = math.INFINITY;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            bossfightPowerUpUnlocked = true;
        }
    }

    //Da Vedere Combo
    #region Attack

    public override void Attack(Character parent, InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ActivateHyperArmor();
            pressed = true;
            Invoke(nameof(CheckAttackToDo), timeCheckAttackType);

        }

        else if (context.canceled)
        {

            pressed = false;
            if (chargedAttackReady)
            {

                animator.SetBool("ChargedAttack", false);
                Debug.Log("Charged Attack executed");
            }
            else if (!chargedAttackReady && canCancelChargedAttck)
            {

                animator.SetTrigger("Attack1");
                Debug.Log($"combo index:[{comboIndex}]   can Double Attack[{doubleAttack}]");
            }

        }

    }
    public void IncreaseComboIndex()
    {
        comboIndex++;
        if (comboIndex > 2)
        {
            comboIndex = 0;
        }
    }
    public void CheckAttackToDo()
    {
        if (pressed && chargedAttack)
        {
            isAttacking = true;
            animator.SetBool("ChargedAttack", pressed);
            Debug.Log($"Started Charged Attack");
            StartCoroutine(StartChargedAttackTimer());

        }

        else if (!pressed)
        {
            if (comboIndex == 0 && !isAttacking)
            {
                isAttacking = true;
                animator.SetTrigger("Attack1");
                IncreaseComboIndex();
            }
            else if (doubleAttack && isAttacking)
            {
                if(comboIndex != 2) 
                {
                    animator.SetTrigger("Attack2");
                    IncreaseComboIndex();
                }
                
            }

        }
    }
    IEnumerator StartChargedAttackTimer()
    {
        chargedAttackReady = false;
        canCancelChargedAttck = true;
        yield return new WaitForSeconds(chargedAttackTimer);
        if (pressed)
        {
            canCancelChargedAttck = false;
            chargedAttackReady = true;
            Debug.Log("Charged Attack Ready");
            //Segnale Visivo
        }

    }
    public void ActivateHyperArmor()
    {
        if (implacableAttack)
        {
            hyperArmorOn = true;

            Debug.Log("hyper armor on");
        }

    }
    public void DeactivateHyperArmor()
    {
        hyperArmorOn = false;
    }
    public void ResetAttackCombo(int endCombo)
    {
        if (comboIndex != 2 || endCombo == 1) 
        {
            comboIndex = 0;
            isAttacking = false;
            Debug.Log("Reset Variables");
        }

        
    }
    public void aaaaaaaaaaaaaaaaaaaaaa()
    {
        Debug.Log($"combo index:[{comboIndex}] can Double Attack[{doubleAttack}]");
    }



    #endregion

    #region Block

    public override void Defence(Character parent, InputAction.CallbackContext context)
    {
        base.Defence(parent, context);
        //se potenziamento 4 parata perfetta fa danno
    }
    #endregion

    #region onHit
    public override void TakeDamage(DamageData data)
    {
        if (hyperArmorOn == false)
        {
            DoHitReacion();
        }
    }

    private void DoHitReacion()
    {

    }
    #endregion

    public override void UseExtraAbility(Character parent, InputAction.CallbackContext context) //Tasto est
    {
        if (context.performed)
        {
            if (bossfightPowerUpUnlocked && isAttacking == false)
            {
                float stunDamageDuration = maximizedStun ? (chargedAttackStunDuration * stunDurationMultyplier) : chargedAttackStunDuration;
                Debug.Log($"BossFight Upgrade Attack Executed, stun duration:[{stunDamageDuration}]");
            }
        }

    }
    public override void UseUniqueAbility(Character parent, InputAction.CallbackContext context)
    {
        base.UseUniqueAbility(parent, context);
        //attacco attiro aggro

    }







}
