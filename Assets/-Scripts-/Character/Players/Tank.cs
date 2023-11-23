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
    [SerializeField, Tooltip("Prefab barra stamina da applicare sopra player")]
    GameObject staminaBar;
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
    private bool canMove = true;
    private bool isBlocking;

    private int comboIndex = 0;
    private int comboMax = 2;
    private int perfectBlockCount;
    private float rangeAggro = math.INFINITY;
    private float currentStamina;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            bossfightPowerUpUnlocked = true;
        }
    }


    #region Attack

    public override void Attack(Character parent, InputAction.CallbackContext context)
    {
        if (context.performed && isBlocking == false)
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
                animator.SetTrigger("ChargedAttackEnd");
                Debug.Log("Charged Attack executed");
                return;
            }

            else if (!chargedAttackReady && canCancelChargedAttck)
            {
                isAttacking = true;
                animator.SetTrigger("Attack1");
                IncreaseComboIndex();
                return;
            }

            isAttacking = false;


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
        SetCanMove(false, GetComponentInParent<Rigidbody>());

        if (pressed && chargedAttack)
        {
            isAttacking = true;
            animator.SetTrigger("ChargedAttack");
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
    public void ResetAttackCombo()
    {
        if (comboIndex == 0 || comboIndex == 1)
        {
            comboIndex = 0;
            isAttacking = false;
            Debug.Log("Reset Variables");
            SetCanMove(true, GetComponentInParent<Rigidbody>());
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

    #endregion

    #region Block

    public override void Defence(Character parent, InputAction.CallbackContext context)
    {
        if (context.performed && isAttacking == false)
        {
            isBlocking = true;
            ShowStaminaBar(true);
            Debug.Log($"is blocking [{isBlocking}]");
        }

        else if (context.canceled && isBlocking == true)
        {
            isBlocking= false;
            ShowStaminaBar(false);
            Debug.Log($"is blocking [{isBlocking}]");
        }


        //se potenziamento 4 parata perfetta fa danno
    }
    public void ShowStaminaBar(bool toShow)
    {
        staminaBar.SetActive(toShow);
    }
    #endregion

    #region onHit
    public override void TakeDamage(float damage, IDamager dealer)
    {       
        if (hyperArmorOn == false)
        {
            DoHitReacion();
        }
        if (isBlocking)
        {
           
        }

    }

    private void DoHitReacion()
    {

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
            SetCanMove(false, GetComponentInParent<Rigidbody>());

            if (bossfightPowerUpUnlocked && isAttacking == false)
            {
                animator.SetTrigger("UniqueAbility");

                float stunDamageDuration = maximizedStun ? (chargedAttackStunDuration * stunDurationMultyplier) : chargedAttackStunDuration;
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
