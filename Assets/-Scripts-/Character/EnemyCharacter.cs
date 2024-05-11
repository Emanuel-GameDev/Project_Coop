using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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


    public virtual float MaxHp => maxHp + powerUpData.MaxHpIncrease;
    public float MoveSpeed => moveSpeed;
    public float Damage => damage;
    public float StaminaDamage => staminaDamage;
    public NavMeshAgent Agent => agent;
    public Transform Target => target;

    [HideInInspector]
    public float currentHp;

    protected override void InitialSetup()
    {
        base.InitialSetup();
        animator = GetComponent<Animator>();
        agent = GetComponentInChildren<NavMeshAgent>();

        currentHp = maxHp;
    }

    #region PowerUp
    public override void AddPowerUp(PowerUp powerUp) => powerUpData.Add(powerUp);
    public override List<PowerUp> GetPowerUpList() => powerUpData.PowerUps;
    public override void RemovePowerUp(PowerUp powerUp) => powerUpData.Remove(powerUp);
    #endregion
    public override DamageData GetDamageData()
    {
        return new DamageData(damage, staminaDamage, this, false);
    }
    public override void TakeDamage(DamageData data)
    {
        if (!isDead)
        {

            currentHp -= data.damage * damageReceivedMultiplier;
            OnHit?.Invoke();
            Debug.Log(currentHp);

            //shader
            SetHitMaterialColor(_OnHitColor);

            if (currentHp <= 0)
            {
                Death();

            }
            if (data.condition != null)
                data.condition.AddCondition(this);
        }
    }

    public virtual void Death()
    {
        if (isDead == false)
        {
            isDead = true;
            OnDeath?.Invoke();
            animator.SetTrigger("isDead");
            TargetManager.Instance.RemoveEnemy(this);
            OnDeath.RemoveAllListeners();
        }
    }

    public virtual void TargetSelection()
    {
        List<PlayerCharacter> activePlayers = PlayerCharacterPoolManager.Instance.ActivePlayerCharacters;

        List<PlayerCharacter> alivePlayers = new();
        foreach(PlayerCharacter p in activePlayers)
        {
            if (!p.isDead)
                alivePlayers.Add(p);

            
        }
      

        Transform target = null;
        float distance = float.MaxValue;

        foreach (PlayerCharacter player in alivePlayers)
        {
            
                if (Vector3.Distance(transform.position, player.transform.position) < distance && !player.isDead)
                    target = player.transform;
        }

        if(target == null)
        {
            target = alivePlayers[0].transform;
            
        }
        



        SetTarget(target);
    }

    public virtual void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }


}
