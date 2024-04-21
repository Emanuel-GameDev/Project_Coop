using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


//Unique = tasto nord,Q = Urlo
//BossUpgrade = tasto est,E = attacco bossFight
public class Tank : PlayerCharacter
{
    [Header("Attack")]
    [SerializeField, Tooltip("Durata tempo pressione tasto attacco prima per decidere se attacco caricato o no")]
    float timeCheckAttackType = 0.2f;
    [SerializeField, Tooltip("Tempo minimo attacco caricato per essere eseguito")]
    float chargedAttackTimer = 2.5f;
    [SerializeField, Tooltip("danno ad area intorno al player")]
    float chargedAttackDamage = 50f;
    [SerializeField, Tooltip("Range danno ad area intorno al player")]
    float chargedAttackRadius = 5f;
    [SerializeField, Tooltip("Sprite del segnale visivo se attacco caricato pronto")]
    GameObject chargedAttackSprite;

    [Header("Block")]

    [SerializeField, Tooltip("Quantit� di danno parabile prima di rottura parata")]
    float maxStamina;
    [SerializeField, Tooltip("timer tra una parata in altra")]
    float blockTimer;
    [SerializeField, Tooltip("Danno parata perfetta")]
    float perfectBlockDamage;
    [SerializeField, Tooltip("Angolo parata frontale al tank")]
    float blockAngle = 90;
    [SerializeField, Tooltip("finestra di tempo nella quale appena viene colpito pu� parare per fare parata perfetta")]
    float perfectBlockTimeWindow = 0.4f;

    public enum blockZone
    {
        none,
        nordEst,
        sudEst,
        nordOvest,
        sudOvest
    }
    private blockZone currentBlockZone;

    [Header("Unique Ability")]

    [SerializeField, Tooltip("cooldown abilit� unica")]
    float cooldownUniqueAbility;
    [SerializeField, Tooltip("Range aggro")]
    float aggroRange;
    [SerializeField, Tooltip("Durata aggro")]
    float aggroDuration;
    [SerializeField, Tooltip("Moltiplicatore buff difesa")]
    float HealthDamageReduction;
    [SerializeField, Tooltip("Moltiplicatore buff stamina")]
    float StaminaDamageReduction;

    [Header("Bossfight Upgrade")]

    [SerializeField, Tooltip("Numero attacchi da parare perfettamente per ottenimento potenziamento bossfight")]
    int attacksToBlockForUpgrade = 10;
    [SerializeField, Tooltip("Cooldown attacco potenziamento bossfight")]
    float cooldownExtraAbility;
    [SerializeField, Tooltip("Durata stun attacco potenziamento boss fight")]
    float chargedAttackStunDuration = 5;
    [SerializeField, Tooltip("aggiunta in secondi alla durata dello stun")]
    float additionalStunDuration = 5;


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
    private bool canCancelAttack;
    private bool canPerfectBlock;
    private bool uniqueAbilityReady = true;
    private bool statBoosted;
    private bool canProtectOther;
    private bool canBlock = true;
    

    private int comboIndex = 0;
    private int comboMax = 2;
    private int perfectBlockCount;

    private float currentStamina;
    private float blockAngleThreshold => (blockAngle - 180) / 180;
    private float staminaDamageReductionMulty;
    private float healthDamageReductionMulty;
    private float lastDirectionYValue;


    private GenericBarScript staminaBar;
    private GameObject chargedAttackAreaObject = null;
    private PerfectTimingHandler perfectBlockHandler;
    private ProtectPlayers triggerProtectPlayer;
    private PivotTriggerProtected pivotTriggerProtected;
   


    public override void Inizialize()
    {
        base.Inizialize();
        currentStamina = maxStamina;
        currentHp = MaxHp;

        staminaBar = GetComponentInChildren<GenericBarScript>();

        staminaBar.Setvalue(maxStamina);
        staminaBar.gameObject.SetActive(false);
        triggerProtectPlayer = GetComponentInChildren<ProtectPlayers>();
        pivotTriggerProtected = GetComponentInChildren<PivotTriggerProtected>();

        perfectBlockHandler = GetComponentInChildren<PerfectTimingHandler>(true);

        staminaDamageReductionMulty = (1 - (StaminaDamageReduction / 100));
        healthDamageReductionMulty = (1 - HealthDamageReduction / 100);

    }
   
