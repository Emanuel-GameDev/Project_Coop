using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour, IDamageable, IDamager
{
    [SerializeField] protected CharacterData characterData;
    protected CharacterClass characterClass;

    public CharacterData CharacterData => characterData;
    public CharacterClass CharacterClass => characterClass; 

    protected float MaxHp => characterClass.maxHp;
    protected float currentHp => characterClass.currentHp;
    protected Rigidbody rb;
    

    //Lo uso per chimare tutte le funzioni iniziali
    protected void Awake()
    {
        InitialSetup();
    }

    //Tutto ciò che va fatto nello ad inizio
    protected virtual void InitialSetup()
    {
        rb = GetComponent<Rigidbody>();      
        characterData.Inizialize(this);       
        
    }

    protected virtual void Attack(InputAction.CallbackContext context) => characterClass.Attack(this, context);
    protected virtual void Defend(InputAction.CallbackContext context) => characterClass.Defence(this, context);
    public virtual void UseUniqueAbility(InputAction.CallbackContext context) => characterClass.UseUniqueAbility(this, context);
    public virtual void UseExtraAbility(InputAction.CallbackContext context) => characterClass.UseExtraAbility(this, context);
    protected virtual void Move(Vector2 direction) => characterClass.Move(direction, rb);


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
    public float GetDamage() => characterClass.GetDamage();
}
