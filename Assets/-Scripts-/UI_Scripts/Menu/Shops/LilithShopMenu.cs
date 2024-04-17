using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class LilithShopMenu : Menu
{
    //[HideInInspector] public Dictionary<LilithShopTable, PlayerInputHandler> tableAssosiation = new();

    public override void Start()
    {
        base.Start();
    }

    public override void OpenMenu()
    {
        base.OpenMenu();
        
        //tableAssosiation.Clear();

        
        foreach(LilithShopTable table in GetComponentsInChildren<LilithShopTable>(true))
        {
            //da sistemare
            table.InitializeButtons();
            //tableAssosiation.Add(table, CoopManager.Instance.GetPlayer(ePlayerID.Player1));
            table.UpdateKeyCounter(PlayerCharacterPoolManager.Instance.AllPlayerCharacters.Find(c => c.Character == table.characterReference).ExtraData.unusedKey);
        }
    }
}
