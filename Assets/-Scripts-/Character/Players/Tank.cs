using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;


//Unique = tasto nord,Q = Urlo
//BossUpgrade = tasto est,E = attacco bossFight
public class Tank : CharacterClass
{
    [Header("Attack")]
    [SerializeField, Tooltip("Durata tempo pressione tasto attacco prima per decidere se attacco caricato o no")]
    float timeCheckAttackType = 0.2f;
    [SerializeField, Tooltip("Tempo minimo attacco caricato per essere eseguito")]
    float chargedAttackTimer = 2.5f;
    [SerializeField, Tooltip("Range danno ad area intorno al player")]
    float chargedAttackRadius = 5f;
    [SerializeField, Tooltip("Durata attacco ad area di attacco caricato")]
    float chargedAttackAreaDuration = 0.2f;


    [Header("Block")]
    
    [SerializeField, Tooltip("Quantità di danno parabile prima di rottura parata")]
    float maxStamina;
    [SerializeField, Tooltip("Danno parata perfetta")]
    float perfectBlockDamage;

    [Header("Unique Ability")]
    [SerializeField, Tooltip("Durata aggro")]
    float aggroDuration;
    [SerializeField, Tooltip("Durata buff difesa")]
    float defenceBuffDuration;
    [SerializeField, Tooltip("Moltiplicatore buff difesa")]
    float defenceMultiplier;
    [SerializeField, Tooltip("Moltiplicatore buff stamina")]
    float staminamultiplier;

    [Header("Bossfight Upgrade")]
    [SerializeField, Tooltip("Numero attacchi da parare perfettamente per ottenimento potenziamento bossfight")]
    int attacksToBlockForUpgrade = 10;
    [SerializeField, Tooltip("Cooldown attacco potenziamento bossfight")]
    float cooldownExtraAbility;
    [SerializeField, Tooltip("Durata stun attacco potenziamento boss fight")]
    float chargedAttackStunDuration = 5;
    [SerializeField, Tooltip("Moltiplicatore durata stun")]
    float stunDurationMultiplier;


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
    private bool canMove = true;
    private bool isBlocking;

    private int comboIndex = 0;
    private int comboMax = 2;
    private int perfectBlockCount;
    private float rangeAggro = math.INFINITY;
    private float currentStamina;
    private GenericBarScript staminaBar;
    private GameObject chargedAttackAreaObject = null;

    bool canCancelAttack;

