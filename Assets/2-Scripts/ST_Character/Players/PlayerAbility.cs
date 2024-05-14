using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "Character/Ability"), Serializable]
public class PlayerAbility : ScriptableObject
{
    public ePlayerCharacter owner;
    public AbilityUpgrade abilityUpgrade;

    public Sprite abilitySprite;
    public LocalizedString abilityName;
    public LocalizedString abilityDescription;

    public int keyCost;
}
