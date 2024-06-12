using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class UICharacterStats : MonoBehaviour
{
    [Header("Chaaracter")]
    public ePlayerCharacter character;
    public Image CharacterNameFlag;
    public Image CharacterBodyArt;
    public LocalizeStringEvent Biography;

    [Header("Inventory")]
    public TextMeshProUGUI MaxHP;
    public TextMeshProUGUI KeyObtained;
    public TextMeshProUGUI CoinObtained;

    [Header("Ability")]
    public UIAbilityTentButton Ability1;
    public UIAbilityTentButton Ability2;
    public UIAbilityTentButton Ability3;
    public UIAbilityTentButton Ability4;
    public UIAbilityTentButton Ability5;

    [Header("PowerUp")]
    public UISouvenirPowerUP PowerUp1;
    public UISouvenirPowerUP PowerUp2;
    public UISouvenirPowerUP PowerUp3;
    public UISouvenirPowerUP UniquePowerUp;
}