    public override void Inizialize(CharacterData characterData, Character character)
    {
        base.Inizialize(characterData, character);
        currentStamina = maxStamina;
        currentHp = maxHp;

        staminaBar = GetComponentInChildren<GenericBarScript>();
        
        staminaBar.Setvalue(maxStamina);
        staminaBar.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            bossfightPowerUpUnlocked = true;
        }
        if(Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(new DamageData(10,null));
        }
       
    }


    #region Attack

    public override void Attack(Character parent, InputAction.CallbackContext context)
    {
        if (context.performed && isBlocking == false)
        {
            ActivateHyperArmor();
            pressed = true;
            canCancelAttack = false;
            Invoke(nameof(CheckAttackToDo), timeCheckAttackType);

        }

        if (context.canceled)
        {
            pressed = false;


            if (chargedAttackReady && !canCancelAttack)
            {
                animator.SetTrigger("ChargedAttackEnd");
                Debug.Log("Charged Attack executed");
               
            }

            else if (!chargedAttackReady && canCancelAttack)
            {
                animator.SetTrigger("Attack1");
                IncreaseComboIndex();

            }


            isAttacking = false;
            chargedAttackReady = false;
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
        SetCanMove(false,character.GetRigidBody());

        if (pressed && chargedAttack)
        {
            canCancelAttack = true;
            isAttacking = true;
            animator.SetTrigger("ChargedAttack");
            Debug.Log($"Started Charged Attack");
            StartCoroutine(StartChargedAttackTimer());

        }

        else if (!pressed)
        {
            canCancelAttack = false;

            if (comboIndex == 0 && !isAttacking)
            {
                isAttacking = true;
                animator.SetTrigger("Attack1");
                IncreaseComboIndex();
            }

            else if (doubleAttack && comboIndex == 1)
            {
                if (comboIndex != 2)
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
      
        yield return new WaitForSeconds(chargedAttackTimer);

        if (pressed)
        {         
            chargedAttackReady = true;
            canCancelAttack = false;
            
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
    private void DeactivateHyperArmor()
    {
        hyperArmorOn = false;
    }
    public void ResetAttackCombo()
    {
        if (comboIndex == 0 || comboIndex == 1)
        {
            comboIndex = 0;
            isAttacking = false;
            Debug.Log("Reset Variables");
            SetCanMove(true, character.GetRigidBody());
        }
        else if (comboIndex == 2)
        {
            IncreaseComboIndex();
        }

    }
    public void aaaaaaaaaaaaaaaaaaaaaa()
    {
        Debug.Log($"combo index:[{comboIndex}] can Double Attack[{doubleAttack}]");
    }
    public void ChargedAttackAreaDamage()
    {
        if(chargedAttackAreaObject == null)
        {
            chargedAttackAreaObject = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), transform);
            chargedAttackAreaObject.name = "ChargedAttackArea";
            chargedAttackAreaObject.GetComponent<Renderer>().enabled = false;
            chargedAttackAreaObject.GetComponent<SphereCollider>().radius = chargedAttackRadius;
            Damager dama = chargedAttackAreaObject.AddComponent<Damager>();
            dama.targetLayers = LayerMask.GetMask("Enemy");
            Invoke(nameof(DeactivateAreaDamage), chargedAttackAreaDuration);
        }
        else
        {
            chargedAttackAreaObject.SetActive(true);
            Invoke(nameof(DeactivateAreaDamage), chargedAttackAreaDuration);
        }

    }

    private void DeactivateAreaDamage()
    {
        chargedAttackAreaObject.SetActive(false);
    }

    #endregion

    #region Block

    public override void Defence(Character parent, InputAction.CallbackContext context)
    {
        if (context.performed && isAttacking == false)
        {
                   
            SetCanMove(false, character.GetRigidBody());           
            isBlocking = true;
            ShowStaminaBar(true);
            Debug.Log($"is blocking [{isBlocking}]");
        }

        else if (context.canceled && isBlocking == true)
        {
            SetCanMove(true, character.GetRigidBody());
            isBlocking= false;
            ShowStaminaBar(false);
            ResetStamina();
            Debug.Log($"is blocking [{isBlocking}]");
        }


        //se potenziamento 4 parata perfetta fa danno
    }

    private void ResetStamina()
    {
        currentStamina = maxStamina;
        staminaBar.ResetValue();
    }

    public void ShowStaminaBar(bool toShow)
    {
        staminaBar.gameObject.SetActive(toShow);       
    }
    #endregion

    #region onHit

    public override void TakeDamage(DamageData data)
    {
       if(hyperArmorOn == false)
        {
            //Hit Reaction
        }

       if(isBlocking)
        {
            staminaBar.DecreaseValue(data.damage);
            currentStamina -= data.damage;
            Debug.Log($"{currentStamina}");
            if(currentStamina <= 0)
            {
                SetCanMove(true, character.GetRigidBody());               
                isBlocking = false;
                ShowStaminaBar(false );
                Debug.Log("Parata Rotta");
                //Stun per 2 secondi dopo reset stamina
            }
        }
        else
        {
            currentHp -= data.damage;
            Debug.Log($"{currentHp}");
        }
    }

    
    #endregion

    #region UniqueAbility(Shout)

    public override void UseUniqueAbility(Character parent, InputAction.CallbackContext context)
    {
        if (context.performed)
        {

        }
        //attacco attiro aggro
    }

    #endregion

    #region ExtraAbility(BossAttack)

    public override void UseExtraAbility(Character parent, InputAction.CallbackContext context) //Tasto est
    {
        if (context.performed)
        {
            SetCanMove(false, character.GetRigidBody());

            if (bossfightPowerUpUnlocked && isAttacking == false)
            {
                animator.SetTrigger("UniqueAbility");

                float stunDamageDuration = maximizedStun ? (chargedAttackStunDuration * stunDurationMultiplier) : chargedAttackStunDuration;
                Debug.Log($"BossFight Upgrade Attack Executed, stun duration:[{stunDamageDuration}]");
            }
        }

    }

    #endregion

    #region Move
    
    public override void Move(Vector2 direction, Rigidbody rb)
    {
        if (canMove)
        {
            base.Move(direction, rb);
            
        }
        animator.SetBool("IsMoving", isMoving);

    }


    private void SetCanMove(bool move, Rigidbody rigidbody)
    {
        canMove = move;
        if (move == false)
        {
            rigidbody.velocity = Vector3.zero;
        }

    }
    #endregion


}
