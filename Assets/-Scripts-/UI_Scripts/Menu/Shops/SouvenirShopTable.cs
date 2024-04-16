using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class SouvenirShopTable : MonoBehaviour
{
    [SerializeField] GameObject buyButton;
    [SerializeField] LocalizeStringEvent souvenirNameLocaleEvent;
    [SerializeField] LocalizeStringEvent souvenirDescriptionLocaleEvent;
    [SerializeField] Image souvenirImage;
    [SerializeField] Image souvenirIcon;

    [SerializeField] TextMeshProUGUI coinsNumberText;

    [SerializeField] GameObject soldoutSign;
    [SerializeField] Image soldoutSouvenirImage;

    PlayerCharacter currentPlayerInShop;
    SouvenirEntry currentSouvenirEntry;

    //Info su cosa vendere in base al personaggio
    [SerializeField] SouvenirEntry[] entries = new SouvenirEntry[4];
    [Serializable]
    public class SouvenirEntry
    {
        [SerializeField] public ePlayerCharacter character;
        [SerializeField] public PowerUp[] souvenirs = new PowerUp[2];
        [HideInInspector] public int souvenirID;
    }

    private void ChangeTableItem(PowerUp souvenirToSell)
    {
        souvenirNameLocaleEvent.StringReference = souvenirToSell.powerUpName;
        souvenirDescriptionLocaleEvent.StringReference = souvenirToSell.powerUpDescription;
        souvenirImage.sprite = souvenirToSell.powerUpSprite;
        coinsNumberText.text = souvenirToSell.moneyCost.ToString();

        if (souvenirToSell.powerUpExtraIcon != null)
        {
            souvenirIcon.color = new Color(1,1,1,1);
            souvenirIcon.sprite = souvenirToSell.powerUpExtraIcon;
        }
        else
        {
            souvenirIcon.color = new Color(1, 1, 1, 0);
        }

        soldoutSouvenirImage.sprite = souvenirToSell.powerUpSprite;
    }

    public void SetTableCurrentCharacter(PlayerCharacter currentCharacterInShop)
    {
        currentPlayerInShop = currentCharacterInShop;
        soldoutSign.SetActive(false);

        foreach (SouvenirEntry entry in entries)
        {
            if(entry.character == currentCharacterInShop.Character)
            {
                SetCurrentEntry(entry);
                //da cambiare quando ci saranno i salvataggi
                //currentSouvenirEntry.souvenirID = 0;
                break;
            }
        }
    }

    private void SetCurrentEntry(SouvenirEntry entry)
    {
        currentSouvenirEntry = entry;
        CheckForSouvenir();
    }

    public void BuySouvenir()
    {
        if (currentPlayerInShop.ExtraData.money < currentSouvenirEntry.souvenirs[currentSouvenirEntry.souvenirID].moneyCost) return;

        currentPlayerInShop.ExtraData.money -= currentSouvenirEntry.souvenirs[currentSouvenirEntry.souvenirID].moneyCost;
        currentPlayerInShop.AddPowerUp(currentSouvenirEntry.souvenirs[currentSouvenirEntry.souvenirID]);
        
        currentSouvenirEntry.souvenirID++;
        
        CheckForSouvenir();
        GetComponentInParent<SouvenirShopMenu>().CheckForMoney();
    }

    public void MoneyCheck()
    {
        if (currentPlayerInShop.ExtraData.money < currentSouvenirEntry.souvenirs[currentSouvenirEntry.souvenirID].moneyCost)
            buyButton.GetComponent<Image>().color = Color.gray;
        else
            buyButton.GetComponent<Image>().color = Color.white;
    }

    private void CheckForSouvenir()
    {
        if (currentSouvenirEntry == null)
            return;

        if (currentSouvenirEntry.souvenirID < currentSouvenirEntry.souvenirs.Length)
            ChangeTableItem(currentSouvenirEntry.souvenirs[currentSouvenirEntry.souvenirID]);
        else
        {
            //finiti souvenir
            soldoutSign.SetActive(true);
        }
    }
}
