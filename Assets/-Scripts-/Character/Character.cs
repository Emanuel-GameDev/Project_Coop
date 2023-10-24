using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Character : MonoBehaviour, IDamageable
{
    [SerializeField] protected CharacterData characterData;
    [SerializeField] protected Damager attackDamager;
    protected CharacterClass characterClass;

    protected float MaxHp => characterClass.MaxHp;
    protected float currentHp;
    protected float Speed => characterClass.MoveSpeed;
    protected Rigidbody rb;
    protected Animator animator;

    //Lo uso per chimare tutte le funzioni iniziali
    protected virtual void Start()
    {
        InitialSetup();
    }

    //Tutto ciò che va fatto nello ad inizio
    protected virtual void InitialSetup()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        characterData.Inizialize(this);
        attackDamager = GetComponentInChildren<Damager>();
    }

    protected virtual void Attack() => characterClass.Attack(this);
    protected virtual void Defend() => characterClass.Defence(this);
    public virtual void UseUniqueAbility() => characterClass.UseUniqueAbility(this);
    public virtual void UseExtraAbility() => characterClass.UseExtraAbility(this);

    #region Move
    //dati x e z chiama Move col Vector2
    protected virtual void Move(float x, float z)
    {
        Move(new Vector2(x, z));
    }

    // Dato un vector2 chiama move col Vector3
    protected virtual void Move(Vector2 direction)
    {
        Move(new Vector3(direction.x, 0, direction.y).normalized);
    }

    //dato un vector 3 setta la velocità del rigidBody in quella direzione, se il vettore non è normalizzato lo normalizza
    protected virtual void Move(Vector3 direction)
    {
        if (!direction.normalized.Equals(direction))
            direction = direction.normalized;

        rb.velocity = new Vector3(direction.x * Speed, direction.y, direction.z * Speed);
    }
    #endregion

    public void AddPowerUp(PowerUp powerUp) => characterClass.AddPowerUp(powerUp);
    public void RemovePowerUp(PowerUp powerUp) => characterClass.RemovePowerUp(powerUp);
    public List<PowerUp> GetPowerUpList() => characterClass.GetPowerUpList();

    public void UnlockUpgrade(AbilityUpgrade abilityUpgrade) => characterClass.UnlockUpgrade(abilityUpgrade);

    public void SetCharacterClass(CharacterClass cClass) => characterClass = cClass;
    public void SetAnimatorController(AnimatorController controller) => animator.runtimeAnimatorController = controller;
    public Animator GetAnimator() => animator;

    public virtual void TakeDamage(float damage, Damager dealer)
    {
        
    }
}
