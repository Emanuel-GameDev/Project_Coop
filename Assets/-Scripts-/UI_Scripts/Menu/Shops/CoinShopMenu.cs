using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class CoinShopMenu : Menu
{
    [HideInInspector] public Dictionary<CoinShopTable, PlayerInputHandler> tableAssosiation = new();

    public override void Start()
    {
        base.Start();
    }

    public override void OpenMenu()
    {
        

        base.OpenMenu();

        tableAssosiation.Clear();

        //CoinShopTable[] tables = GetComponentsInChildren<CoinShopTable>();

        //for (int i = 0; i < GetComponentsInChildren<CoinShopTable>().Length; i++) 
        //{ 
        //    tableAssosiation.Add()
        //}

        for(int i = 0; i < 4; i++) { }

        foreach (PlayerInputHandler ih in CoopManager.Instance.GetComponentsInChildren<PlayerInputHandler>())
        {
            tableAssosiation.Add(GetComponentInChildren<CoinShopTable>(true),ih);
            
        }
    }
}
