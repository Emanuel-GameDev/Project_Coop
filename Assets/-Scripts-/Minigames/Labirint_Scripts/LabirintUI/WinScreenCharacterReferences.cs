using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class WinScreenCharacterReferences : MonoBehaviour
{
    [SerializeField]
    private LocalizeStringEvent characterRank;
    [SerializeField]
    private TextMeshProUGUI earnedCoin;
    [SerializeField]
    private TextMeshProUGUI totalCoin;
    [SerializeField]
    private TextMeshProUGUI earnedKey;
    [SerializeField]
    private TextMeshProUGUI totalKey;

    public void SetValues(LocalizedString rank, int earnedCoin, int totalCoin, int earnedKey, int totalKey)
    {
        characterRank.StringReference = rank;
        this.earnedCoin.text = earnedCoin.ToString();
        this.totalCoin.text = totalCoin.ToString();
        this.earnedKey.text = earnedKey.ToString();
        this.totalKey.text = totalKey.ToString();
    }
}
