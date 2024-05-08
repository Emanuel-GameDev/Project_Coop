using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LilithShopMenu : Menu
{
    [Header("Dialogues")]
    [SerializeField] DialogueBox dialogueBox;
    [SerializeField] Dialogue FirstTimeDialogue;
    [SerializeField] Dialogue beforeEnterDialogue;
    //[HideInInspector] public Dictionary<LilithShopTable, PlayerInputHandler> tableAssosiation = new();

    public override void Start()
    {
        base.Start();
    }

    bool firstTime=true;

    public override void OpenMenu()
    {
        if (shopGroup.activeSelf) return;
        //base.OpenMenu();

        //tableAssosiation.Clear();

        //int i = 0;
        //canClose = true;

        //shopGroup.SetActive(true);

        //foreach (PlayerCharacter pc in PlayerCharacterPoolManager.Instance.ActivePlayerCharacters)
        //{

        //    pc.GetInputHandler().SetPlayerActiveMenu(gameObject, table[i].GetComponentInChildren<Selectable>().gameObject);

        //    InputActionAsset actions = pc.GetInputHandler().PlayerInput.actions;
            

        //    actions.FindActionMap("Player").Disable();
        //    actions.FindActionMap("UI").Enable();
        //    actions.FindActionMap("UI").FindAction("Menu").Disable();
        //    actions.FindAction("Cancel").performed += Menu_performed;
        //    if (actions.FindActionMap("UI").FindAction("Menu").enabled)
        //        Debug.Log("true");

        //    i++;
        //}



        
      

        if (dialogueBox != null)
        {
            if (firstTime)
            {
                dialogueBox.SetDialogue(FirstTimeDialogue);
                firstTime=false;
            }
            else
                dialogueBox.SetDialogue(beforeEnterDialogue);

            dialogueBox.RemoveAllDialogueEnd();

            dialogueBox.OnDialogueEnded += OpenAnimationEvent;

            dialogueBox.StartDialogue();

            
        }
        else
            OpenAnimationEvent();
    }


    private void OpenAnimationEvent()
    {
        dialogueBox.OnDialogueEnded -= OpenAnimationEvent;
        OpenAnimation();
    }

    private void OpenAnimation()
    {
        closeQueueInt = 0;
        int i = 0;
        canClose = true;

        shopGroup.SetActive(true);

        foreach (PlayerInputHandler ih in CoopManager.Instance.GetActiveHandlers())
        {

            //ih.SetPlayerActiveMenu(tables.gameObject, table[i].GetComponentInChildren<Selectable>().gameObject);

            //ih.MultiplayerEventSystem.SetSelectedGameObject(table[i].GetComponentInChildren<Selectable>().gameObject);
            InputActionAsset actions = ih.GetComponent<PlayerInput>().actions;

            actions.FindActionMap("Player").Disable();
            actions.FindActionMap("UI").Enable();
            actions.FindActionMap("UI").FindAction("Menu").Disable();
            actions.FindAction("Cancel").performed += Menu_performed;

            i++;
        }

        foreach (LilithShopTable table in GetComponentsInChildren<LilithShopTable>(true))
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
        shopGroup.GetComponent<Animation>().Play("LilithShopExit");
        StartCoroutine(CloseMenuWithDelay(shopGroup.GetComponent<Animation>().clip.length));
        SaveManager.Instance.SavePlayersData();
        
    }

    IEnumerator CloseMenuWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (PlayerInputHandler ih in CoopManager.Instance.GetComponentsInChildren<PlayerInputHandler>())
        {
            InputActionAsset actions = ih.GetComponent<PlayerInput>().actions;

            actions.FindActionMap("Player").Enable();

            actions.FindActionMap("UI").Disable();
            actions.FindActionMap("UI").FindAction("Menu").Enable();

            actions.FindAction("Cancel").performed -= Menu_performed;
        }

        foreach (PlayerCharacter pc in PlayerCharacterPoolManager.Instance.ActivePlayerCharacters)
        {
            gameObject.GetComponentInParent<PressInteractable>().CancelInteraction(pc);
        }
        shopGroup.SetActive(false);
    }
}
