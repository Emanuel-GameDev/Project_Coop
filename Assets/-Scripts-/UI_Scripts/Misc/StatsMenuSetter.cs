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
                    characterStatsReference.HPText.text = $"+{character.MaxHp}%";
                    characterStatsReference.KeyText.text = $"+{character.ExtraData.key}%";
                    characterStatsReference.CoinText.text = $"+{character.ExtraData.coin}%";

                    characterStatsReference.PowerUp1Value.text = $"+{(character.PowerUpData.DamageIncrease * 100) - 100}%";
                    characterStatsReference.PowerUp2Value.text = $"+{(character.PowerUpData.MaxHpIncrease * 100) - 100}%";
                    characterStatsReference.PowerUp3Value.text = $"+{100 - (character.PowerUpData.UniqueAbilityCooldownDecrease * 100)}%";


                    switch (character.Character)
                    {
                        case ePlayerCharacter.Brutus:
                            characterStatsReference.UniquePowerUpValue.text = $"+{(character.PowerUpData.DodgeDistanceIncrease * 100) - 100}%";
                            break;
                        case ePlayerCharacter.Kaina:
                            characterStatsReference.UniquePowerUpValue.text = $"+{(character.PowerUpData.StaminaIncrease * 100) - 100}%";
                            break;
                        case ePlayerCharacter.Cassius:
                            characterStatsReference.UniquePowerUpValue.text = $"+{(character.PowerUpData.MoveSpeedIncrease * 100) - 100}%";
                            break;
                        case ePlayerCharacter.Jude:
                            characterStatsReference.UniquePowerUpValue.text = $"+{(character.PowerUpData.DodgeDistanceIncrease * 100) - 100}%";
                            break;
                    }
                }
            }
        }
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
