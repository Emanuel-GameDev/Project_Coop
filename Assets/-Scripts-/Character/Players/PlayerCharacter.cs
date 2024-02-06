using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : Character
{
    [SerializeField]
    protected CharacterClass characterClass;
    public CharacterClass CharacterClass => characterClass; 
    
    public float MaxHp => characterClass.MaxHp;
    public float CurrentHp => characterClass.currentHp;
    public bool protectedByTank;
    

    private Vector3 screenPosition;
    private Vector3 worldPosition;
    Plane plane = new Plane(Vector3.up, -1);

    Vector2 lookDir;
    Vector2 moveDir;

    public Vector2 MoveDirection => moveDir;
    protected override void InitialSetup()
    {
        base.InitialSetup();
        InizializeClass();
    }

    private void Update()
    {
        Move(moveDir);
    }

    public void InizializeClass()
    {
        CharacterClass cClass = Instantiate(characterClass.gameObject, gameObject.transform).GetComponent<CharacterClass>();
        cClass.Inizialize(this);
        SetCharacterClass(cClass);
    }


    protected virtual void Move(Vector2 direction) => characterClass.Move(direction, rb);

    public override void AddPowerUp(PowerUp powerUp) => characterClass.AddPowerUp(powerUp);
    public override void RemovePowerUp(PowerUp powerUp) => characterClass.RemovePowerUp(powerUp);
    public override List<PowerUp> GetPowerUpList() => characterClass.GetPowerUpList();

    public void UnlockUpgrade(AbilityUpgrade abilityUpgrade) => characterClass.UnlockUpgrade(abilityUpgrade);

    public void SwitchCharacterClass(CharacterClass newCharClass)
    {
        characterClass.Disable(this);
        Destroy(characterClass.gameObject);
        characterClass = newCharClass;
        InizializeClass();
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

    public void SwitchUp(InputAction.CallbackContext context)
    {
        if (context.performed)
            if(CoopManager.Instance.CanSwitchCharacter(CoopManager.Instance.SwitchPlayerUp, this))
            {
                SwitchCharacterClass(CoopManager.Instance.SwitchPlayerUp);
            }
    }

    public void SwitchRight(InputAction.CallbackContext context)
    {
        if(context.performed)
            if (CoopManager.Instance.CanSwitchCharacter(CoopManager.Instance.SwitchPlayerRight, this))
            {
                SwitchCharacterClass(CoopManager.Instance.SwitchPlayerRight);
            }
    }

    public void SwitchDown(InputAction.CallbackContext context)
    {
        if (context.performed)
            if (CoopManager.Instance.CanSwitchCharacter(CoopManager.Instance.SwitchPlayerDown, this))
            {
                SwitchCharacterClass(CoopManager.Instance.SwitchPlayerDown);
            }
    }
    public void SwitchLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
            if (CoopManager.Instance.CanSwitchCharacter(CoopManager.Instance.SwitchPlayerLeft, this))
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

    #endregion
}
