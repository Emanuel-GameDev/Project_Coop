using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


//Unique = tasto nord,Q = Urlo
//BossUpgrade = tasto est,E = attacco bossFight

public class Tank : PlayerCharacter, IPerfectTimeReceiver
{
    [Header("Attack")]
    [SerializeField, Tooltip("Durata tempo pressione tasto attacco prima per decidere se attacco caricato o no")]
    float timeCheckAttackType = 0.2f;
    [SerializeField, Tooltip("Tempo minimo attacco caricato per essere eseguito")]
    float chargedAttackTimer = 2.5f;
    [SerializeField, Tooltip("danno diretto di attacco caricato")]
    float chargedAttackDirectDamage = 50f;
    [SerializeField, Tooltip("Range danno ad area intorno al player")]
    float chargedAttackRadius = 5f;
    [SerializeField, Tooltip("danno ad area intorno al player")]
    float chargedAttackAreaDamage = 50f;
    [SerializeField, Tooltip("prefab onda urto")]
    GameObject crashPrefab;
    [SerializeField, Tooltip("punto origine onda urto da impostare in animazione")]
    Transform crashSpawnPosition;

    [Header("ChargeAttack")]
    [SerializeField, Tooltip("timer tra una carica e un altra per non spammare")]
    float chargeCooldownTimer = 5f;
    [SerializeField, Tooltip("Velocità movimento in carica")]
    float chargeSpeed = 5f;
    [SerializeField, Tooltip("durata carica se non interrotta")]
    float chargeDuration = 5f;

    [Header("Block")]
    [SerializeField, Tooltip("Quantit� di danno parabile prima di rottura parata")]
    float maxStamina;
    [SerializeField, Tooltip("timer tra una parata in altra")]
    float blockTimer;
    [SerializeField, Tooltip("Danno parata perfetta")]
    float perfectBlockDamage;
    [SerializeField, Tooltip("Angolo parata frontale al tank")]
    float blockAngle = 90;
    [SerializeField, Tooltip("dempo per anulare notifica parta perfetta se non colpito ma avviso inviato")]
    float perfectBlockTimeWindow = 0.4f;

    [SerializeField, Tooltip("velocità minima parata")]
    float blockMoveSpeed = 0.1f;


   [HideInInspector] public enum blockZone
    {
        none,
        nordEst,
        sudEst,
        nordOvest,
        sudOvest
    }
    private blockZone currentBlockZone;

    [Header("Unique Ability")]

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
    [SerializeField, Tooltip("aggiunta in secondi alla durata dello stun")]
    float stunDurationOnAggro = 5;

    [Header("VFX")]
    [SerializeField] GameObject shieldVFX;
    [SerializeField] GameObject chargedAttackVFX;
    private Coroutine ShieldCoroutine;
    ParticleSystem.EmissionModule emissionModule;

    [Header("Shield Health Color", order = 1)]
    [ColorUsage(true, true)]
    [SerializeField] Color shieldVFXGoodColor;
    [ColorUsage(true, true)]
    [SerializeField] Color shieldVFXDamagedColor;
    [ColorUsage(true, true)]
    [SerializeField] Color shieldVFXCriticalColor;
    [ColorUsage(true, true)]
    [SerializeField] Color shieldVFXParryColor;
    private Color shieldVFXBaseColor;

    [SerializeField]
    private UnityEvent onBlock;
    [SerializeField]
    private UnityEvent onPerfectBlock;


    //private bool doubleAttack => upgradeStatus[AbilityUpgrade.Ability1];
    private bool chargeUnlocked => upgradeStatus[AbilityUpgrade.Ability1];
    private bool maximizedStunUnlocked => upgradeStatus[AbilityUpgrade.Ability2];
    private bool implacableAttackUnlocked => upgradeStatus[AbilityUpgrade.Ability3];
    private bool damageOnParryUnlocked => upgradeStatus[AbilityUpgrade.Ability4];
    private bool chargedAttackUnlocked => upgradeStatus[AbilityUpgrade.Ability5];


    private PerfectTimingHandler perfectTimingHandler;
    private bool perfectTimingEnabled;
    private bool hyperArmorOn = false;

