using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.InputSystem;

public class SmashPlayer : InputReceiver
{
    [SerializeField] SmashMinigameManager SMManager;
    internal int smashCount = 0;
    internal bool canCount = false;

    private void Awake()
    {
        SMManager = FindAnyObjectByType<SmashMinigameManager>();
        SMManager.listOfCurrentPlayer.Add(this);

    }

    private void Start()
    {
        ResetCounter();
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
        if (canCount && context.performed)
        {
            smashCount++;
        }



    }

    public void ResetCounter()
    {
        smashCount = 0;
        canCount = false;
    }
}
