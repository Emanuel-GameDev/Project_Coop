using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class LilithShopTable : MonoBehaviour
{
    [SerializeField] LocalizeStringEvent abilityNameLocaleEvent;
    [SerializeField] LocalizeStringEvent abilityDescriptionLocaleEvent;
    [SerializeField] TextMeshProUGUI keysNumberText;
    [SerializeField] Selectable buyButton;

    [SerializeField] Selectable firstSelected;

    [SerializeField] public ePlayerCharacter characterReference;
    internal PlayerCharacter playerCharacterReference;

   [SerializeField] public List<AbilityShopEntry> entrys = new List<AbilityShopEntry>(5);

    [Serializable]
    public class AbilityShopEntry
    {
        [SerializeField] public LilithShopButton button;
        [SerializeField] public Image abilityImage;
        [SerializeField] public PlayerAbility[] abilitys = new PlayerAbility[1];

        /*[HideInInspector]*/ public int id;
    }


    LilithShopMenu shopMenu;
    GameObject lastSelected;


    public void InitializeButtons()
    {
        shopMenu = GetComponentInParent<LilithShopMenu>(true);
        playerCharacterReference = PlayerCharacterPoolManager.Instance.AllPlayerCharacters.Find(c => c.Character == characterReference);
        
        if(playerCharacterReference != null)
        {
            if(playerCharacterReference.GetInputHandler() != null)
                playerCharacterReference.GetInputHandler().MultiplayerEventSystem.SetSelectedGameObject(firstSelected.gameObject);

        }

        //da cambiare con i salvataggi
            foreach (AbilityShopEntry entry in entrys)
            {
                entry.button.buttonImage = entry.abilityImage;
                entry.button.SetAbility(entry.abilitys[0]);
            }
        
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

    public void ChangeDescriptionAndName(LocalizedString localStringName, LocalizedString localStringDescription)
    {
        abilityNameLocaleEvent.StringReference = localStringName;
        abilityDescriptionLocaleEvent.StringReference = localStringDescription;
    }
    LilithShopButton lastButton;
    public void SetOnBuyButton()
    {
        lastSelected = playerCharacterReference.GetInputHandler().MultiplayerEventSystem.currentSelectedGameObject;
        lastButton = lastSelected.GetComponent<LilithShopButton>();

        if (!lastButton.isActive)
            return;

        shopMenu.canClose = false;
        playerCharacterReference.GetInputHandler().MultiplayerEventSystem.SetSelectedGameObject(buyButton.gameObject);

        playerCharacterReference.GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("Cancel").performed += CoinShopTable_performed;

    }

    private void CoinShopTable_performed(InputAction.CallbackContext obj)
    {
        DesetOnBuyButton();
        playerCharacterReference.GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("Cancel").performed -= CoinShopTable_performed;
    }

    public void BuyAbility()
    {
       lastButton = lastSelected.GetComponent<LilithShopButton>();

        AbilityShopEntry lastEntry = entrys.Find(b => b.button == lastButton);
        //PlayerCharacterPoolManager.Instance.AllPlayerCharacters.Find(p=>p == playerCharacterReference).UnlockUpgrade(lastEntry.abilitys[lastEntry.id].abilityUpgrade);
        playerCharacterReference.UnlockUpgrade(lastEntry.abilitys[lastEntry.id].abilityUpgrade);

        lastEntry.id++;

        if (lastEntry.id >= lastEntry.abilitys.Length)
        {
            lastButton.DeactivateButton();
        }
        else
            lastButton.SetAbility(lastEntry.abilitys[lastEntry.id]);


        UpdateKeyCounter(--playerCharacterReference.ExtraData.unusedKey);
        KeyRequirementChecks();
        DesetOnBuyButton();



    }

    public void DesetOnBuyButton()
    {
        shopMenu.canClose = true;
        playerCharacterReference.GetInputHandler().MultiplayerEventSystem.SetSelectedGameObject(lastSelected);
    }

    public void UpdateKeyCounter(int counter)
    {
        if (counter < 0) return;

        if(counter < 10)
            keysNumberText.text =$"0{counter}";
        else
            keysNumberText.text = counter.ToString();
    }

    public void KeyRequirementChecks()
    {
        foreach(AbilityShopEntry entry in entrys)
        {
            entry.button.KeyRequiredCheck();
        }
    }
}
