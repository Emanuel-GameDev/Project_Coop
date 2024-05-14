using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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

    bool canBuy = true;

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

    public void SetUpInput()
    {
        if(currentPlayerOnTable != null)
        {
            if (currentPlayerOnTable.GetInputHandler())
            {
                currentPlayerOnTable.GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("Next").performed += GoToNextEntryInput;
                currentPlayerOnTable.GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("Previous").performed += GoToPreviousEntryInput;
                currentPlayerOnTable.GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("RandomSelection").performed += GoToCrownedEntryInput;

            }

        }

    }
    public void DesetInput()
    {
        if (currentPlayerOnTable != null)
        {
            if (currentPlayerOnTable.GetInputHandler())
            {
                currentPlayerOnTable.GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("Next").performed -= GoToNextEntryInput;
                currentPlayerOnTable.GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("Previous").performed -= GoToPreviousEntryInput;
                currentPlayerOnTable.GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("RandomSelection").performed -= GoToCrownedEntryInput;
            }
        }
    }
    private void GoToCrownedEntryInput(InputAction.CallbackContext context)
    {
        GoNextSouvenir();
    }
    private void GoToPreviousEntryInput(InputAction.CallbackContext context)
    {
        GoToPreviousEntry();
    }
    private void GoToNextEntryInput(InputAction.CallbackContext context)
    {
        GoToNextEntry();
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



       
        //soldoutSouvenirImage.sprite = souvenirToSell.powerUpSprite;
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
        SetCurrentEntry(entries[entryID]);
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


                entryID = i;
                break;
            }
        }


        currentSouvenirEntry = entry;
        CheckForSouvenir();
        MoneyCheck();
    }

    public void GoNextSouvenir()
    {
        if ((currentSouvenirEntry.souvenirID + 1) < entries[entryID].souvenirs.Length)
        {
            currentSouvenirEntry.souvenirID++;
            CheckForSouvenir();
        }
        else
        {
            currentSouvenirEntry.souvenirID  = 0;
            CheckForSouvenir();
        }
    }

    public void GoToNextEntry()
    {
        if ((entryID + 1) < entries.Length)
            SetCurrentEntry(entries[entryID + 1]);
        else
            SetCurrentEntry(entries[0]);
    }

    public void GoToPreviousEntry()
    {
        if ((entryID - 1) > 0)
            SetCurrentEntry(entries[entryID - 1]);
        else
            SetCurrentEntry(entries[entries.Length - 1]);
    }

    public void BuySouvenir()
    {
        if (!canBuy) return;


        if (currentPlayerOnTable.ExtraData.coin < currentSouvenirEntry.souvenirs[currentSouvenirEntry.souvenirID].moneyCost) return;

        currentPlayerOnTable.ExtraData.coin -= currentSouvenirEntry.souvenirs[currentSouvenirEntry.souvenirID].moneyCost;
        currentPlayerOnTable.AddPowerUp(currentSouvenirEntry.souvenirs[currentSouvenirEntry.souvenirID]);
        
        
        CheckForSouvenir();
        MoneyCheck();

        StartCoroutine(BuyDelay());
    }

    public void MoneyCheck()
    {
        UpdateCoinCounter();

        if (currentPlayerOnTable.ExtraData.coin < currentSouvenirEntry.souvenirs[currentSouvenirEntry.souvenirID].moneyCost)
            buyButton.GetComponent<Image>().color = Color.gray;
        else
            buyButton.GetComponent<Image>().color = Color.white;
    }

    private void UpdateCoinCounter()
    {
        playerCoinsNumberText.text = currentPlayerOnTable.ExtraData.coin.ToString();
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
            //soldoutSign.SetActive(true);
        }
    }

    IEnumerator BuyDelay()
    {
        canBuy = false;
        yield return new WaitForSecondsRealtime(0.3f);
        canBuy = true;
    }

    
}
