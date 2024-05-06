using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsMenuSetter : MonoBehaviour
{
    [SerializeField]
    List<VisualizationChangerHandler> visualizations = new List<VisualizationChangerHandler>();

    [SerializeField]
    List<CharacterStatsReferences> characterStats = new List<CharacterStatsReferences>();

    public void ResetStatus()
    {
        foreach (VisualizationChangerHandler visualization in visualizations)
            visualization.ChangeToDefault();
    }

    public void SetCharacterStats()
    {
        List<PlayerCharacter> playerCharacters = PlayerCharacterPoolManager.Instance.AllPlayerCharacters;


        foreach (PlayerCharacter character in playerCharacters)
        {
            foreach (CharacterStatsReferences characterStatsReference in characterStats)
            {
                if (characterStatsReference.character == character.Character)
                {
                    characterStatsReference.HPText.text = $"{character.MaxHp}";
                    characterStatsReference.KeyText.text = $"{character.ExtraData.key}";
                    characterStatsReference.CoinText.text = $"{character.ExtraData.coin}";

                    characterStatsReference.PowerUp1Value.text = GetPowerUpValue(character.PowerUpData.DamageIncrease);
                    characterStatsReference.PowerUp2Value.text = GetPowerUpValue(character.PowerUpData.MaxHpIncrease);
                    characterStatsReference.PowerUp3Value.text = GetPowerUpValue(character.PowerUpData.UniqueAbilityCooldownDecrease);


                    switch (character.Character)
                    {
                        case ePlayerCharacter.Brutus:
                            characterStatsReference.UniquePowerUpValue.text = GetPowerUpValue(character.PowerUpData.DodgeDistanceIncrease);
                            break;
                        case ePlayerCharacter.Kaina:
                            characterStatsReference.UniquePowerUpValue.text = GetPowerUpValue(character.PowerUpData.StaminaIncrease);
                            break;
                        case ePlayerCharacter.Cassius:
                            characterStatsReference.UniquePowerUpValue.text = GetPowerUpValue(character.PowerUpData.MoveSpeedIncrease);
                            break;
                        case ePlayerCharacter.Jude:
                            characterStatsReference.UniquePowerUpValue.text = GetPowerUpValue(character.PowerUpData.DodgeDistanceIncrease);
                            break;
                    }
                }
            }
        }
    }

    public string GetPowerUpValue(float value)
    {
        int valueInt = Mathf.RoundToInt((value * 100) - 100);

        return $"+{valueInt}%";
    }

    private void OnEnable()
    {
        ResetStatus();
        SetCharacterStats();
    }
}

[Serializable]
public class CharacterStatsReferences
{
    public ePlayerCharacter character;
    public TextMeshProUGUI HPText;
    public TextMeshProUGUI KeyText;
    public TextMeshProUGUI CoinText;

    [Line]
    public TextMeshProUGUI PowerUp1Value;
    public TextMeshProUGUI PowerUp1Description;

    [Line]
    public TextMeshProUGUI PowerUp2Value;
    public TextMeshProUGUI PowerUp2Description;

    [Line]
    public TextMeshProUGUI PowerUp3Value;
    public TextMeshProUGUI PowerUp3Description;

    [Line]
    public TextMeshProUGUI UniquePowerUpValue;
    public TextMeshProUGUI UniquePowerUpDescription;

}
