using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class LilithShopTable : MonoBehaviour
{
    [SerializeField] LocalizeStringEvent abilityNameLocaleEvent;
    [SerializeField] LocalizeStringEvent abilityDescriptionLocaleEvent;
    [SerializeField] TextMeshProUGUI keysNumberText;
    [SerializeField] Selectable buyButton;

    [SerializeField] public ePlayerCharacter characterReference;
    internal PlayerCharacter playerCharacterReference;

   [SerializeField] public List<AbilityShopEntry> entrys = new List<AbilityShopEntry>(5);

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
        playerCharacterReference = PlayerCharacterPoolManager.Instance.AllPlayerCharacters.Find(c => c.Character == characterReference);

        //da cambiare con i salvataggi
        foreach (AbilityShopEntry entry in entrys)
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
        LilithShopButton lastButton = lastSelected.GetComponent<LilithShopButton>();

        if (!lastButton.isActive)
            return;

        shopMenu.canClose = false;
        shopMenu.tableAssosiation[this].MultiplayerEventSystem.SetSelectedGameObject(buyButton.gameObject);

        shopMenu.tableAssosiation[this].GetComponent<PlayerInput>().actions.FindAction("Cancel").performed += CoinShopTable_performed;

    }

    private void CoinShopTable_performed(InputAction.CallbackContext obj)
    {
        DesetOnBuyButton();
        shopMenu.tableAssosiation[this].GetComponent<PlayerInput>().actions.FindAction("Cancel").performed -= CoinShopTable_performed;
    }

    public void BuyAbility()
    {
        LilithShopButton lastButton = lastSelected.GetComponent<LilithShopButton>();

        AbilityShopEntry lastEntry = entrys.Find(b => b.button == lastButton);

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
        shopMenu.tableAssosiation[this].MultiplayerEventSystem.SetSelectedGameObject(lastSelected);
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
