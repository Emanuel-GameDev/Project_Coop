using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class CoinShopTable : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI abilityNameText;
    [SerializeField] LocalizeStringEvent abilityNameLocaleEvent;
    [SerializeField] TextMeshProUGUI abilityDescriptionText;
    [SerializeField] LocalizeStringEvent abilityDescriptionLocaleEvent;
    [SerializeField] TextMeshProUGUI coinsNumberText;
    [SerializeField] Selectable buyButton;

    [SerializeField] public List<CoinShopEntry> entrys;

    [Serializable]
    public class CoinShopEntry
    {
        [SerializeField] public CoinShopButton button;
        [SerializeField] public List<PowerUp> abilitys;
    }


    CoinShopMenu shopMenu;
    GameObject lastSelected;

    private void Start()
    {
        shopMenu = GetComponentInParent<CoinShopMenu>();

        InitializeButtons();
    }

    private void InitializeButtons()
    {
        //da cambiare con i salvataggi
        foreach(CoinShopEntry entry in entrys)
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
        inputReceiver.ActualPlayerCharacter.AddPowerUp(lastSelected.GetComponent<CoinShopButton>().powerUp);

        DesetOnBuyButton();
    }

    public void DesetOnBuyButton()
    {
        shopMenu.canClose = true;
        shopMenu.tableAssosiation[this].MultiplayerEventSystem.SetSelectedGameObject(lastSelected);
    }
}
