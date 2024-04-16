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
        if (context.performed)
            MinigameMenuManager.Instance.MenuButton(playerInputHandler.playerID);
    }

    public override void Navigate(InputAction.CallbackContext context)
    {
        if (context.performed)
            MinigameMenuManager.Instance.NavigateButton(context.ReadValue<Vector2>(), playerInputHandler.playerID);
    }

    public override void Submit(InputAction.CallbackContext context)
    {
        if (context.performed)
            MinigameMenuManager.Instance.SubmitButton(playerInputHandler.playerID);
    }

    public override void Cancel(InputAction.CallbackContext context)
    {
        if (context.performed)
            MinigameMenuManager.Instance.CancelButton(playerInputHandler.playerID);
    }


    public override void ButtonSouth(InputAction.CallbackContext context)
    {
        if(slotmachine.inGame)
        {
            slotmachine.InputFromPlayer(this);
        }
        

        
    }


}
