using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
    protected PlayerCharacter playerCharacter;
    protected PowerUpData powerUpData;
    protected ExtraData extraData;
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

    [SerializeField, Tooltip("Identifica il personaggio.")]
    protected ePlayerCharacter character;
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

    public bool Stunned => playerCharacter.stunned;

    public virtual float MaxHp => maxHp * powerUpData.maxHpIncrease;
    [HideInInspector]
    public float currentHp;

    public ePlayerCharacter Character => character;
    public virtual float Damage => damage * powerUpData.damageIncrease;
    public virtual float MoveSpeed => moveSpeed * powerUpData.moveSpeedIncrease;
    public virtual float AttackSpeed => attackSpeed * powerUpData.attackSpeedIncrease;
    public virtual float UniqueAbilityCooldown => (uniqueAbilityCooldown + (uniqueAbilityCooldownIncreaseAtUse * uniqueAbilityUses)) * powerUpData.UniqueAbilityCooldownDecrease;
    public float DamageReceivedMultiplier => playerCharacter.damageReceivedMultiplier;

    #region Animation Variable
    private static string Y = "Y";
    #endregion

    public virtual void Inizialize()
    {
        powerUpData = new PowerUpData();
        extraData = new ExtraData();
        upgradeStatus = new();
        currentHp = MaxHp;
        foreach (AbilityUpgrade au in Enum.GetValues(typeof(AbilityUpgrade)))
        {
            upgradeStatus.Add(au, false);
        }
        animator = GetComponent<Animator>();
        bossfightPowerUpUnlocked = false;
        uniqueAbilityUses = 0;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        pivot = GetComponentInChildren<Pivot>();
        lastNonZeroDirection = Vector2.down;
        damager = GetComponentInChildren<Damager>(true);
        if (damager != null)
        {
            damager.SetSource(playerCharacter);
        }
        SetIsInBossfight(false);
    }

    public virtual void Enable(PlayerCharacter character)
    {
        playerCharacter = character;
        lastNonZeroDirection = Vector2.down;
        transform.parent = playerCharacter.transform;
        transform.localPosition = Vector3.zero;
        damager.SetSource(playerCharacter);
    }

    public virtual void Disable(PlayerCharacter character)
    {
        playerCharacter = null;
        damager.SetSource(null);
        CharacterPoolManager.Instance.ReturnCharacter(this);
    }

    #region Abilities

    public virtual void Attack(Character parent, InputAction.CallbackContext context)
    {
        if (Stunned)
        {
            return;

        }
    }
    public virtual void Defence(Character parent, InputAction.CallbackContext context)
    {

    }

    public virtual void UseUniqueAbility(Character parent, InputAction.CallbackContext context)
    {

    }
    public virtual void UseExtraAbility(Character parent, InputAction.CallbackContext context)
    {

    }
    #endregion

    #region Combat
    public virtual void TakeDamage(DamageData data)
    {

        if (data.condition != null)
            playerCharacter.AddToConditions(data.condition);

        currentHp -= data.damage * DamageReceivedMultiplier;
        damager.RemoveCondition();
        Debug.Log($"Dealer: {data.dealer}, Damage: {data.damage}, Condition: {data.condition}");
    }

    public virtual DamageData GetDamageData()
    {
        DamageData data = new DamageData(Damage, playerCharacter);
        return data;
    }

    public virtual void SetIsInBossfight(bool value) => isInBossfight = value;
    #endregion

    #region Move
    public Vector2 GetLastNonZeroDirection() => lastNonZeroDirection;

    // Dato un vector2 chiama move col Vector3
    public virtual void Move(Vector2 direction, Rigidbody2D rb)
    {
        //Move(new Vector3(direction.x, direction.y, direction.y).normalized, rb);
        if (!direction.normalized.Equals(direction))
            direction = direction.normalized;

        rb.velocity = direction * MoveSpeed;

        isMoving = rb.velocity.magnitude > 0.2f;

        if (direction != Vector2.zero)
            lastNonZeroDirection = direction;

        SetSpriteDirection(lastNonZeroDirection);
    }

    //dato un vector 3 setta la velocit� del rigidBody in quella direzione, se il vettore non � normalizzato lo normalizza
    //public virtual void Move(Vector3 direction, Rigidbody2D rb)
    //{
    //    if (!direction.normalized.Equals(direction))
    //        direction = direction.normalized;
    //    //rb.velocity = new Vector3(direction.x * MoveSpeed, direction.y, direction.z * MoveSpeed);

    //    Vector3 movementDirection = new Vector3(direction.x, direction.y , direction.z);

    //    rb.velocity = movementDirection * MoveSpeed;

    //    isMoving = rb.velocity.magnitude > 0.2f;

    //    Vector2 direction2D = new Vector2(direction.x, direction.z);

    //    if (direction2D != Vector2.zero)
    //        lastNonZeroDirection = direction2D;
    //    SetSpriteDirection(lastNonZeroDirection);
    //}

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
    public void AddPowerUp(PowerUp powerUp) => powerUpData.Add(powerUp);

    public void RemovePowerUp(PowerUp powerUp) => powerUpData.Remove(powerUp);

    public List<PowerUp> GetPowerUpList() => powerUpData.powerUps;

    #endregion

    #region SaveLoadGame
    public virtual ClassData SaveClassData()
    {
        ClassData data = new ClassData();

        data.className = this.GetType().ToString();

        data.powerUpsData = powerUpData;
        data.extraData = extraData;

        foreach (KeyValuePair<AbilityUpgrade, bool> kvp in upgradeStatus)
        {
            if (kvp.Value)
                data.unlockedAbility.Add(kvp.Key);
        }

        return data;
    }

    public virtual void LoadClassData(ClassData data)
    {
        string className = this.GetType().ToString();

        if (data == null || data.className != className)
            return;

        powerUpData = data.powerUpsData;
        extraData = data.extraData;

        foreach (AbilityUpgrade au in data.unlockedAbility)
        {
            upgradeStatus[au] = true;
        }
    }

    #endregion

    #region Misc
    public ExtraData GetExtraData()
    {
        ExtraData data = extraData;
        return extraData;
    }


    #endregion
}
