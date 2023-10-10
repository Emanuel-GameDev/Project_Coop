using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType
{
    Damage,
    Health,
    Speed,
    Cooldown,
    Stamina
}

[CreateAssetMenu(menuName = "PowerUp")]
public class PowerUp : ScriptableObject
{
    public PowerUpType powerUpType;

    // Damage
    public int damageIncrease;

    // Health
    public int healthIncrease;

    // Speed
    public float speedIncrease;

    // Cooldown
    public int cooldownDecrease;

    // Stamina
    public int staminaIncrease;

    
}
