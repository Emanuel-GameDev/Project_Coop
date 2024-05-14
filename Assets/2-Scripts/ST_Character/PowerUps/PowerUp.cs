using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[Serializable]
public enum StatsType
{
    Damage,
    Health,
    MoveSpeed,
    UniqueAbilityCooldown,
    AttackSpeed,
    Stamina,
    DodgeDistance
}

[CreateAssetMenu(menuName = "Character/PowerUp"), Serializable]
public class PowerUp : ScriptableObject
{
    public StatsType powerUpType;

    public Sprite powerUpSprite;
    public Sprite powerUpExtraIcon;
    public LocalizedString powerUpName;
    public LocalizedString powerUpDescription;

    [/*Range(0, 1),*/ Tooltip("Incremento della statistica in percentuale dove 1 = 100%.")]
    public float value;

    public int moneyCost;
}
