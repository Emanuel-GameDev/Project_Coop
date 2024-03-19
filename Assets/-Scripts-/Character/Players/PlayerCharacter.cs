using Cinemachine.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerCharacter : Character, InputReceiver
{
    public float MaxHp;
    public float CurrentHp => currentHp;
    public bool protectedByTank;

    private PlayerInputHandler playerInputHandler;
    private Vector3 screenPosition;
    private Vector3 worldPosition;
    Plane plane = new Plane(Vector3.back,0);

    protected Vector2 lookDir;
    protected Vector2 moveDir;
    protected Vector2 lastNonZeroDirection;
    public Vector2 MoveDirection => moveDir;
    public Vector2 LastDirection => lastNonZeroDirection;

    #region FromCharacterClass

    protected Pivot pivot;
    protected Damager damager;
    protected Animator animator;
    protected ExtraData extraData;
    protected PowerUpData powerUpData;
    protected UnityAction unityAction;
    protected SpriteRenderer spriteRenderer;
    protected Dictionary<AbilityUpgrade, bool> upgradeStatus;

    protected bool isMoving;
    protected bool isInBossfight;
    protected bool bossfightPowerUpUnlocked;

    [SerializeField, Tooltip("Identifica il personaggio.")]
    protected ePlayerCharacter character;
    [SerializeField, Tooltip("La salute massima del personaggio.")]
    protected float maxHp;
    [SerializeField, Tooltip("Il danno inflitto dal personaggio.")]
    protected float damage;
    [SerializeField, Tooltip("La velocità di attacco del personaggio.")]
    protected float attackSpeed;
    [SerializeField, Tooltip("La velocità di movimento del personaggio.")]
    protected float moveSpeed;
    [SerializeField, Tooltip("Il tempo di attesa per l'abilità unica.")]
    protected float uniqueAbilityCooldown;
    [SerializeField, Tooltip("L'incremento del tempo di attesa dell'abilità unica dopo ogni uso.")]
    protected float uniqueAbilityCooldownIncreaseAtUse;

    protected float currentHp;
    protected float uniqueAbilityUses;

    public virtual float Damage => damage * powerUpData.damageIncrease;
    public virtual float MoveSpeed => moveSpeed * powerUpData.moveSpeedIncrease;
    public virtual float AttackSpeed => attackSpeed * powerUpData.attackSpeedIncrease;
    public virtual float UniqueAbilityCooldown => (uniqueAbilityCooldown + (uniqueAbilityCooldownIncreaseAtUse * uniqueAbilityUses)) * powerUpData.UniqueAbilityCooldownDecrease;
    public float DamageReceivedMultiplier => damageReceivedMultiplier;

    #region Animation Variable
    private static string Y = "Y";
    #endregion

    public virtual void Inizialize()
    {
        powerUpData = new PowerUpData();
        extraData = new ExtraData();
        upgradeStatus = new();
        currentHp = MaxHp;
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
        SetIsInBossfight(false);
    }

    public virtual void SetIsInBossfight(bool value) => isInBossfight = value;


    public virtual void Move(Vector2 direction)
    {
        if (!direction.normalized.Equals(direction))
            direction = direction.normalized;

        rb.velocity = direction * MoveSpeed;
        //rb.velocity =new Vector2(direction.x,direction.y*0.7f)* MoveSpeed;
        isMoving = rb.velocity.magnitude > 0.2f;

        if (direction != Vector2.zero)
            lastNonZeroDirection = direction;

        SetSpriteDirection(lastNonZeroDirection);
    }

    protected void SetSpriteDirection(Vector2 direction)
    {
        if (direction.y != 0)
            animator.SetFloat(Y, direction.y);
        Vector3 scale = pivot.gameObject.transform.localScale;
        if ((direction.x > 0.5 && scale.x > 0) || (direction.x < -0.5 && scale.x < 0))
            scale.x *= -1;

        pivot.gameObject.transform.localScale = scale;
    }

    #region Upgrades
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

    #endregion

    protected override void InitialSetup()
    {
        base.InitialSetup();
            Inizialize();
    }

    private void Update()
    {
        Move(moveDir);
    }

    #region Redirect To Class
    public override void AddPowerUp(PowerUp powerUp) => powerUpData.Add(powerUp);
    public override void RemovePowerUp(PowerUp powerUp) => powerUpData.Remove(powerUp);
    public override List<PowerUp> GetPowerUpList() => powerUpData.powerUps;
    #endregion

    #region Damage
    public override void TakeDamage(DamageData data)
    {
        if (protectedByTank && data.blockedByTank)
        {
            Debug.Log("Protetto da tank");
        }
        else
        {
            PubSub.Instance.Notify(EMessageType.characterDamaged, this);
        }
        //Ricontrollare
        if (inLove && GetComponentInChildren<LoveCondition>().started && data.condition is LoveCondition)
        {
            foreach (PlayerCharacter p in GameManager.Instance.coopManager.ActivePlayers)
            {
                if (this != p)
                {
                    if (p.inLove)
                    {
                        //Mettere calcolo danno
                    }
                }
            }
        }
    }


    public override DamageData GetDamageData()
    {
        DamageData data = new DamageData(Damage, this);
        return data;
    }
    #endregion

    #region Interface Implementation

    public void SetCharacter(ePlayerCharacter character)
    {
       
    }
    public ePlayerCharacter GetCharacter() => character;

    public virtual GameObject GetReceiverObject()
    {
        return gameObject;
    }

    public void SetInputHandler(PlayerInputHandler inputHandler)
    {
        playerInputHandler = inputHandler;
        if (playerInputHandler != null)
        {
            if (playerInputHandler.currentCharacter != ePlayerCharacter.EmptyCharacter)
                SetCharacter(playerInputHandler.currentCharacter);
            else
                CharacterPoolManager.Instance.GetFreeRandomCharacter(this);
        }
    }
    public PlayerInputHandler GetInputHandler()
    {
        return playerInputHandler;
    }

    public void Dismiss()
    {
        //TODO
        CameraManager.Instance.RemoveTarget(this.transform);
        //characterClass.SaveClassData();
        Destroy(gameObject);
    }

    #endregion

    #region Input

    #region Look
    public void LookInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            lookDir = context.ReadValue<Vector2>();
    }

    public void DialogueInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PubSub.Instance.Notify(EMessageType.dialogueInput, this);
            Debug.Log("input");
        }
    }

    public Vector2 ReadLook(InputAction.CallbackContext context)
    {
        string gamepad = playerInputHandler.PlayerInput.currentControlScheme;

        if (gamepad.Contains("Gamepad") || gamepad.Contains("Controller") || gamepad.Contains("Joystick"))
        {
            //perndo la look dal player.input utilizzando il gamepad
            return new Vector2(lookDir.x, lookDir.y).normalized;
        }
        else
        {
            //prendo la look con un raycast dal mouse
            screenPosition = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(screenPosition);

            if (plane.Raycast(ray, out float distance))
            {
                worldPosition = ray.GetPoint(distance);

                worldPosition = (worldPosition - transform.position).normalized;
            }

            //Debug.Log(worldPosition);
            return new Vector2(worldPosition.x, worldPosition.y);
        }

    }

    #endregion

    #region SwitchCharacters

    public void SwitchUpInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            CharacterPoolManager.Instance.SwitchCharacter(this, ePlayerCharacter.Brutus);

    }

    public void SwitchRightInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            CharacterPoolManager.Instance.SwitchCharacter(this, ePlayerCharacter.Caina);
    }

    public void SwitchDownInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            CharacterPoolManager.Instance.SwitchCharacter(this, ePlayerCharacter.Cassius);
    }

    public void SwitchLeftInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            CharacterPoolManager.Instance.SwitchCharacter(this, ePlayerCharacter.Jude);
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

    public  virtual void ExtraAbilityInput(InputAction.CallbackContext context)
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


    #endregion

    #region UnusedInput

    public virtual void Navigate(InputAction.CallbackContext context)
    {
    }

    public virtual void Submit(InputAction.CallbackContext context)
    {
    }

    public virtual void RandomSelection(InputAction.CallbackContext context)
    {
    }

    public virtual void Cancel(InputAction.CallbackContext context)
    {
    }

    public virtual void Point(InputAction.CallbackContext context)
    {
    }

    public virtual void ScrollWheel(InputAction.CallbackContext context)
    {
    }

    public void JoinInput(InputAction.CallbackContext context)
    {

    }

    public void MenuInput(InputAction.CallbackContext context)
    {

    }

    public void OptionInput(InputAction.CallbackContext context)
    {

    }

    public void MoveMinigameInput(InputAction.CallbackContext context)
    {

    }

    public virtual void ButtonEast(InputAction.CallbackContext context)
    {

    }

    public virtual void ButtonNorth(InputAction.CallbackContext context)
    {

    }

    public virtual void ButtonWeast(InputAction.CallbackContext context)
    {

    }

    public virtual void ButtonSouth(InputAction.CallbackContext context)
    {

    }
    #endregion


    #endregion

}

public enum AbilityUpgrade
{
    Ability1,
    Ability2,
    Ability3,
    Ability4,
    Ability5
}
