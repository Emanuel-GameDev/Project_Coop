using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
    protected List<Condition> conditions;
    protected UnityAction unityAction;
    protected SpriteRenderer spriteRenderer;
    protected Pivot pivot;
    protected Damager damager;
    protected bool isMoving;
    protected bool bossfightPowerUpUnlocked;
    protected bool isInBossfight;
    protected float uniqueAbilityUses;
    protected float damageReceivedMultiplier = 1;
    protected Vector2 lastNonZeroDirection;


    public virtual float maxHp => characterData.MaxHp + powerUpData.maxHpIncrease;
    [HideInInspector]
    public float currentHp;

    public virtual float Damage => characterData.Damage + powerUpData.damageIncrease;
    public virtual float MoveSpeed => characterData.MoveSpeed + powerUpData.moveSpeedIncrease;
    public virtual float AttackSpeed => characterData.AttackSpeed + powerUpData.attackSpeedIncrease;
    public virtual float UniqueAbilityCooldown => characterData.UniqueAbilityCooldown - powerUpData.uniqueAbilityCooldownDecrease + (characterData.UniqueAbilityCooldownIncreaseAtUse * uniqueAbilityUses);


    #region Animation Variable
    private static string Y = "Y";
    #endregion

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
        damageReceivedMultiplier = 1;
        uniqueAbilityUses = 0;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        pivot = GetComponentInChildren<Pivot>();
        lastNonZeroDirection = Vector2.down;
        damager = GetComponentInChildren<Damager>();
        if (damager != null)
        {
            damager.SetSource(character);
        }
        SetIsInBossfight(false);
    }



    public virtual void Attack(Character parent, InputAction.CallbackContext context)
    {

    }
    public virtual void Defence(Character parent, InputAction.CallbackContext context)
    {

    }

    public virtual void Disable(Character character)
    {

    }

    public virtual void UseUniqueAbility(Character parent, InputAction.CallbackContext context)
    {

    }
    public virtual void UseExtraAbility(Character parent, InputAction.CallbackContext context)
    {

    }

    public virtual void TakeDamage(DamageData data)
    {
        if (data.condition != null)
            conditions.Add((Condition)gameObject.AddComponent(data.condition.GetType()));

        currentHp -= data.damage * damageReceivedMultiplier;
    }

    public virtual float GetDamage() => Damage;
    public virtual void SetIsInBossfight(bool value) => isInBossfight = value;

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

        isMoving = rb.velocity.magnitude > 0.2f;

        Vector2 direction2D = new Vector2(direction.x, direction.z);

        if (direction2D != Vector2.zero)
            lastNonZeroDirection = direction2D;
        SetSpriteDirection(lastNonZeroDirection);


    }

    private void SetSpriteDirection(Vector2 direction)
    {
        if (direction.y != 0)
            animator.SetFloat(Y, direction.y);
        Vector3 scale = pivot.gameObject.transform.localScale;
        if ((direction.x > 0.5 && scale.x > 0) || (direction.x < -0.5 && scale.x < 0))
            scale.x *= -1;

        pivot.gameObject.transform.localScale = scale;
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
