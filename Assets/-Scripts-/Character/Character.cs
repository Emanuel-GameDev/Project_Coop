using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Character : MonoBehaviour, IDamageable
{
    [SerializeField] protected CharacterData characterData;
    [SerializeField] protected Damager attackDamager;
    protected CharacterClass characterClass;

    public CharacterData CharacterData => characterData;
    protected float MaxHp => characterClass.MaxHp;
    protected float currentHp => characterClass.currentHp;
    protected Rigidbody rb;
    protected Animator animator;

    //Lo uso per chimare tutte le funzioni iniziali
    protected void Awake()
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
    protected virtual void Move(Vector2 direction) => characterClass.Move(direction, rb);


    public void AddPowerUp(PowerUp powerUp) => characterClass.AddPowerUp(powerUp);
    public void RemovePowerUp(PowerUp powerUp) => characterClass.RemovePowerUp(powerUp);
    public List<PowerUp> GetPowerUpList() => characterClass.GetPowerUpList();

    public void UnlockUpgrade(AbilityUpgrade abilityUpgrade) => characterClass.UnlockUpgrade(abilityUpgrade);

    public void SetCharacterData(CharacterData newCharData)
    {
        characterData = newCharData;
        newCharData.Inizialize(this);
    }

    public void SetCharacterClass(CharacterClass cClass) => characterClass = cClass;
    public void SetAnimatorController(AnimatorController controller) => animator.runtimeAnimatorController = controller;
    public Animator GetAnimator() => animator;
    public Damager GetDamager() => attackDamager;

    public virtual void TakeDamage(float damage, Damager dealer) => characterClass.TakeDamage(damage, dealer);
}
