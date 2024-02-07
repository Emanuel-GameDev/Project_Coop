using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : Character, InputReceiver
{
    [SerializeField]
    protected CharacterClass characterClass;
    public CharacterClass CharacterClass => characterClass; 
    
    public float MaxHp => characterClass.MaxHp;
    public float CurrentHp => characterClass.currentHp;
    public bool protectedByTank;

    private ePlayerCharacter currentCharacter;
    private PlayerInputHandler playerInputHandler;
    private Vector3 screenPosition;
    private Vector3 worldPosition;
    Plane plane = new Plane(Vector3.up, -1);

    Vector2 lookDir;
    Vector2 moveDir;

    public Vector2 MoveDirection => moveDir;
    protected override void InitialSetup()
    {
        base.InitialSetup();
        if(characterClass != null)
            InizializeClass(characterClass);
    }

    private void Update()
    {
        Move(moveDir);
    }

    public void InizializeClass(CharacterClass newCharClass)
    {
        CharacterClass cClass = Instantiate(newCharClass.gameObject, gameObject.transform).GetComponent<CharacterClass>();
        cClass.Inizialize(this);
        SetCharacterClass(cClass);
        SetCharacter(cClass.Character);
    }


    protected virtual void Move(Vector2 direction) => characterClass.Move(direction, rb);

    public override void AddPowerUp(PowerUp powerUp) => characterClass.AddPowerUp(powerUp);
    public override void RemovePowerUp(PowerUp powerUp) => characterClass.RemovePowerUp(powerUp);
    public override List<PowerUp> GetPowerUpList() => characterClass.GetPowerUpList();

    public void UnlockUpgrade(AbilityUpgrade abilityUpgrade) => characterClass.UnlockUpgrade(abilityUpgrade);

    public void SwitchCharacterClass(CharacterClass newCharClass)
    {
        if(characterClass != null)
        {
            characterClass.Disable(this);
            Destroy(characterClass.gameObject);
        }
        InizializeClass(newCharClass);
    }

    public void SetCharacterClass(CharacterClass cClass) => characterClass = cClass;
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
    }
       
    public override DamageData GetDamageData() => characterClass.GetDamageData();

    #region InterfaceImpletation

    public void SetCharacter(ePlayerCharacter character)
    {
        currentCharacter = character;
        if(playerInputHandler != null)
            playerInputHandler.SetCharacter(currentCharacter);
        if (characterClass == null)
            SwitchCharacterClass(CoopManager.Instance.GetCharacterClass(character));
    }
    public ePlayerCharacter GetCharacter() => currentCharacter;

    public void SetInputHandler(PlayerInputHandler inputHandler)
    {
        playerInputHandler = inputHandler;
    }
    public PlayerInputHandler GetInputHandler()
    {
        return playerInputHandler;
    }

    public void Dismiss()
    {
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

    public Vector3 ReadLook()
    {
        var gamepad = Gamepad.current;

        if (gamepad != null)
        {
            //perndo la look dal player.input utilizzando il gamepad
            return new Vector3(lookDir.x, 0, lookDir.y).normalized;
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

            Debug.Log(worldPosition);
            return new Vector3(worldPosition.x, 0, worldPosition.z);
        }

    }

    #endregion

    #region SwitchCharacters

    public void SwitchUpInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            if(CoopManager.Instance.CanSwitchCharacter(CoopManager.Instance.SwitchPlayerUp))
            {
                SwitchCharacterClass(CoopManager.Instance.SwitchPlayerUp);
            }
    }

    public void SwitchRightInput(InputAction.CallbackContext context)
    {
        if(context.performed)
            if (CoopManager.Instance.CanSwitchCharacter(CoopManager.Instance.SwitchPlayerRight))
            {
                SwitchCharacterClass(CoopManager.Instance.SwitchPlayerRight);
            }
    }

    public void SwitchDownInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            if (CoopManager.Instance.CanSwitchCharacter(CoopManager.Instance.SwitchPlayerDown))
            {
                SwitchCharacterClass(CoopManager.Instance.SwitchPlayerDown);
            }
    }
    public void SwitchLeftInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            if (CoopManager.Instance.CanSwitchCharacter(CoopManager.Instance.SwitchPlayerLeft))
            {
                SwitchCharacterClass(CoopManager.Instance.SwitchPlayerLeft);
            }
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
    }

    public void InteractInput(InputAction.CallbackContext context)
    {
        Interact(context);
    }


    #endregion

    #region UnusedInput

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
    #endregion


    #endregion


}
