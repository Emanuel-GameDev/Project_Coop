using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType
{
    damage,
    health,
    speed,
    cooldown,
    stamina
}

[CreateAssetMenu(menuName = "PowerUp")]
public class PowerUp : ScriptableObject
{
    protected PowerUpType powerUpType;

    // Damage
    [SerializeField] private int damageUp;

    // Health
    [SerializeField] private int healthUp;
    public virtual void Pick(Character c)
    {
        c.powerPool.Add(this);
    }
}