    private bool isAttacking = false;
    private bool bossfightUpgradeUnlocked = false;
    private bool canPressInput;
    private bool chargedAttackReady = false;
    private bool canMove = true;
    private bool isBlocking = false;
    private bool isPerfectBlock = false;

    private bool canPerfectBlock = false;
    //private bool UniqueAbilityAvaiable = true;
    private bool statBoosted = false;
    private bool canProtectOther = false;
   
    private bool isChargingAttack = false;
    private bool canBlock = true;
    private bool canAttack = true;
    private bool canCharge = true;
    private bool inAttackAnimation = false;
    private bool inCharge = false;


    private int perfectBlockCount = 0;

    private float currentStamina;
    private float blockAngleThreshold => (blockAngle - 180) / 180;
    private float staminaDamageReductionMulty;
    private float healthDamageReductionMulty;
    private float moveSpeedCopy;



    private GenericBarScript staminaBar;
    private GameObject chargedAttackAreaObject = null;
    private PerfectTimingHandler perfectBlockHandler;
    private ProtectPlayers triggerProtectPlayer;
    private PivotTriggerProtected pivotTriggerProtected;
    private IDamager perfectBlockIDamager;






    public override void Inizialize()
    {
        base.Inizialize();

        moveSpeedCopy = moveSpeed;

        currentStamina = maxStamina;
        currentHp = MaxHp;

        staminaBar = GetComponentInChildren<GenericBarScript>();
        staminaBar.SetMaxValue(maxStamina);

        staminaBar.gameObject.SetActive(false);
        triggerProtectPlayer = GetComponentInChildren<ProtectPlayers>();
        pivotTriggerProtected = GetComponentInChildren<PivotTriggerProtected>();

        perfectBlockHandler = GetComponentInChildren<PerfectTimingHandler>(true);

        staminaDamageReductionMulty = (1 - (StaminaDamageReduction / 100));
        healthDamageReductionMulty = (1 - HealthDamageReduction / 100);

        //Debug.Log(powerUpData.UniqueAbilityCooldownDecrease);

        emissionModule = _walkDustParticles.emission;

        shieldVFXBaseColor = shieldVFX.GetComponentInChildren<MeshRenderer>().material.GetColor("_MainColor");

       //characterController.GetInputHandler().PlayerInput.currentActionMap.FindAction("ExtraAbility").
        

       
    }

    #region Attack

    public override void AttackInput(InputAction.CallbackContext context)
    {
       
        if (stunned) return;

        if (context.performed && canAttack)
        {
            //Carica
            if (chargeUnlocked && canCharge && isBlocking)
            {
                StartCoroutine(ChargeCoroutine());
            }

            //Attacco
            else if (!isBlocking && !inCharge && !inAttackAnimation)
            {
             
                Attack();

            }


        }

        if (context.canceled && inCharge)
        {
            StopCharge();
        }
    }

    #region charge
    private void StopCharge()
    {
        StopCoroutine(ChargeCoroutine());
        inCharge = false;
        canBlock = true;
        animator.SetBool("InCharge", inCharge);
        moveSpeed = moveSpeedCopy;
        StartCoroutine(StartChargeCooldown());
    }
    IEnumerator ChargeCoroutine()
    {
        isBlocking = false;      
        shieldVFX.gameObject.SetActive(false);
        canBlock = false;
        canCharge = false;
        animator.SetTrigger("ToggleBlock");
        inCharge = true;
        animator.SetBool("InCharge", inCharge);
        moveSpeed = chargeSpeed;
        base.Move(lastNonZeroDirection);

        yield return new WaitForSeconds(chargeDuration);

        StopCharge();




    }
    IEnumerator StartChargeCooldown()
    {
        yield return new WaitForSeconds(chargeCooldownTimer);
        canCharge = true;
    }
    #endregion
 
