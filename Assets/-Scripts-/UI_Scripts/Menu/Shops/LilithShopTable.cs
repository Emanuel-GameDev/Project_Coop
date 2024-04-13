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

    [SerializeField] public ePlayerCharacter characterReference;

    [SerializeField] public AbilityShopEntry[] entrys = new AbilityShopEntry[5];

    [Serializable]
    public class AbilityShopEntry
    {
        [SerializeField] public LilithShopButton button;
        [SerializeField] public PlayerAbility[] abilitys = new PlayerAbility[1];

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
            entry.button.SetAbility(entry.abilitys[0]);
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

    public void BuyAbility()
    {
        //PlayerCharacterController inputReceiver = (PlayerCharacterController)shopMenu.tableAssosiation[this].CurrentReceiver;
        //inputReceiver.ActualPlayerCharacter.UnlockUpgrade(abilityUpgrade);
        //Debug.Log("buy");

        //LilithShopButton lastButton = lastSelected.GetComponent<LilithShopButton>();
        //AbilityShopEntry lastEntry = entrys.Find(b => b.button == lastButton);

        //inputReceiver.ActualPlayerCharacter.AddPowerUp(lastButton.powerUp);

        //lastEntry.id++;
        //if(lastEntry.id >= lastEntry.abilitys.Count)
        //{
        //    Debug.Log("End");
            
        //    lastEntry.id--;
        //}    
        //lastButton.SetPowerUp(lastEntry.abilitys[lastEntry.id]);

        //DesetOnBuyButton();
    }

    public void DesetOnBuyButton()
    {
        shopMenu.canClose = true;
        shopMenu.tableAssosiation[this].MultiplayerEventSystem.SetSelectedGameObject(lastSelected);
    }

    public void UpdateKeyCounter(int counter)
    {
        keysNumberText.text = counter.ToString();
    }
}
