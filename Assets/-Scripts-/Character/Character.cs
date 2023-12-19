using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour, IDamageable, IDamager, IInteracter
{
    [SerializeField] protected CharacterData characterData;
    protected CharacterClass characterClass;

    public CharacterData CharacterData => characterData;
    public CharacterClass CharacterClass => characterClass; 

    protected float MaxHp => characterClass.maxHp;
    protected float currentHp => characterClass.currentHp;
    protected Rigidbody rb;
    private bool canInteract;
    private IInteractable activeInteractable;



    //Lo uso per chimare tutte le funzioni iniziali
    protected void Awake()
    {
        InitialSetup();
    }

    //Tutto ci� che va fatto nello ad inizio
    protected virtual void InitialSetup()
    {
        rb = GetComponent<Rigidbody>();      
        characterData.Inizialize(this);
        canInteract = false;
    }

    protected virtual void Attack(InputAction.CallbackContext context) => characterClass.Attack(this, context);
    protected virtual void Defend(InputAction.CallbackContext context) => characterClass.Defence(this, context);
    public virtual void UseUniqueAbility(InputAction.CallbackContext context) => characterClass.UseUniqueAbility(this, context);
    public virtual void UseExtraAbility(InputAction.CallbackContext context) => characterClass.UseExtraAbility(this, context);
    protected virtual void Move(Vector2 direction) => characterClass.Move(direction, rb);

    protected void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(canInteract)
                InteractWith(activeInteractable);
        }
    }

    public void AddPowerUp(PowerUp powerUp) => characterClass.AddPowerUp(powerUp);
    public void RemovePowerUp(PowerUp powerUp) => characterClass.RemovePowerUp(powerUp);
    public List<PowerUp> GetPowerUpList() => characterClass.GetPowerUpList();

    public void UnlockUpgrade(AbilityUpgrade abilityUpgrade) => characterClass.UnlockUpgrade(abilityUpgrade);

    public void SetCharacterData(CharacterData newCharData)
    {
        characterClass.Disable(this);
        Destroy(characterClass.gameObject);
        characterData = newCharData;
        characterData.Inizialize(this);
    }

    public void SetCharacterClass(CharacterClass cClass) => characterClass = cClass;
    public Rigidbody GetRigidBody() => rb;

    public virtual void TakeDamage(DamageData data) => characterClass.TakeDamage(data);
    

    #region InteractionSystem
    public void InteractWith(IInteractable interactable)
    {
        activeInteractable.Interact(this);
    }

    public void EnableInteraction(IInteractable interactable)
    {
        activeInteractable = interactable;
        canInteract = true;
    }

    public void DisableInteraction(IInteractable interactable)
    {
        activeInteractable = interactable;
        canInteract = false;
    }

    //modifiche

    //public float GetDamage() => characterClass.GetDamage();

    public DamageData GetDamageData() => characterClass.GetDamageData();

    //fine modifiche


    #endregion
}