    public void ResetVariables()
    {

        SetHyperArmor(false);
        isBlocking = false;
        canBlock = true;
        canAttack = true;
        shieldVFX.gameObject.SetActive(false);
        chargedAttackVFX.gameObject.SetActive(false);
        isAttacking = false;
        
        chargedAttackReady = false;
        isChargingAttack = false;
        inAttackAnimation = false;
        SetCanMove(true, rb);

        Utility.DebugTrace("ResetAttack");
    }
    public void Attack()
    {
        isAttacking = true;

        inAttackAnimation = true;

        SetHyperArmor(true);
        SetCanMove(false, rb);

        animator.SetTrigger("Attack");
    }
    public void SetHyperArmor(bool value)
    {
        if (implacableAttackUnlocked)
        {
            hyperArmorOn = value;
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
    public override void DefenseInput(InputAction.CallbackContext context)
    {
        if (context.performed && isAttacking == false && canBlock)
        {
            OnDefenceAbility?.Invoke();
            ResetAllAnimatorAndTriggers();
            //SetCanMove(false, rb);

            if (isBlocking != true)
            {
                isBlocking = true;
                //vfx
                shieldVFX.gameObject.SetActive(true);
                SetShieldColorHealth();

                if (perfectTimingEnabled)
                {
                    perfectBlockCount++;



                    if (perfectBlockCount >= attacksToBlockForUpgrade)
                    {
                        //Sblocco parry che fa danno
                        UnlockUpgrade(AbilityUpgrade.Ability4);
                    }

                    Debug.Log("parata perfetta eseguita, rimanenti per potenziamento boss = " + (attacksToBlockForUpgrade - perfectBlockCount));
                    PlayPerfectParrySound();

                    PubSub.Instance.Notify(EMessageType.perfectGuardExecuted, this);


                    perfectBlockIDamager.OnParryNotify(this);

                    if (damageOnParryUnlocked && perfectBlockIDamager != null && perfectBlockIDamager is IDamageable)
                    {
                        ((IDamageable)perfectBlockIDamager).TakeDamage(new DamageData(perfectBlockDamage, this));
                    }
                }


                currentBlockZone = SetBlockZone(lastNonZeroDirection.y);
                animator.SetTrigger("StartBlock");

            }

            ShowStaminaBar(true);

            //Rotate pivor Trigger per proteggere player
            pivotTriggerProtected.enabled = true;
            pivotTriggerProtected.Rotate(lastNonZeroDirection);
            //Trigger che setta ai player protectedByTank a true;
            triggerProtectPlayer.SetPlayersProtected(true);

        }

        else if (context.canceled && isBlocking == true)
        {
            ResetAllAnimatorAndTriggers();
            SetCanMove(true, rb);

            currentBlockZone = blockZone.none;
            StartCoroutine(nameof(ToggleBlock));
            animator.SetTrigger("ToggleBlock");

            //vfx
            shieldVFX.gameObject.SetActive(false);


            ShowStaminaBar(false);


            //Da mettere reset in animazione alzata scudo
            ResetStamina();
            //Trigger che setta ai player protectedByTank a true;
            triggerProtectPlayer.SetPlayersProtected(false);


            isBlocking = false;


        }

    }


    public blockZone SetBlockZone(float lastYValue)
    {

        if ((lastYValue > 0 && pivot.transform.localScale.x < 0))
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
    private void SetShieldColorHealth()
    {
        Material shieldMaterial = shieldVFX.GetComponentInChildren<MeshRenderer>().material;

        float healthLevel = currentStamina / maxStamina;

        if (healthLevel > 0.5f)
        {
            //good healt
            shieldMaterial.SetColor("_HealthColor", shieldVFXGoodColor);

        }
        else
        {
            if (healthLevel > 0.25f)
            {
                //damaged health
                shieldMaterial.SetColor("_HealthColor", shieldVFXDamagedColor);
            }
            else
            {
                //critical health
                shieldMaterial.SetColor("_HealthColor", shieldVFXCriticalColor);
            }
        }
    }
    private IEnumerator PerfectBlockVFX()
    {
        float elapsedTime = 0;

        Material shieldMaterial = shieldVFX.GetComponentInChildren<MeshRenderer>().material;

        shieldVFX.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        shieldMaterial.SetColor("_MainColor", shieldVFXParryColor);

        while (elapsedTime < 1)
        {
            shieldVFX.transform.localScale = Vector3.Lerp(shieldVFX.transform.localScale, Vector3.one, (elapsedTime / 1));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        shieldVFX.transform.localScale = Vector3.one;
        shieldMaterial.SetColor("_MainColor", shieldVFXBaseColor);

    }
    private IEnumerator BlockVFX()
    {
        float elapsedTime = 0;

        Material shieldMaterial = shieldVFX.GetComponentInChildren<MeshRenderer>().material;
        shieldMaterial.SetColor("_MainColor", shieldVFXBaseColor);

        shieldVFX.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        while (elapsedTime < 1)
        {
            shieldVFX.transform.localScale = Vector3.Lerp(shieldVFX.transform.localScale, Vector3.one, (elapsedTime / 1));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        shieldVFX.transform.localScale = Vector3.one;
    }


    #endregion

    #region onHit

    public void SetPerfectTimingHandler(PerfectTimingHandler handler) => perfectTimingHandler = handler;

    public void PerfectTimeStarted(IDamager damager)
    {
        if (!isBlocking)
        {
            perfectTimingHandler.ActivateAlert();
            perfectBlockIDamager = damager;
            perfectTimingEnabled = true;

        }

        StartCoroutine(DisablePerfectTimeAfter(perfectBlockTimeWindow));
    }

    public void PerfectTimeEnded()
    {
        perfectTimingHandler.DeactivateAlert();
        perfectTimingEnabled = false;
        isPerfectBlock = false;
        perfectBlockIDamager = null;
        Utility.DebugTrace("Perfect Time Ended");
    }

    protected IEnumerator DisablePerfectTimeAfter(float time)
    {
        yield return new WaitForSeconds(time);
        if (perfectTimingEnabled)
            PerfectTimeEnded();
    }

    public override void Die()
    {
        base.Die();
        animator.SetTrigger("Death");
    }

    public override void Ress()
    {
        base.Ress();
        animator.SetTrigger("Ress");
    }
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

            if (perfectTimingEnabled)
            {
                if (ShieldCoroutine != null)
                {
                    StopCoroutine(ShieldCoroutine);

                }
                onPerfectBlock?.Invoke();
                ShieldCoroutine = StartCoroutine(PerfectBlockVFX());
            }
            else
            {
                if (ShieldCoroutine != null)
                {
                    StopCoroutine(ShieldCoroutine);

                }
                onBlock?.Invoke();
                ShieldCoroutine = StartCoroutine(BlockVFX());

            }


            //vfx
            SetShieldColorHealth();

            Debug.Log($"current stamina : {currentStamina}");
            if (currentStamina <= 0)
            {
                SetCanMove(true, rb);
                isBlocking = false;
                ShowStaminaBar(false);
                shieldVFX.SetActive(false);
                Debug.Log("Parata Rotta");
                animator.SetTrigger("ShieldBroken");
                //CONTROLLARE
                stunned = true;

            }
        }
        else
        {
            base.TakeDamage(data);
            if (inCharge)
            {
                StopCharge();
            }
        }
        if (perfectTimingEnabled)
        {
            PerfectTimeEnded();
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
        if (context.performed && UniqueAbilityAvaiable && !inAttackAnimation && !inCharge && !isChargingAttack)
        {
            chargedAttackVFX.gameObject.SetActive(true);
            SetCanMove(false, rb);         
            base.UniqueAbilityInput(context);
            //Invoke(nameof(StartCooldownUniqueAbility), UniqueAbilityCooldown);
            UniqueAbilityUsed();
            animator.SetTrigger("UniqueAbility");
            Debug.Log("UniqueAbility Used");
        }

    }


    public void PerformUniqueAbility()
    {
        chargedAttackVFX.gameObject.SetActive(false);
        SetCanMove(true, rb);

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

                    if(r.transform.gameObject.GetComponent<BossCharacter>() && maximizedStunUnlocked)
                    {
                        StunCondition stunCondition = Utility.InstantiateCondition<StunCondition>();
                        stunCondition.SetVariable(stunDurationOnAggro);
                        hittedDama.TakeDamage(new DamageData(0, this, stunCondition));
                        hittedDama.TakeDamage(new DamageData(0, this, aggroCondition));
                    }
                    else
                    {
                        hittedDama.TakeDamage(new DamageData(0, this, aggroCondition));
                    }
                   

                }
            }
        }

        //Incremento statistiche difesa e stamina
        statBoosted = true;
        damageReceivedMultiplier = healthDamageReductionMulty;
        Invoke(nameof(SetStatToNormal), aggroDuration);

        PubSub.Instance.Notify(EMessageType.uniqueAbilityActivated, this);
    }
 
    private void SetStatToNormal()
    {
        statBoosted = false;
        damageReceivedMultiplier = 1;
    }

    #endregion

    #region ExtraAbility(ChargedAttack)

    #region chargedAttack
   
    public void EndChargedAttack()
    {
       animator.SetTrigger("ChargedAttackEnd");
        chargedAttackReady = false;
        chargedAttackVFX.gameObject.SetActive(false);
        canAttack = true;
        canBlock = true;


    }
    //Rivedere e mettere onda urto
    public void ChargedAttackAreaDamage()
    {      
        GameObject instantiatedWave = Instantiate(crashPrefab, crashSpawnPosition.position, Quaternion.identity);
        
        instantiatedWave.GetComponentInChildren<CrashWave>().SetVariables(chargedAttackAreaDamage, 0, this);
        
    }
   
    #endregion
    public override void ExtraAbilityInput(InputAction.CallbackContext context) //Tasto est
    {

        if (context.started && !inAttackAnimation && !inCharge)
        {
            canBlock = false;
            canAttack = false;
            isChargingAttack = true;
            chargedAttackReady = false;
            animator.SetTrigger("ChargedAttack");
        }
        if (context.performed)
        {      
                chargedAttackReady = true;          
                chargedAttackVFX.gameObject.SetActive(true);
            Debug.LogWarning("PAlle");
            
        }
        
            
        if (context.canceled && !inAttackAnimation)
        {
            isChargingAttack = false;
            if (chargedAttackReady)
            {
                SetCanMove(false, rb);
                EndChargedAttack();
                
            }
            else
            {
                chargedAttackReady = false;
                chargedAttackVFX.gameObject.SetActive(false);
                Attack();
            }
        }
        

    }


    #endregion

    #region Move

    public override void Move(Vector2 direction)
    {
        if (inCharge)
        {
            base.Move(lastNonZeroDirection);

        }
        else
        {
            if (isChargingAttack)
            {
                moveSpeed = blockMoveSpeed;
                base.Move(direction);
            }

            else if (!isBlocking)
            {
                if (canMove)
                {
                    moveSpeed = moveSpeedCopy;
                    base.Move(direction);

                    emissionModule.enabled = isMoving;
                }

                animator.SetBool("IsMoving", isMoving);
            }
            else if(isBlocking)
            {
                moveSpeed = blockMoveSpeed;
                base.Move(direction);
                SetBlockZone(lastNonZeroDirection.y);

            }
            
        }

    }


    private void SetCanMove(bool move, Rigidbody2D rigidbody)
    {
        canMove = move;
        if (move == false)
        {
            rigidbody.velocity = Vector3.zero;
            emissionModule.enabled = false;
        }

    }
    #endregion


    public override void ResetAllAnimatorAndTriggers()
    {
        base.ResetAllAnimatorAndTriggers();
        ResetVariables();
        ResetVariables();
    }
    private void PlayPerfectParrySound()
    {
        if (soundsDatabase.specialEffectsSounds.Count > 0)
        {

            if (soundsDatabase.specialEffectsSounds.Count >= perfectBlockCount)
            {
                AudioManager.Instance.PlayAudioClip(soundsDatabase.specialEffectsSounds[perfectBlockCount - 1]);
            }
            else
            {
                AudioManager.Instance.PlayAudioClip(soundsDatabase.specialEffectsSounds[soundsDatabase.specialEffectsSounds.Count - 1]);
            }
        }
    }
}
