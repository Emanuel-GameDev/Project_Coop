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

    [HideInInspector] public bool isActive = true;

    protected override void Awake()
    {
        //shopMenu = GetComponentInParent<CoinShopMenu>();
        shopTable= GetComponentInParent<LilithShopTable>();
        //buttonImage = GetComponent<Image>();
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
        KeyRequiredCheck();
    }

    private void ChangeDescription()
    {
        if(ability != null)
           shopTable.ChangeDescriptionAndName(ability.abilityName, ability.abilityDescription);
    }

    public void ActivateButton()
    {
        isActive = true;

        foreach (Image i in  GetComponentsInChildren<Image>())
        {
            i.color = Color.white;
        }
    }

    public void DeactivateButton()
    {
        isActive = false;
        foreach(Image i in GetComponentsInChildren<Image>())
        {
            i.color = Color.gray;
            Debug.Log("grey");
        }

    }

    public void KeyRequiredCheck()
    {
        if (shopTable.playerCharacterReference.ExtraData.unusedKey < ability.keyCost)
            DeactivateButton();
        else
            ActivateButton();
    }


}
