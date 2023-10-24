using System;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.U2D.Animation;

public enum AbilityUpgrade
{
    Ability1,
    Ability2,
    Ability3,
    Ability4,
    Ability5
}


public class CharacterClass : MonoBehaviour , IDamageable
{
    protected CharacterData characterData;
    protected Animator animator;
    protected Character character;
    protected PowerUpData powerUpData;
    protected Dictionary<AbilityUpgrade, bool> upgradeStatus; 

    public float MaxHp => characterData.MaxHp + powerUpData.maxHpIncrease;
    public float Damage => characterData.Damage + powerUpData.damageIncrease;
    public float MoveSpeed => characterData.MoveSpeed + powerUpData.moveSpeedIncrease;
    public float AttackSpeed => characterData.AttackSpeed + powerUpData.attackSpeedIncrease;
    public float uniqueAbilityCooldown => characterData.UniqueAbilityCooldown - powerUpData.uniqueAbilityCooldownDecrease;


    public void Inizialize(CharacterData characterData, Character character)
    {
        powerUpData = new PowerUpData();
        this.characterData = characterData;
        upgradeStatus = new();
        foreach(AbilityUpgrade au in Enum.GetValues(typeof(AbilityUpgrade)))
        {
            upgradeStatus.Add(au, false);
        }
        animator = character.GetAnimator();
        this.character = character;
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
        throw new NotImplementedException();
    }

    #region Upgrades
    public virtual void UnlockUpgrade(AbilityUpgrade abilityUpgrade)
    {
        if (upgradeStatus[abilityUpgrade]  == false)
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
