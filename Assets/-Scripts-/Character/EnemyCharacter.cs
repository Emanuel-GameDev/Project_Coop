using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
    protected NavMeshAgent agent;
    protected PowerUpData powerUpData;
    public Transform target;

    public virtual float MaxHp => maxHp + powerUpData.maxHpIncrease;
    public float MoveSpeed => moveSpeed;
    public NavMeshAgent Agent => agent;
    public Transform Target => target;

    [HideInInspector]
    public float currentHp;
    
    protected override void InitialSetup()
    {
        base.InitialSetup();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
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
        Debug.Log(currentHp);

        if (data.condition != null)
            data.condition.AddCondition(this);
    }

    public virtual void TargetSelection() 
    {
        List<PlayerCharacter> activePlayers = GameManager.Instance.coopManager.activePlayers;

        Transform target = activePlayers[0].transform;
        float distance = Vector3.Distance(transform.position, target.position);
        
        foreach (PlayerCharacter player in activePlayers)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < distance)
                target = player.transform;
        }

        this.target = target;
    }

}
