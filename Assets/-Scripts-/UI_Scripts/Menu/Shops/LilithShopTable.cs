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
    [SerializeField] TextMeshProUGUI coinsNumberText;
    [SerializeField] Selectable buyButton;

    [SerializeField] public ePlayerCharacter characterReference;

    [SerializeField] public List<AbilityShopEntry> entrys;

    [Serializable]
    public class AbilityShopEntry
    {
        [SerializeField] public LilithShopButton button;
        [SerializeField] public List<PowerUp> abilitys;

        [HideInInspector] public int id;
    }


    LilithShopMenu shopMenu;
    GameObject lastSelected;

    private void Start()
    {
        shopMenu = GetComponentInParent<LilithShopMenu>();

        InitializeButtons();
    }

    private void InitializeButtons()
    {
        //da cambiare con i salvataggi
        foreach(AbilityShopEntry entry in entrys)
        {
            entry.button.SetPowerUp(entry.abilitys[0]);
        }
    }

    public void ChangeDescriptionAndName(LocalizedString localStringName, LocalizedString localStringDescription)
    {
        abilityNameLocaleEvent.StringReference = localStringName;
        abilityDescriptionLocaleEvent.StringReference = localStringDescription;
    }

    public void SetOnBuyButton()
    {
        lastSelected = shopMenu.tableAssosiation[this].MultiplayerEventSystem.currentSelectedGameObject;
        shopMenu.tableAssosiation[this].MultiplayerEventSystem.SetSelectedGameObject(buyButton.gameObject);
        shopMenu.canClose = false;

        shopMenu.tableAssosiation[this].GetComponent<PlayerInput>().actions.FindAction("Menu").performed += CoinShopTable_performed;

    }

    private void CoinShopTable_performed(InputAction.CallbackContext obj)
    {
        DesetOnBuyButton();
        shopMenu.tableAssosiation[this].GetComponent<PlayerInput>().actions.FindAction("Menu").performed += CoinShopTable_performed;
    }

    public void BuySouvenir()
    {
        Debug.Log("buy");
        PlayerCharacterController inputReceiver = (PlayerCharacterController)shopMenu.tableAssosiation[this].CurrentReceiver;

        LilithShopButton lastButton = lastSelected.GetComponent<LilithShopButton>();
        AbilityShopEntry lastEntry = entrys.Find(b => b.button == lastButton);

        inputReceiver.ActualPlayerCharacter.AddPowerUp(lastButton.powerUp);

        lastEntry.id++;
        if(lastEntry.id >= lastEntry.abilitys.Count)
        {
            Debug.Log("End");
            
            lastEntry.id--;
        }    
        lastButton.SetPowerUp(lastEntry.abilitys[lastEntry.id]);

        DesetOnBuyButton();
    }

    public void DesetOnBuyButton()
    {
        shopMenu.canClose = true;
        shopMenu.tableAssosiation[this].MultiplayerEventSystem.SetSelectedGameObject(lastSelected);
    }

    public void UpdateCoinCounter(int counter)
    {
        coinsNumberText.text = counter.ToString();
    }
}
