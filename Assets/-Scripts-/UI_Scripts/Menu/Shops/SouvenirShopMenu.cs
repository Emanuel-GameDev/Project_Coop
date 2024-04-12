using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

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
            shopGroup.SetActive(true);

            PlayerInputHandler ih = interacter.GetInteracterObject().GetComponent<PlayerCharacter>().GetInputHandler();
                //ih.SetPlayerActiveMenu(tables.gameObject, table[i].GetComponentInChildren<Selectable>().gameObject);

                ih.MultiplayerEventSystem.SetSelectedGameObject(firstSelected.GetComponentInChildren<Selectable>().gameObject);
                InputActionAsset actions = ih.GetComponent<PlayerInput>().actions;

                //actions.Disable();
                actions.FindActionMap("Player").Disable();
                actions.FindActionMap("UI").Enable();

                actions.FindAction("Menu").performed += Menu_performed;

            

            foreach (SouvenirShopTable table in shopTables)
            {
                table.SetTableCurrentCharacter(playerInShop);
            }
        }
    }


  
}
