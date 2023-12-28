using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class EnemyCharacter : Character
{
    [SerializeField, Tooltip("La salute massima del nemico.")]
    protected float maxHp;
    [SerializeField, Tooltip("Il danno inflitto dal nemico.")]
    protected float damage;
    [SerializeField, Tooltip("Il danno inflitto alla stamina dal nemico.")]
    protected float staminaDamage;
    [SerializeField, Tooltip("La velocità di attacco del nemico.")]
    protected float attackSpeed;
    [SerializeField, Tooltip("La velocità di movimento del nemico.")]
    protected float moveSpeed;

    protected Animator animator;
    protected PowerUpData powerUpData;

    public virtual float MaxHp => maxHp + powerUpData.maxHpIncrease;
    [HideInInspector]
    public float currentHp;
    
    protected override void InitialSetup()
    {
        base.InitialSetup();
        animator = GetComponent<Animator>();
    }

    #region PowerUp
    public override void AddPowerUp(PowerUp powerUp) => powerUpData.Add(powerUp);
    public override List<PowerUp> GetPowerUpList() => powerUpData._powerUpData;
    public override void RemovePowerUp(PowerUp powerUp) => powerUpData.Remove(powerUp);
    #endregion
    public override DamageData GetDamageData()
    {
        return new DamageData(damage, staminaDamage, this, false);
    }
    public override void TakeDamage(DamageData data)
    {
        currentHp -= data.damage * damageReceivedMultiplier;

        if (data.condition != null)
            data.condition.AddCondition(this);
    }
}
