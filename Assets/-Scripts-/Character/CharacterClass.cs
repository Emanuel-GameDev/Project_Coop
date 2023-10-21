using System;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class CharacterClass : MonoBehaviour
{

    [SerializeField] protected AnimatorOverrideController animatorOverride;
    [SerializeField] protected SpriteLibraryAsset spriteLibrary;

    protected CharacterData characterData;
    protected PowerUpData powerUpData;
    protected bool[] upgradeStatus;
    protected static int upgradeQuantity = 5;

    public float MaxHp => characterData.MaxHp + powerUpData.maxHpIncrease;
    public float Damage => characterData.Damage + powerUpData.damageIncrease;
    public float MoveSpeed => characterData.MoveSpeed + powerUpData.moveSpeedIncrease;
    public float AttackSpeed => characterData.AttackSpeed + powerUpData.attackSpeedIncrease;
    public float uniqueAbilityCooldown => characterData.UniqueAbilityCooldown - powerUpData.uniqueAbilityCooldownDecrease;


    public void Inizialize(CharacterData characterData)
    {
        powerUpData = new PowerUpData();
        this.characterData = characterData;
        upgradeStatus = new bool[upgradeQuantity];
        for (int i = 0; i < upgradeQuantity; i++)
        {
            upgradeStatus[i] = false;
        }
        
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

    #region Upgrades
    public virtual void UnlockUpgrade(int n)
    {
        if (0 <= n && n < upgradeQuantity)
        {
            upgradeStatus[n] = true;
        }
    }
    public virtual void LockUpgrade(int n)
    {
        if (0 <= n && n < upgradeQuantity)
        {
            upgradeStatus[n] = false;
        }
    }
    #endregion

    #region PowerUp
    internal void AddPowerUp(PowerUp powerUp) => powerUpData.Add(powerUp);

    internal void RemovePowerUp(PowerUp powerUp) => powerUpData.Remove(powerUp);

    internal List<PowerUp> GetPowerUpList() => powerUpData._powerUpData;
    #endregion


}
