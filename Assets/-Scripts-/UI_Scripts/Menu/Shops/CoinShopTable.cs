using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    private void Start()
    {
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

    public void SetOnBuyButton(int p)
    {

    }
}
