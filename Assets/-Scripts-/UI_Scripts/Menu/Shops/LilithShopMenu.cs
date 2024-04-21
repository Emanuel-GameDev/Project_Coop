using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
        if (shopGroup.activeSelf) return;
        base.OpenMenu();
        
        //tableAssosiation.Clear();

        
        foreach(LilithShopTable table in GetComponentsInChildren<LilithShopTable>(true))
        {
            //da sistemare
            table.InitializeButtons();
            //tableAssosiation.Add(table, CoopManager.Instance.GetPlayer(ePlayerID.Player1));
            table.UpdateKeyCounter(PlayerCharacterPoolManager.Instance.AllPlayerCharacters.Find(c => c.Character == table.characterReference).ExtraData.key);
            table.StartIdleAnimationIn(Random.value);
        }

        shopGroup.GetComponent<Animation>().Play("LilithShopEntrance");
    }

    public override void CloseMenu()
    {
        foreach (PlayerInputHandler ih in CoopManager.Instance.GetComponentsInChildren<PlayerInputHandler>())
        {
            InputActionAsset actions = ih.GetComponent<PlayerInput>().actions;

            actions.FindActionMap("Player").Enable();

            actions.FindActionMap("UI").Disable();
            actions.FindActionMap("UI").FindAction("Menu").Enable();

            actions.FindAction("Cancel").performed -= Menu_performed;
        }

        shopGroup.GetComponent<Animation>().Play("LilithShopExit");
        StartCoroutine(CloseMenuWithDelay(shopGroup.GetComponent<Animation>().clip.length));
        
    }

    IEnumerator CloseMenuWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        shopGroup.SetActive(false);
    }
}
