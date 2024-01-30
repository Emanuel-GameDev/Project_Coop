using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    protected Animator animator;
    protected PlayerCharacter character;
    protected PowerUpData powerUpData;
    protected Dictionary<AbilityUpgrade, bool> upgradeStatus;
    protected UnityAction unityAction;
    protected SpriteRenderer spriteRenderer;
    protected Pivot pivot;
    protected Damager damager;
    protected bool isMoving;
    protected bool bossfightPowerUpUnlocked;
    protected bool isInBossfight;
    protected float uniqueAbilityUses;
    
    protected Vector2 lastNonZeroDirection;

    [SerializeField, Tooltip("La salute massima del personaggio.")]
    protected float maxHp;
    [SerializeField, Tooltip("Il danno inflitto dal personaggio.")]
    protected float damage;
    [SerializeField, Tooltip("La velocità di attacco del personaggio.")]
    protected float attackSpeed;
    [SerializeField, Tooltip("La velocità di movimento del personaggio.")]
    protected float moveSpeed;
    [SerializeField, Tooltip("Il tempo di attesa per l'abilità unica.")]
    protected float uniqueAbilityCooldown;
    [SerializeField, Tooltip("L'incremento del tempo di attesa dell'abilità unica dopo ogni uso.")]
    protected float uniqueAbilityCooldownIncreaseAtUse;

    public bool Stunned => character.stunned;
    
    public virtual float MaxHp => maxHp + powerUpData.maxHpIncrease;
    //[HideInInspector]
    public float currentHp;

    public virtual float Damage => damage + powerUpData.damageIncrease;
    public virtual float MoveSpeed => moveSpeed + powerUpData.moveSpeedIncrease;
    public virtual float AttackSpeed => attackSpeed + powerUpData.attackSpeedIncrease;
    public virtual float UniqueAbilityCooldown => uniqueAbilityCooldown - powerUpData.uniqueAbilityCooldownDecrease + (uniqueAbilityCooldownIncreaseAtUse * uniqueAbilityUses);
    public float DamageReceivedMultiplier => character.damageReceivedMultiplier;


    #region Animation Variable
    private static string Y = "Y";
    #endregion

    public virtual void Inizialize(PlayerCharacter character)
    {
        powerUpData = new PowerUpData();
        //this.characterData = characterData;
        upgradeStatus = new();
        foreach (AbilityUpgrade au in Enum.GetValues(typeof(AbilityUpgrade)))
        {
            upgradeStatus.Add(au, false);
        }
        animator = GetComponent<Animator>();
        this.character = character;
        bossfightPowerUpUnlocked = false;
        uniqueAbilityUses = 0;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        pivot = GetComponentInChildren<Pivot>();
        lastNonZeroDirection = Vector2.down;
        damager = GetComponentInChildren<Damager>(true);
        if (damager != null)
        {
            damager.SetSource(character);
        }
        currentHp = maxHp;
        SetIsInBossfight(false);
    }

    public virtual void Attack(Character parent, InputAction.CallbackContext context)
    {
        if(Stunned)
        {
            return;
            
        }
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
            character.AddToConditions(data.condition);

        currentHp -= data.damage * DamageReceivedMultiplier;
        damager.RemoveCondition();
        Debug.Log($"Dealer: {data.dealer}, Damage: {data.damage}, Condition: {data.condition}");
    }

    public virtual DamageData GetDamageData()
    {
        DamageData data = new DamageData(Damage, character);
        return data;
    }
    
    public virtual void SetIsInBossfight(bool value) => isInBossfight = value;
    public Vector2 GetLastNonZeroDirection() => lastNonZeroDirection;

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

    //dato un vector 3 setta la velocit� del rigidBody in quella direzione, se il vettore non � normalizzato lo normalizza
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

    protected void SetSpriteDirection(Vector2 direction)
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
