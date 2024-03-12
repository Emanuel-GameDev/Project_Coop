using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : Character, InputReceiver
{
    protected CharacterClass characterClass;
    public CharacterClass CharacterClass => characterClass;

    public float MaxHp => characterClass.MaxHp;
    public float CurrentHp => characterClass.currentHp;
    public bool protectedByTank;

    private ePlayerCharacter currentCharacter;
    private PlayerInputHandler playerInputHandler;
    private Vector3 screenPosition;
    private Vector3 worldPosition;
    Plane plane = new Plane(Vector3.back,0);

    Vector2 lookDir;
    Vector2 moveDir;
    Vector2 lastNonZeroDirection;
    public Vector2 MoveDirection => moveDir;
    public Vector2 LastDirection => lastNonZeroDirection;
    protected override void InitialSetup()
    {
        base.InitialSetup();
        if (characterClass != null)
            InizializeClass(characterClass);
    }

    private void Update()
    {
        Move(moveDir);
    }

    #region CharacterClass Management
    public void InizializeClass(CharacterClass newCharClass)
    {
        newCharClass.Enable(this);
        SetCharacterClass(newCharClass);
        SetCharacter(newCharClass.Character);
    }
    public void SetCharacterClass(CharacterClass cClass) => characterClass = cClass;
    public void SwitchCharacterClass(CharacterClass newCharClass)
    {
        if (characterClass != null)
            characterClass.Disable(this);

        if (newCharClass != null)
            InizializeClass(newCharClass);
    }
    #endregion

    #region Redirect To Class
    protected virtual void Move(Vector2 direction) => characterClass.Move(direction, rb);
    public override void AddPowerUp(PowerUp powerUp) => characterClass.AddPowerUp(powerUp);
    public override void RemovePowerUp(PowerUp powerUp) => characterClass.RemovePowerUp(powerUp);
    public override List<PowerUp> GetPowerUpList() => characterClass.GetPowerUpList();
    public void UnlockUpgrade(AbilityUpgrade abilityUpgrade) => characterClass.UnlockUpgrade(abilityUpgrade);
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
            characterClass.TakeDamage(data);
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


    public override DamageData GetDamageData() => characterClass.GetDamageData();

    #endregion

    #region Interface Implementation

    public void SetCharacter(ePlayerCharacter character)
    {
        currentCharacter = character;
        if (playerInputHandler != null)
            playerInputHandler.SetCharacter(currentCharacter);
        if (characterClass == null)
            CharacterPoolManager.Instance.SwitchCharacter(this, character);
    }
    public ePlayerCharacter GetCharacter() => currentCharacter;

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
        if (characterClass != null)
            characterClass.Disable(this);

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
        string gamepad = context.control.device.displayName;

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
    public void AttackInput(InputAction.CallbackContext context)
    {
        characterClass.Attack(this, context);
    }
    public void DefenseInput(InputAction.CallbackContext context)
    {
        characterClass.Defence(this, context);
    }

    public void UniqueAbilityInput(InputAction.CallbackContext context)
    {
        characterClass.UseUniqueAbility(this, context);
    }

    public void ExtraAbilityInput(InputAction.CallbackContext context)
    {
        characterClass.UseExtraAbility(this, context);
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
