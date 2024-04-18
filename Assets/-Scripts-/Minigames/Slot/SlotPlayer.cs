using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlotPlayer : InputReceiver
{
    [SerializeField] Slotmachine slotmachine;

    private void Awake()
    {
        slotmachine = FindAnyObjectByType<Slotmachine>();
        slotmachine.listOfCurrentPlayer.Add(this);

    }

    private void Start()
    {
        
    }

    public override void MenuInput(InputAction.CallbackContext context)
    {
        if (context.performed && MenuManager.Instance.ActualMenu == null)
            MenuManager.Instance.OpenPauseMenu(playerInputHandler);
    }

    public override void OptionInput(InputAction.CallbackContext context)
    {
        if (context.performed && MenuManager.Instance.ActualMenu == null)
            MenuManager.Instance.OpenOptionMenu(playerInputHandler);
    }

    public override void UIMenuInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            MenuManager.Instance.ClosePauseMenu(playerInputHandler);
    }

    public override void UIOptionInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            MenuManager.Instance.CloseOptionMenu(playerInputHandler);
    }

    public override void ButtonSouth(InputAction.CallbackContext context)
    {
        if(slotmachine.inGame)
        {
            slotmachine.InputFromPlayer(this);
        }
        

        
    }


}
