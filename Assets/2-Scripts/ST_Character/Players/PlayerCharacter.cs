using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class PlayerCharacter : Character
{
    #region Variables

    #region Stats

    [SerializeField, Tooltip("Identifica il personaggio.")]
    protected ePlayerCharacter character;
    [SerializeField, Tooltip("La salute massima del personaggio.")]
    protected float maxHp;
    [SerializeField, Tooltip("Il danno inflitto dal personaggio.")]
    protected float damage;
    [SerializeField, Tooltip("La velocit� di attacco del personaggio.")]
    protected float attackSpeed;
    [SerializeField, Tooltip("La velocit� di movimento del personaggio.")]
    protected float moveSpeed;
    [SerializeField, Tooltip("Il tempo di attesa per l'abilit� unica.")]
    protected float uniqueAbilityCooldown;
    [SerializeField, Tooltip("L'incremento del tempo di attesa dell'abilit� unica dopo ogni uso.")]
    protected float uniqueAbilityCooldownIncreaseAtUse;
    [SerializeField, Tooltip("Tempo che deve trascorrere prima di poter cambiare di nuovo personaggio")]
    protected float switchCharacterCooldown;
    [SerializeField, Tooltip("Tempo di invulnerabilità dopo essere staty colpiti")]
    protected float invulnerabilityTime;
    [SerializeField, Tooltip("Reference per l'interactable del ress")]
    protected GameObject ressInteracter;
    [SerializeField]
    protected float onHitPushTimer = 0.2f;
    [SerializeField]
    protected float onHitPushForce = 1;

    protected float lastestCharacterSwitch;
    protected float currentHp;
    protected float lastHitTime;

    protected float uniqueAbilityRemainingCooldown = 0;
    protected float uniqueAbilityUses = 0;
    protected bool uniqueAbilityMustIncreaseUses = false;

    #endregion

    #region References

    protected Pivot pivot;
    protected Damager damager;
    protected Animator animator;
    protected ExtraData extraData;
    protected PowerUpData powerUpData;
    protected UnityAction unityAction;
    protected SpriteRenderer spriteRenderer;
    protected Dictionary<AbilityUpgrade, bool> upgradeStatus;
    [HideInInspector] public PlayerCharacterController characterController;

    #endregion

    #region Misc

    protected Vector3 screenPosition;
    protected Vector3 worldPosition;
    protected Plane plane = new Plane(Vector3.back, 0);

    protected Vector2 lookDir;
    protected Vector2 moveDir;
    protected Vector2 lastNonZeroDirection;

    protected bool isMoving;
    protected bool isInBossfight = false;
    protected bool bossfightPowerUpUnlocked = false;

    protected bool isRightInputRecently => rightInputTimer > 0;
    [SerializeField] protected float recentlyInputTimer = 3f;
    protected float rightInputTimer = 0;

    protected BossFightHandler bossFightHandler = null;
    public bool protectedByTank; //DA RIVEDERE 

    [Header("Crosshair distance multiplier")]

    [SerializeField] float crosshairDistance = 6f;

    #endregion

    #region Propriety
    public ePlayerCharacter Character => character;

    public virtual float MaxHp => Mathf.RoundToInt(maxHp * powerUpData.MaxHpIncrease);
    public virtual float CurrentHp
    {
        get { return Mathf.RoundToInt(currentHp); }
        set
        {
            currentHp = value;

            if (currentHp < 0)
                currentHp = 0;

            if (currentHp > MaxHp)
                currentHp = MaxHp;
        }
    }
    public virtual float Damage => damage * powerUpData.DamageIncrease;
    public virtual float MoveSpeed => moveSpeed * powerUpData.MoveSpeedIncrease;
    public virtual float AttackSpeed => attackSpeed * powerUpData.AttackSpeedIncrease;
    public virtual float UniqueAbilityCooldown => (uniqueAbilityCooldown + (uniqueAbilityCooldownIncreaseAtUse * uniqueAbilityUses)) / powerUpData.UniqueAbilityCooldownDecrease;
    public float DamageReceivedMultiplier => damageReceivedMultiplier;
    public float SwitchCharacterCooldown => switchCharacterCooldown;
    public virtual ExtraData ExtraData => extraData;
    public virtual PowerUpData PowerUpData => powerUpData;

    public Vector2 MoveDirection => moveDir;
    public Vector2 LastDirection => lastNonZeroDirection;

    protected bool CanSwitch => !isDead && Time.time - lastestCharacterSwitch > switchCharacterCooldown;
    protected bool IsHitInvulnerable => Time.time - lastHitTime < invulnerabilityTime;
    public bool UniqueAbilityAvaiable => UniqueAbilityRemainingCooldown <= 0;
    public float UniqueAbilityRemainingCooldown => uniqueAbilityRemainingCooldown;

    #endregion

    #region Animation
    private static string Y = "Y";
    #endregion

    #endregion

    #region Inizialization & Setup

    protected override void InitialSetup()
    {
        base.InitialSetup();
        Inizialize();
    }

    public virtual void Inizialize()
    {
        powerUpData = new PowerUpData();
        extraData = new ExtraData();
        upgradeStatus = new();
        CurrentHp = MaxHp;
        ressInteracter.SetActive(false);
        foreach (AbilityUpgrade au in Enum.GetValues(typeof(AbilityUpgrade)))
        {
            upgradeStatus.Add(au, false);
        }
        animator = GetComponent<Animator>();
        bossfightPowerUpUnlocked = false;
        uniqueAbilityUses = 0;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        pivot = GetComponentInChildren<Pivot>();
        lastNonZeroDirection = Vector2.down;
        damager = GetComponentInChildren<Damager>(true);
        if (damager != null)
        {
            damager.SetSource(this);
        }
        SetIsInBossfight(false, null);
        lastestCharacterSwitch = Time.time;
    }

    public virtual void SetIsInBossfight(bool value, BossFightHandler bossFightHandler)
    {
        isInBossfight = value;
        this.bossFightHandler = bossFightHandler;
    }
    public void SetMaxHP(float value) => maxHp = value;
    public void SetCurrentHP(float value) => CurrentHp = value;

    public PlayerInputHandler GetInputHandler()
    {
        if (characterController != null)
            return characterController.GetInputHandler();
        else
            return null;
    }

    #endregion

    #region Movement
    protected virtual void Update()
    {
        Move(moveDir);
        UniqueAbilityCooldownTimer();
    }


    public virtual void Move(Vector2 direction)
    {
        if (!direction.normalized.Equals(direction))
            direction = direction.normalized;

        rb.velocity = direction * MoveSpeed;
        isMoving = rb.velocity.magnitude > 0.2f;

        SetSpriteDirection(lastNonZeroDirection);
    }

    public void SetSpriteDirection(Vector2 direction)
    {
        if (direction.y != 0)
            animator.SetFloat(Y, direction.y);
        Vector3 scale = pivot.gameObject.transform.localScale;
        if ((direction.x > 0.5 && scale.x > 0) || (direction.x < -0.5 && scale.x < 0))
            scale.x *= -1;

        pivot.gameObject.transform.localScale = scale;
    }

    public void ResetSpriteDirection()
    {
        lastNonZeroDirection = new Vector2(0, 0);
    }

    #endregion

    #region Upgrades

    public virtual bool GetAbilityStatus(AbilityUpgrade abilityUpgrade)
    {
        return upgradeStatus[abilityUpgrade];
    }

    public virtual void UnlockUpgrade(AbilityUpgrade abilityUpgrade)
    {
        if (upgradeStatus[abilityUpgrade] == false)
            upgradeStatus[abilityUpgrade] = true;
    }

    public virtual void LockUpgrade(AbilityUpgrade abilityUpgrade)
    {
        if (upgradeStatus[abilityUpgrade] == true)
            upgradeStatus[abilityUpgrade] = false;
    }
    #endregion

    #region PowerUp
    public override void AddPowerUp(PowerUp powerUp) => powerUpData.Add(powerUp);
    public override void RemovePowerUp(PowerUp powerUp) => powerUpData.Remove(powerUp);
    public override List<PowerUp> GetPowerUpList() => powerUpData.PowerUps;
    #endregion

    #region Damage
    public override void TakeDamage(DamageData data)
    {
        if (!isDead && !IsHitInvulnerable)
        {
            if (data.condition != null)
                AddToConditions(data.condition);

            CurrentHp -= data.damage * DamageReceivedMultiplier;
            damager.RemoveCondition();
            Debug.Log($"Dealer: {data.dealer}, Damage: {data.damage}, Condition: {data.condition}");

            //shader
            SetHitMaterialColor(_OnHitColor);

            OnHit?.Invoke();
            if (data.dealer != null)
                StartCoroutine(PushCharacter(data.dealer.dealerTransform.transform.position, onHitPushForce, onHitPushTimer));

            if (protectedByTank && data.blockedByTank)
            {
                Debug.Log("Protetto da tank");
            }
            else
            {
                PubSub.Instance.Notify(EMessageType.characterDamaged, this);
            }

            if (CurrentHp <= 0)
            {

                Die();
                onDeath?.Invoke();
            }

            lastHitTime = Time.time;
        }
    }

    public virtual void Die()
    {
        characterController.GetInputHandler().PlayerInput.actions.Disable();
        characterController.GetInputHandler().PlayerInput.actions.FindAction("Menu").Enable();
        characterController.GetInputHandler().PlayerInput.actions.FindAction("Option").Enable();
        ressInteracter.gameObject.SetActive(true);
        isDead = true;

        foreach (Collider2D coll in colliders)
        {
            coll.enabled = false;
        }

        PlayerCharacterPoolManager.Instance.PlayerIsDead();
        TargetManager.Instance.ChangeTarget(this);
    }

    public virtual void Ress()
    {
        characterController.GetInputHandler().PlayerInput.actions.Enable();
        isDead = false;

        foreach (Collider2D coll in colliders)
        {
            coll.enabled = true;
        }
        TakeHeal(new DamageData(MathF.Floor(MaxHp / 2), this));
        ressInteracter.gameObject.SetActive(false);
        PlayerCharacterPoolManager.Instance.PlayerIsRessed();
    }


    public virtual void TakeHeal(DamageData data)
    {
        if (!isDead)
        {
            if(_healParticlesObject != null)
                Instantiate(_healParticlesObject, transform.position,Quaternion.identity,transform);

            if (data.condition != null)
                RemoveFromConditions(data.condition);

            CurrentHp += data.damage;
            PubSub.Instance.Notify(EMessageType.characterDamaged, this);

            damager.RemoveCondition();
            Debug.Log($"Healer: {data.dealer}, Heal: {data.damage}, Condition Removed: {data.condition}");

        }

    }

    public override DamageData GetDamageData()
    {
        DamageData data = new DamageData(Damage, this);
        return data;
    }

    #endregion

    #region UniqueAbilityCooldown

    protected void UniqueAbilityUsed()
    {
        uniqueAbilityRemainingCooldown = UniqueAbilityCooldown;
        uniqueAbilityMustIncreaseUses = true;
    }

    protected void UniqueAbilityCooldownTimer()
    {
        if (!UniqueAbilityAvaiable)
        {
            uniqueAbilityRemainingCooldown -= Time.deltaTime;
            Debug.Log(Character + " Unique Ability Cooldown: " + uniqueAbilityRemainingCooldown);
        }
        else if (uniqueAbilityMustIncreaseUses)
        {
            uniqueAbilityUses++;
            uniqueAbilityMustIncreaseUses = false;
        }
            
    }
    #endregion

    #region Input

    #region Look
    public void LookInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            lookDir = context.ReadValue<Vector2>();
            rightInputTimer = recentlyInputTimer;
        }

    }

    public void LookInputMouse(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 temp = context.ReadValue<Vector2>();
            lookDir = ((Camera.main.ScreenToWorldPoint(temp) - transform.position)).normalized;
            rightInputTimer = recentlyInputTimer;
        }


    }

    public void DialogueInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PubSub.Instance.Notify(EMessageType.dialogueInput, this);
            Debug.Log("input");
        }
    }

    public Vector2 ReadLookdirCrosshair(Vector2 shootSource)
    {
        if (lookDir.magnitude <= 1)
        {
            lookDir.Normalize();
            lookDir *= crosshairDistance;
        }




        return (lookDir + shootSource);
    }

    public Vector2 ReadLook(InputAction.CallbackContext context)
    {
        //string gamepad = context.control.device.displayName;

        //string gamepad = characterController.GetInputHandler().PlayerInput.currentControlScheme;


        //if (gamepad.Contains("Gamepad") || gamepad.Contains("Controller") || gamepad.Contains("Joystick"))
        //{
        //    //perndo la look dal player.input utilizzando il gamepad

        //    Debug.Log(lookDir);
        //    return new Vector2(lookDir.x, lookDir.y).normalized;


        //}
        //else
        //{
        //    //prendo la look con un raycast dal mouse
        //    screenPosition = Input.mousePosition;

        //    Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        //    if (plane.Raycast(ray, out float distance))
        //    {
        //        worldPosition = ray.GetPoint(distance);

        //        worldPosition = (worldPosition - transform.position).normalized;
        //    }

        //    //Debug.Log(worldPosition);
        //    return new Vector2(worldPosition.x, worldPosition.y);
        //}

        Debug.Log(lookDir);
        return new Vector2(lookDir.x, lookDir.y).normalized;

    }

    #endregion

    #region SwitchCharacters

    public void SetSwitchCooldown()
    {
        lastestCharacterSwitch = Time.time;
    }

    public void SwitchUpInput(InputAction.CallbackContext context)
    {
        if (context.performed && CanSwitch)
            PlayerCharacterPoolManager.Instance.SwitchCharacter(this, ePlayerCharacter.Brutus);
    }

    public void SwitchRightInput(InputAction.CallbackContext context)
    {
        if (context.performed && CanSwitch)
            PlayerCharacterPoolManager.Instance.SwitchCharacter(this, ePlayerCharacter.Kaina);
    }

    public void SwitchDownInput(InputAction.CallbackContext context)
    {
        if (context.performed && CanSwitch)
            PlayerCharacterPoolManager.Instance.SwitchCharacter(this, ePlayerCharacter.Cassius);
    }

    public void SwitchLeftInput(InputAction.CallbackContext context)
    {
        if (context.performed && CanSwitch)
            PlayerCharacterPoolManager.Instance.SwitchCharacter(this, ePlayerCharacter.Jude);
    }

    #endregion

    #region MainActions
    public virtual void AttackInput(InputAction.CallbackContext context)
    {

    }
    public virtual void DefenseInput(InputAction.CallbackContext context)
    {

    }

    public virtual void UniqueAbilityInput(InputAction.CallbackContext context)
    {
       
    }

    public virtual void ExtraAbilityInput(InputAction.CallbackContext context)
    {

    }

    public virtual void LockTargetInput(InputAction.CallbackContext context)
    {
        
    }

    public virtual void ChangeTartgetInput(InputAction.CallbackContext context)
    {

    }

    public void MoveInput(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
        if (moveDir != Vector2.zero)
            lastNonZeroDirection = moveDir;

    }

    public void InteractInput(InputAction.CallbackContext context)
    {
        Interact(context);
    }

    public void CancelInteractInput(InputAction.CallbackContext context)
    {
        CancelInteraction(context);
    }

    #endregion

    #endregion

    #region SaveGame

    public CharacterSaveData GetSaveData()
    {
        CharacterSaveData saveData = new CharacterSaveData();
        saveData.characterName = character;
        saveData.powerUps = powerUpData.PowerUps;
        saveData.extraData = extraData;
        foreach (AbilityUpgrade au in Enum.GetValues(typeof(AbilityUpgrade)))
        {
            if (upgradeStatus[au])
                saveData.unlockedAbility.Add(au);
        }

        return saveData;
    }

    public void LoadSaveData(CharacterSaveData saveData)
    {
        if (saveData == null || saveData.characterName != character)
            return;
        Debug.Log("load");
        foreach (PowerUp pu in saveData.powerUps)
        {
            powerUpData.Add(pu);
        }

        extraData = saveData.extraData;
        foreach (AbilityUpgrade au in saveData.unlockedAbility)
        {
            upgradeStatus[au] = true;
        }
    }

    #endregion

    #region Rumble

    public void RumblePad(string rumbleName)
    {
        RumbleData dataFound = GetRumbleData(rumbleName);

        if (dataFound == null) return;

        characterController.GetInputHandler().RumblePulse(dataFound);
    }

    public void StopRumblePad(string rumbleName)
    {
        RumbleData dataFound = GetRumbleData(rumbleName);

        if (dataFound == null) return;

        characterController.GetInputHandler().RumbleStop(dataFound);
    }

    #endregion

    public override void DisableOtherActions()
    {
        base.DisableOtherActions();

        characterController.GetInputHandler().PlayerInput.actions.Disable();
        characterController.GetInputHandler().PlayerInput.actions.FindAction("Menu").Enable();
        characterController.GetInputHandler().PlayerInput.actions.FindAction("Option").Enable();
        characterController.GetInputHandler().PlayerInput.actions.FindAction("CancelInteract").Enable();
    }

    public override void EnableAllActions()
    {
        base.EnableAllActions();
        characterController.GetInputHandler().PlayerInput.actions.Enable();
    }

    public virtual void ResetAllAnimatorAndTriggers()
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);
            }
            animator.Play("Idle");
        }

        moveDir = Vector2.zero;
        isMoving = false;
        rb.velocity = Vector2.zero;
    }

    public void Dismiss()
    {
        PlayerCharacterPoolManager.Instance.ReturnCharacter(this);
    }

    public void CancelSwitchCooldown()
    {
        lastestCharacterSwitch = SwitchCharacterCooldown;
    }
}
