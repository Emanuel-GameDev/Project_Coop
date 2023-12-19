using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : Character
{
    [SerializeField] protected CharacterData characterData;
    protected CharacterClass characterClass;

    public CharacterData CharacterData => characterData;
    public CharacterClass CharacterClass => characterClass; 
    
    protected float MaxHp => characterClass.maxHp;
    protected float currentHp => characterClass.currentHp;

    private Vector3 screenPosition;
    private Vector3 worldPosition;
    Plane plane = new Plane(Vector3.up, -1);

    Vector2 lookDir;
    Vector2 moveDir;

    public Vector2 MoveDirection => moveDir;
    protected override void InitialSetup()
    {
        base.InitialSetup();
        characterData.Inizialize(this);
    }

    private void Update()
    {
        Move(moveDir);
    }

    protected virtual void Attack(InputAction.CallbackContext context) => characterClass.Attack(this, context);
    protected virtual void Defend(InputAction.CallbackContext context) => characterClass.Defence(this, context);
    public virtual void UseUniqueAbility(InputAction.CallbackContext context) => characterClass.UseUniqueAbility(this, context);
    public virtual void UseExtraAbility(InputAction.CallbackContext context) => characterClass.UseExtraAbility(this, context);
    protected virtual void Move(Vector2 direction) => characterClass.Move(direction, rb);

    public override void AddPowerUp(PowerUp powerUp) => characterClass.AddPowerUp(powerUp);
    public override void RemovePowerUp(PowerUp powerUp) => characterClass.RemovePowerUp(powerUp);
    public override List<PowerUp> GetPowerUpList() => characterClass.GetPowerUpList();

    public void UnlockUpgrade(AbilityUpgrade abilityUpgrade) => characterClass.UnlockUpgrade(abilityUpgrade);

    public void SetCharacterData(CharacterData newCharData)
    {
        characterClass.Disable(this);
        Destroy(characterClass.gameObject);
        characterData = newCharData;
        characterData.Inizialize(this);
    }

    public void SetCharacterClass(CharacterClass cClass) => characterClass = cClass;
    public override void TakeDamage(DamageData data) => characterClass.TakeDamage(data);
    public override DamageData GetDamageData() => characterClass.GetDamageData();

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

    public void SwitchUp(InputAction.CallbackContext context)
    {
        if (context.performed)
            GameManager.Instance.coopManager.SwitchCharacter(this, 0);
    }

    public void SwitchRight(InputAction.CallbackContext context)
    {
        if(context.performed)
            GameManager.Instance.coopManager.SwitchCharacter(this, 1);
    }

    public void SwitchDown(InputAction.CallbackContext context)
    {
        if (context.performed)
            GameManager.Instance.coopManager.SwitchCharacter(this, 2);
    }
    public void SwitchLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
            GameManager.Instance.coopManager.SwitchCharacter(this, 3);
    }

    #endregion

    public void AttackInput(InputAction.CallbackContext context)
    {
        Attack(context);
    }

    public void UniqueAbilityInput(InputAction.CallbackContext context)
    {
        UseUniqueAbility(context);
    }

    public void ExtraAbilityInput(InputAction.CallbackContext context)
    {
        UseExtraAbility(context);
    }

    public void DefenseInput(InputAction.CallbackContext context)
    {
        Defend(context);
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

}
