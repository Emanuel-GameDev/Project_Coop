using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType
{
    Damage,
    Health,
    MoveSpeed,
    UniqueAbilityCooldown,
    AttackSpeed
}

[CreateAssetMenu(menuName = "Character/PowerUp")]
public class PowerUp : ScriptableObject
{
    public PowerUpType powerUpType;

    public float value;
}
