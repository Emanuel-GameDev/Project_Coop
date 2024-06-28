using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SmashPlayer : InputReceiver
{
    SmashMinigameManager SMManager;
    [SerializeField] internal Animator animator;
    [SerializeField] internal TextMeshProUGUI countText;

    internal int smashCount = 0;
    internal bool canCount = false;

    

    private void Awake()
    {
        SMManager = SmashMinigameManager.Instance;
        transform.SetParent(SMManager.spawnPoints.transform);

        SMManager.listOfCurrentPlayer.Add(this);


        if (character == ePlayerCharacter.EmptyCharacter)
            character = ePlayerCharacter.Brutus;
    }

    private void Start()
    {
        animator.runtimeAnimatorController = GameManager.Instance.GetCharacterData(character).SmashMinigameAnimator;
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

        countText.text = "0";
        countText.gameObject.SetActive(false);
        smashCount = 0;
        canCount = false;
    }

   
}
