using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum AbilityUpgrade
{
    Ability1,
    Ability2,
    Ability3,
    Ability4,
    Ability5
}


public class CharacterClass : MonoBehaviour
{
    protected CharacterData characterData;
    protected Animator animator;
    protected Character character;
    protected PowerUpData powerUpData;
    protected Dictionary<AbilityUpgrade, bool> upgradeStatus;
    protected bool bossfightPowerUpUnlocked;

    public virtual float MaxHp => characterData.MaxHp + powerUpData.maxHpIncrease;
    //[HideInInspector]
    public float currentHp;

    public virtual float Damage => characterData.Damage + powerUpData.damageIncrease;
    public virtual float MoveSpeed => characterData.MoveSpeed + powerUpData.moveSpeedIncrease;
    public  virtual float AttackSpeed => characterData.AttackSpeed + powerUpData.attackSpeedIncrease;
    public virtual float UniqueAbilityCooldown => characterData.UniqueAbilityCooldown - powerUpData.uniqueAbilityCooldownDecrease + (characterData.UniqueAbilityCooldownIncreaseAtUse * uniqueAbilityUses);

    protected float uniqueAbilityUses;

    public virtual void Inizialize(CharacterData characterData, Character character)
    {
        powerUpData = new PowerUpData();
        this.characterData = characterData;
        upgradeStatus = new();
        foreach (AbilityUpgrade au in Enum.GetValues(typeof(AbilityUpgrade)))
        {
            upgradeStatus.Add(au, false);
        }
        animator = GetComponent<Animator>();
        this.character = character;
        bossfightPowerUpUnlocked = false;
        uniqueAbilityUses = 0;
    }

    public virtual void Attack(Character parent, InputAction.CallbackContext context)
    {
       
    }
    public virtual void Defence(Character parent, InputAction.CallbackContext context)
    {
       
    }

    public virtual void Disable(Character character)
    {
        // Disattivo eventuali modifiche al prefab
    }

    public virtual void UseUniqueAbility(Character parent, InputAction.CallbackContext context)
    {
       
    }
    public virtual void UseExtraAbility(Character parent, InputAction.CallbackContext context)
    {
      
    }
    public virtual void TakeDamage(float damage, IDamager dealer)
    {

    }

    public virtual float GetDamage() => Damage;

    #region Move
    //dati x e z chiama Move col Vector2
    public virtual void Move(float x, float z, Rigidbody rb)
    {
        Move(new Vector2(x, z), rb);
    }

    // Dato un vector2 chiama move col Vector3
    public virtual void Move(Vector2 direction, Rigidbody rb)
    {
        Move(new Vector3(direction.x, 0, direction.y).normalized, rb);
    }

    //dato un vector 3 setta la velocità del rigidBody in quella direzione, se il vettore non è normalizzato lo normalizza
    public virtual void Move(Vector3 direction, Rigidbody rb)
    {
        if (!direction.normalized.Equals(direction))
            direction = direction.normalized;

        rb.velocity = new Vector3(direction.x * MoveSpeed, direction.y, direction.z * MoveSpeed);
    }
    #endregion


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

    #region PowerUp
    internal void AddPowerUp(PowerUp powerUp) => powerUpData.Add(powerUp);

    internal void RemovePowerUp(PowerUp powerUp) => powerUpData.Remove(powerUp);

    internal List<PowerUp> GetPowerUpList() => powerUpData._powerUpData;



    #endregion


}
