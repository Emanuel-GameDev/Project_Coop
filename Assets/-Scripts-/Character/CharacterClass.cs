using System;
using System.Collections.Generic;
using UnityEngine;

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
    public float currentHp;

    public virtual float Damage => characterData.Damage + powerUpData.damageIncrease;
    public virtual float MoveSpeed => characterData.MoveSpeed + powerUpData.moveSpeedIncrease;
    public  virtual float AttackSpeed => characterData.AttackSpeed + powerUpData.attackSpeedIncrease;
    public virtual float UniqueAbilityCooldown => characterData.UniqueAbilityCooldown - powerUpData.uniqueAbilityCooldownDecrease + (characterData.UniqueAbilityCooldownIncreaseAtUse * uniqueAbilityUses);

    protected float uniqueAbilityUses;

    public void Inizialize(CharacterData characterData, Character character)
    {
        powerUpData = new PowerUpData();
        this.characterData = characterData;
        upgradeStatus = new();
        foreach (AbilityUpgrade au in Enum.GetValues(typeof(AbilityUpgrade)))
        {
            upgradeStatus.Add(au, false);
        }
        animator = character.GetAnimator();
        this.character = character;
        bossfightPowerUpUnlocked = false;
        uniqueAbilityUses = 0;
    }

    public virtual void Attack(Character parent)
    {
        Debug.Log($"Attack from {parent.name} with {characterData.Damage} damage and {characterData.AttackSpeed} attack speed");
        Debug.Log($"TotalAttack: {characterData.Damage + powerUpData.damageIncrease}");
        Debug.Log($"UpgradeStatus: {upgradeStatus[0]}");
    }
    public virtual void Defence(Character parent)
    {

    }
    public virtual void UseUniqueAbility(Character parent)
    {

    }
    public virtual void UseExtraAbility(Character parent)
    {

    }
    public virtual void TakeDamage(float damage, Damager dealer)
    {

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

    #region PowerUp
    internal void AddPowerUp(PowerUp powerUp) => powerUpData.Add(powerUp);

    internal void RemovePowerUp(PowerUp powerUp) => powerUpData.Remove(powerUp);

    internal List<PowerUp> GetPowerUpList() => powerUpData._powerUpData;


    #endregion


}
