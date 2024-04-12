using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class SouvenirShopMenu : Menu
{
    public PlayerCharacter currentPlayerInShop;

    [SerializeField] public SouvenirShopTable[] shopTables = new SouvenirShopTable[4];



    //[Serializable]
    //public class SouvenirShopEntry
    //{
    //    [SerializeField] public SouvenirShopTable table;
    //    [SerializeField] public PowerUp[] souvenirs = new PowerUp[2];
    //    [HideInInspector] public int souvenirID;
    //}

    public void OpenMenu(IInteracter interacter)
    {
        if(interacter.GetInteracterObject().TryGetComponent<PlayerCharacter>(out PlayerCharacter playerInShop))
        {
            base.OpenMenu();

            foreach (SouvenirShopTable table in shopTables)
            {
                table.SetTableCurrentCharacter(playerInShop);
            }
        }
    }


  
}
