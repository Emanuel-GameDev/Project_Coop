using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class SouvenirShopTable : MonoBehaviour
{
    [Header("TableInfo")]
    [SerializeField] public ePlayerCharacter character;
    [SerializeField] GameObject buyButton;
    [SerializeField] LocalizeStringEvent souvenirNameLocaleEvent;
    [SerializeField] LocalizeStringEvent souvenirDescriptionLocaleEvent;
    [SerializeField] Image souvenirImage;
    [SerializeField] Image souvenirIcon;
    [SerializeField] TextMeshProUGUI coinsCostNumberText;
    [SerializeField] TextMeshProUGUI playerCoinsNumberText;

    [SerializeField] GameObject soldoutSign;
    [SerializeField] Image soldoutSouvenirImage;

    [SerializeField] Image nextSouvenirImage;
    [SerializeField] Image previousSouvenirImage;

    PlayerCharacter currentPlayerOnTable;
    SouvenirEntry currentSouvenirEntry;


    //Info su cosa vendere in base al personaggio
    [SerializeField] SouvenirEntry[] entries = new SouvenirEntry[4];
    int entryID;

    [Serializable]
    public class SouvenirEntry
    {
        [SerializeField] public PowerUp[] souvenirs = new PowerUp[2];
        [HideInInspector] public int souvenirID;


    }

    public void StartIdleAnimationIn(float delay)
    {
        StartCoroutine(PlayIdle(delay));
    }

    IEnumerator PlayIdle(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<Animation>().Play();
    }

    private void ChangeTableItem(PowerUp souvenirToSell)
    {
        souvenirNameLocaleEvent.StringReference = souvenirToSell.powerUpName;
        souvenirDescriptionLocaleEvent.StringReference = souvenirToSell.powerUpDescription;
        souvenirImage.sprite = souvenirToSell.powerUpSprite;
        coinsCostNumberText.text = souvenirToSell.moneyCost.ToString();

        if (souvenirToSell.powerUpExtraIcon != null)
        {
            souvenirIcon.color = new Color(1,1,1,1);
            souvenirIcon.sprite = souvenirToSell.powerUpExtraIcon;
        }
        else
        {
            souvenirIcon.color = new Color(1, 1, 1, 0);
        }

        //nextSouvenirImage.sprite = entries.;




        soldoutSouvenirImage.sprite = souvenirToSell.powerUpSprite;
    }

   

    public void SetTableCurrentCharacter()
    {
        currentPlayerOnTable = PlayerCharacterPoolManager.Instance.AllPlayerCharacters.Find(c => c.Character == character);

        if (currentPlayerOnTable != null)
        {
            if (currentPlayerOnTable.GetInputHandler() != null)
                currentPlayerOnTable.GetInputHandler().MultiplayerEventSystem.SetSelectedGameObject(buyButton.gameObject);

        }

        //soldoutSign.SetActive(false);

        //foreach (SouvenirEntry entry in entries)
        //{
        //    if(character == currentCharacterInShop.Character)
        //    {
        SetCurrentEntry(entries[0]);
        //        //da cambiare quando ci saranno i salvataggi
        //        //currentSouvenirEntry.souvenirID = 0;
        //        break;
        //    }
        //}
    }

    private void SetCurrentEntry(SouvenirEntry entry)
    {
        for(int i = 0; i < entries.Length; i++)
        {
            if (entry == entries[i])
            {
                if((i+1)<entries.Length)
                    nextSouvenirImage.sprite = entries[i + 1].souvenirs[0].powerUpSprite;
                else
                    nextSouvenirImage.sprite = entries[0].souvenirs[0].powerUpSprite;

                if((i - 1) > 0)
                    previousSouvenirImage.sprite = entries[i - 1].souvenirs[0].powerUpSprite;
                else
                    previousSouvenirImage.sprite = entries[entries.Length-1].souvenirs[0].powerUpSprite;
            }
        }


        currentSouvenirEntry = entry;
        CheckForSouvenir();
    }

    public void BuySouvenir()
    {
        //if (currentPlayerOnTable.ExtraData.coin < currentSouvenirEntry.souvenirs[currentSouvenirEntry.souvenirID].moneyCost) return;
        Debug.Log("Compra");
        currentPlayerOnTable.ExtraData.coin -= currentSouvenirEntry.souvenirs[currentSouvenirEntry.souvenirID].moneyCost;
        currentPlayerOnTable.AddPowerUp(currentSouvenirEntry.souvenirs[currentSouvenirEntry.souvenirID]);
        
        currentSouvenirEntry.souvenirID++;
        
        CheckForSouvenir();
        GetComponentInParent<SouvenirShopMenu>().CheckForMoney();
    }

    public void MoneyCheck()
    {
        if (currentPlayerOnTable.ExtraData.coin < currentSouvenirEntry.souvenirs[currentSouvenirEntry.souvenirID].moneyCost)
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
