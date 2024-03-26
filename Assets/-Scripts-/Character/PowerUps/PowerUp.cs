using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

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

    public Sprite powerUpSprite;
    public LocalizedString powerUpName;
    public LocalizedString powerUpDescription;

    [Range(0, 1), Tooltip("Incremento della statistica in percentuale dove 1 = 100%.")]
    public float value;

    public int moneyCost;
}
