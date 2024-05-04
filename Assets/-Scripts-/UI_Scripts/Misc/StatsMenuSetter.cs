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
                    characterStatsReference.HPText.text = character.MaxHp.ToString();
                    characterStatsReference.KeyText.text = character.ExtraData.key.ToString();
                    characterStatsReference.CoinText.text = character.ExtraData.coin.ToString();
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
}
