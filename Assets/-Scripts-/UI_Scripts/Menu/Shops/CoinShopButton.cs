using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CoinShopButton : Button
{
    //CoinShopMenu shopMenu;
    CoinShopTable shopTable;
    [SerializeField] Image buttonImage;
    [SerializeField] TextMeshProUGUI coinCostText;
    [SerializeField] public PowerUp powerUp;

    protected override void Awake()
    {
        //shopMenu = GetComponentInParent<CoinShopMenu>();
        shopTable= GetComponentInParent<CoinShopTable>();
    }

   

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);

        if (state == SelectionState.Selected)
        {
            ChangeDescription();
        }
    }

    private void ChangeDescription()
    {
        shopTable.ChangeDescriptionAndName(powerUp.powerUpName, powerUp.powerUpDescription);
        
    }

    public void SetPowerUp(PowerUp powerUpToSet)
    {
        powerUp=powerUpToSet;
        buttonImage.sprite = powerUp.powerUpSprite;
        coinCostText.text = powerUp.moneyCost.ToString();
    }
}