    #region Attack

    public override void AttackInput(InputAction.CallbackContext context)
    {
        //Cercar soluzione forse
        if (stunned) return;

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
                chargedAttackSprite.gameObject.SetActive(false);

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
        SetCanMove(false, rb);

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
            chargedAttackSprite.SetActive(true);
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
            SetCanMove(true, rb);

        }
        else if (comboIndex == 2)
        {
            IncreaseComboIndex();
        }

    }

    public void ChargedAttackAreaDamage()
    {
        RaycastHit[] hitted = Physics.SphereCastAll(transform.position, chargedAttackRadius, Vector3.up, chargedAttackRadius);
        if (hitted != null)
        {
            foreach (RaycastHit r in hitted)
            {
                if (Utility.IsInLayerMask(r.transform.gameObject, LayerMask.GetMask("Enemy")))
                {
                    IDamageable hittedDama = r.transform.gameObject.GetComponent<IDamageable>();
                    hittedDama.TakeDamage(new DamageData(chargedAttackDamage, this, null));
                    Debug.Log(r.transform.gameObject.name + " colpito con " + chargedAttackDamage + " damage di attacco ad area");

                }
            }
        }
    }


    #endregion

    #region Block
    private bool AttackInBlockAngle(DamageData data)
    {
        Character dealerMB = null;
        if (data.dealer is Character)
            dealerMB = (Character)data.dealer;
        
        Vector2 dealerDirection = dealerMB.gameObject.transform.position - transform.position;
        
        float angle = 0;

        if (currentBlockZone is blockZone.nordEst)
        {
            angle = Vector2.Angle(dealerDirection, new Vector2(1, 1));
        }
        else if (currentBlockZone is blockZone.sudEst)
        {
            angle = Vector2.Angle(dealerDirection, new Vector2(1, -1));
        }
        else if (currentBlockZone is blockZone.nordOvest)
        {
            angle = Vector2.Angle(dealerDirection, new Vector2(-1, 1));
        }
        else if (currentBlockZone is blockZone.sudOvest)
        {
            angle = Vector2.Angle(dealerDirection, new Vector2(-1, -1));
        }

        

        return Mathf.Abs(angle) <= blockAngle / 2;
    }
    Vector3 RotateVectorAroundPivot(Vector3 vector, Vector3 pivot, float angle)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        vector -= pivot;
        vector = rotation * vector;
        vector += pivot;
        return vector;
    }
    public override void DefenseInput(InputAction.CallbackContext context)
    {
        if (context.performed && isAttacking == false && canCancelAttack == false && canBlock)
        {
            SetCanMove(false, rb);
            if(isBlocking != true)
            {
                isBlocking = true;
                currentBlockZone = SetBlockZone(lastNonZeroDirection.y);
                animator.SetTrigger("StartBlock");

            }

            ShowStaminaBar(true);
            
            //Rotate pivor Trigger per proteggere player
            pivotTriggerProtected.enabled = true;
            pivotTriggerProtected.Rotate(lastNonZeroDirection);
            

            //Trigger che setta ai player protectedByTank a true;
            triggerProtectPlayer.SetPlayersProtected(true);


            //eliminare
            mostraGizmoRangeParata = true;

        }

        else if (context.canceled && isBlocking == true)
        {
            SetCanMove(true, rb);
            if (isBlocking != false)
            {
                
                isBlocking = false;
                currentBlockZone = blockZone.none;
                StartCoroutine(nameof(ToggleBlock));               
                animator.SetTrigger("ToggleBlock");
            }
            isBlocking = false;
            ShowStaminaBar(false);
            

            //Da mettere reset in animazione alzata scudo
            ResetStamina();

            //Trigger che setta ai player protectedByTank a true;
            triggerProtectPlayer.SetPlayersProtected(false);


            //eliminare
            mostraGizmoRangeParata = false;
        }


        //se potenziamento 4 parata perfetta fa danno
    }
    public blockZone SetBlockZone(float lastYValue)
    {
        
        if ((lastYValue > 0 && pivot.transform.localScale.x <0))
        {
            return blockZone.nordEst;
        }

        else if (lastYValue > 0 && pivot.transform.localScale.x > 0)
        {
            return blockZone.nordOvest;
        }

        else if (lastYValue < 0 && pivot.transform.localScale.x < 0)
        {
            return blockZone.sudEst;
        }

        else if (lastYValue < 0 && pivot.transform.localScale.x > 0)
        {
            return blockZone.sudOvest;
        }

        else
            return blockZone.none;

    }
    private IEnumerator ToggleBlock()
    {
        canBlock = false;
       yield return new WaitForSeconds(blockTimer);
        canBlock = true;

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
    private IEnumerator StartPerfectBlockTimer(DamageData data)
    {
        canPerfectBlock = true;
        perfectBlockHandler.gameObject.SetActive(true);
        Character dealerMB = null;
        if (data.dealer is Character)
            dealerMB = (Character)data.dealer;


        yield return new WaitForSeconds(perfectBlockTimeWindow);

        //parata perfetta
        if (isBlocking && AttackInBlockAngle(data))
        {
            perfectBlockCount++;
            if (perfectBlockCount >= attacksToBlockForUpgrade)
            {
                //Sblocco parry che fa danno
                UnlockUpgrade(AbilityUpgrade.Ability4);
            }

            Debug.Log("parata perfetta eseguita, rimanenti per potenziamento boss = " + (attacksToBlockForUpgrade - perfectBlockCount));
            PubSub.Instance.Notify(EMessageType.perfectGuardExecuted, this);


            data.dealer.OnParryNotify(this);           
            if (damageOnParry && dealerMB != null)
            {
                dealerMB.TakeDamage(new DamageData(perfectBlockDamage, this));
            }
        }

        else
        {
            base.TakeDamage(data);
            Debug.Log($" Tank  hp : {currentHp}  stamina: {currentStamina}");
        }
        canPerfectBlock = false;
        perfectBlockHandler.gameObject.SetActive(false);


    }

    #endregion

    #region onHit

    public override void TakeDamage(DamageData data)
    {

        if (hyperArmorOn == false && !isBlocking)
        {
            //Hit Reaction
            animator.SetTrigger("Hit");

        }
        if (isBlocking && AttackInBlockAngle(data))
        {
            data.blockedByTank = true;

            if (statBoosted)
            {
               
                staminaBar.DecreaseValue(data.staminaDamage * staminaDamageReductionMulty);
                currentStamina -= (data.staminaDamage * staminaDamageReductionMulty);

            }
            else
            {
                staminaBar.DecreaseValue(data.staminaDamage);
                currentStamina -= data.staminaDamage;

            }

            PubSub.Instance.Notify(EMessageType.guardExecuted, this);


            Debug.Log($"current stamina : {currentStamina}");
            if (currentStamina <= 0)
            {
                SetCanMove(true, rb);
                isBlocking = false;
                ShowStaminaBar(false);
                Debug.Log("Parata Rotta");
                animator.SetTrigger("ShieldBroken");
                //CONTROLLARE
                stunned = true;
               
            }
        }

        else
        {
            StartCoroutine(StartPerfectBlockTimer(data));
        }
    }

    public void Unstun()
    {
        stunned = false;
       
    }

    #endregion

    #region UniqueAbility(Shout)

    public override void UniqueAbilityInput(InputAction.CallbackContext context)
    {

        if (context.performed && uniqueAbilityReady)
        {
            //Da eliminare
            mostraGizmoAbilitaUnica = true;
            Invoke(nameof(SetGizmoAbilitaUnica), 1.2f);

            uniqueAbilityReady = false;
            Invoke(nameof(StartCooldownUniqueAbility), cooldownUniqueAbility);
            animator.SetTrigger("UniqueAbility");
            Debug.Log("UniqueAbility Used");
        }

    }


    public void PerformUniqueAbility()
    {
        RaycastHit2D[] hitted = Physics2D.CircleCastAll(transform.position, aggroRange, Vector2.up, aggroRange);
        
        if (hitted != null)
        {
           
            foreach (RaycastHit2D r in hitted)
            {
                
                if (Utility.IsInLayerMask(r.transform.gameObject, LayerMask.GetMask("Enemy")))
                {
                    IDamageable hittedDama = r.transform.gameObject.GetComponent<IDamageable>();

                    AggroCondition aggroCondition = Utility.InstantiateCondition<AggroCondition>();
                    aggroCondition.SetVariable(this, aggroDuration);
                    

                    hittedDama.TakeDamage(new DamageData(0, this, aggroCondition));
                   
                }
            }
        }
        //Incremento statistiche difesa e stamina
        statBoosted = true;
        damageReceivedMultiplier = healthDamageReductionMulty;
        Invoke(nameof(SetStatToNormal), aggroDuration);


    }
    private void StartCooldownUniqueAbility()
    {
        uniqueAbilityReady = true;
        Debug.Log("abilita unica pronta");
    }
    private void SetStatToNormal()
    {
        statBoosted = false;
        damageReceivedMultiplier = 1;
    }


    #endregion

    #region ExtraAbility(BossAttack)

    public override void ExtraAbilityInput(InputAction.CallbackContext context) //Tasto est
    {
        if (context.performed && !isAttacking && !isBlocking)
        {
            
            SetCanMove(false, rb);

            if (bossfightPowerUpUnlocked && isAttacking == false)
            {
                damager.SetCondition(Utility.InstantiateCondition<StunCondition>(), true);
                animator.SetTrigger("extraAbility");

                float stunDamageDuration = maximizedStun ? (chargedAttackStunDuration + additionalStunDuration) : chargedAttackStunDuration;
                Debug.Log($"BossFight Upgrade Attack Executed, stun duration:[{stunDamageDuration}]");
            }
        }

    }


    #endregion

    #region Move

    public override void Move(Vector2 direction)
    {
        if (canMove)
        {
            base.Move(direction);

            if (direction.y != 0)
                lastDirectionYValue = direction.y;


        }
        animator.SetBool("IsMoving", isMoving);

    }


    private void SetCanMove(bool move, Rigidbody2D rigidbody)
    {
        canMove = move;
        if (move == false)
        {
            rigidbody.velocity = Vector3.zero;
        }

    }
    #endregion

    //Eliminare

    private bool mostraGizmoAbilitaUnica;
    private bool mostraGizmoRangeParata;
    Vector3 dealerPosition;





    public void OnDrawGizmos()
    {
        if (dealerPosition != Vector3.zero)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, dealerPosition);

        }
        if (mostraGizmoAbilitaUnica)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, aggroRange);
        }

        if (mostraGizmoRangeParata)
        {
            if(currentBlockZone is blockZone.nordEst)
            {
                Vector2 temp = transform.position + new Vector3(1,1,0);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, temp);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, RotateVectorAroundPivot(temp, transform.position, (-1 * (blockAngle / 2))));
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, RotateVectorAroundPivot(temp, transform.position, (1 * (blockAngle / 2))));

            }else if (currentBlockZone is blockZone.sudEst)
            {
                Vector2 temp = transform.position + new Vector3(1, -1, 0);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, temp);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, RotateVectorAroundPivot(temp, transform.position, (-1 * (blockAngle / 2))));
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, RotateVectorAroundPivot(temp, transform.position, (1 * (blockAngle / 2))));

            }
            else if (currentBlockZone is blockZone.nordOvest)
            {
                Vector2 temp = transform.position + new Vector3(-1, 1, 0);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, temp);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, RotateVectorAroundPivot(temp, transform.position, (-1 * (blockAngle / 2))));
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, RotateVectorAroundPivot(temp, transform.position, (1 * (blockAngle / 2))));

            }
            else if (currentBlockZone is blockZone.sudOvest)
            {
                Vector2 temp = transform.position + new Vector3(-1, -1, 0);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, temp);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, RotateVectorAroundPivot(temp, transform.position, (-1 * (blockAngle / 2))));
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, RotateVectorAroundPivot(temp, transform.position, (1 * (blockAngle / 2))));

            }


        }

    }

    private void SetGizmoAbilitaUnica()
    {
        mostraGizmoAbilitaUnica = false;
    }


    void DrawArc(Vector3 center, float radius, float angle, int segments)
    {
        Gizmos.color = Color.blue;

    }


    Vector3 GetPointOnCircle(float radius, float angle)
    {
        float radianAngle = Mathf.Deg2Rad * angle;
        float x = transform.position.x + radius * Mathf.Cos(radianAngle);
        float y = transform.position.y + radius * Mathf.Sin(radianAngle);

        return new Vector3(x, y, transform.position.z);
    }

}
