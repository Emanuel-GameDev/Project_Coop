using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eStatsType
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
    public eStatsType powerUpType;

    [Range(0, 1), Tooltip("Incremento della statistica in percentuale dove 1 = 100%.")]
    public float value;

    public int moneyCost;
}
