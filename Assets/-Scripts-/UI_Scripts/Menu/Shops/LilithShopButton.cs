using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LilithShopButton : Button
{
    //CoinShopMenu shopMenu;
    LilithShopTable shopTable;
    [SerializeField] Image buttonImage;
    [SerializeField] TextMeshProUGUI coinCostText;
    [SerializeField] public PlayerAbility ability;

    protected override void Awake()
    {
        //shopMenu = GetComponentInParent<CoinShopMenu>();
        shopTable= GetComponentInParent<LilithShopTable>();
    }


    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);

        if (state == SelectionState.Selected)
        {
            ChangeDescription();
        }
    }

    internal void SetAbility(PlayerAbility playerAbility)
    {
        ability = playerAbility;
        buttonImage.sprite = playerAbility.abilitySprite;
        coinCostText.text = playerAbility.keyCost.ToString();
    }

    private void ChangeDescription()
    {
        //if(!string.IsNullOrEmpty(ability.abilityName.GetLocalizedString()) && !string.IsNullOrEmpty(ability.abilityDescription.GetLocalizedString()))
        //    shopTable.ChangeDescriptionAndName(ability.abilityName, ability.abilityDescription);
    }


}
