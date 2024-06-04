using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class WinScreenCharacterReferences : MonoBehaviour
{
    [SerializeField]
    private LocalizeStringEvent characterRank;
    [SerializeField]
    Image characterIcon;
    [SerializeField]
    Image medalIcon;
    [SerializeField]
    private TextMeshProUGUI playerName;
    [SerializeField]
    private TextMeshProUGUI earnedCoin;
    [SerializeField]
    private TextMeshProUGUI totalCoin;
    [SerializeField]
    private TextMeshProUGUI earnedKey;
    [SerializeField]
    private TextMeshProUGUI totalKey;

    public void SetValues(LocalizedString rank, Sprite medalIcon, ePlayerCharacter character, ePlayerID playerID, int earnedCoin, int totalCoin, int earnedKey, int totalKey)
    {
        characterRank.StringReference = rank;
        this.medalIcon.sprite = medalIcon;
        characterIcon.sprite = GameManager.Instance.GetCharacterData(character).PixelFaceSprite;
        playerName.text = playerID != ePlayerID.NotSet ? playerID.ToString() : "";
        this.earnedCoin.text = earnedCoin.ToString();
        this.totalCoin.text = totalCoin.ToString();
        this.earnedKey.text = earnedKey.ToString();
        this.totalKey.text = totalKey.ToString();
    }
}
