using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;
    public PlayerInput PlayerInput => playerInput;

    [SerializeField]
    private MultiplayerEventSystem multiplayerEventSystem;
    public MultiplayerEventSystem MultiplayerEventSystem => multiplayerEventSystem;

    public ePlayerID playerID { private set; get; } = ePlayerID.NotSet;

    public ePlayerCharacter startingCharacter { private set; get; } = ePlayerCharacter.EmptyCharacter;
    public ePlayerCharacter currentCharacter { private set; get; }

    private InputMap previousInputMap = InputMap.EmptyMap;
    private InputMap currentInputMap = InputMap.EmptyMap;

    public InputReceiver _currentReceiver;
    public InputReceiver CurrentReceiver
    {
        get
        {
            if (_currentReceiver == null)
            {
                _currentReceiver = SceneInputReceiverManager.Instance.GetSceneInputReceiver(this);
            }

            return _currentReceiver;
        }
        private set { _currentReceiver = value; }
    }

    private void Awake()
    {
        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();
    }

    public void SetReceiver(InputReceiver newReceiver)
    {
        CurrentReceiver = newReceiver;
        if(!GameManager.Instance.IsLoading)
            SetActionMap(SceneInputReceiverManager.Instance.GetSceneActionMap());
        CurrentReceiver.SetInputHandler(this);
        if (currentCharacter != ePlayerCharacter.EmptyCharacter)
            CurrentReceiver.SetCharacter(currentCharacter);
    }

    public void SetCurrentCharacter(ePlayerCharacter character)
    {
        currentCharacter = character;
    }

    public bool SetStartingCharacter(ePlayerCharacter character)
    {
        if (startingCharacter == ePlayerCharacter.EmptyCharacter)
        {
            startingCharacter = character;
            currentCharacter = character;
            return true;
        }
        else
            return false;
    }

    public Color GetPlayerColor()
    {
        return GameManager.Instance.GetCharacterData(startingCharacter).CharacterColor;
    }

    public void SetPlayerID(List<PlayerInputHandler> playerInputHandlers)
    {
        foreach (ePlayerID ID in Enum.GetValues(typeof(ePlayerID)))
        {
            if (ID != ePlayerID.NotSet)
            {
                bool found = false;
                foreach (PlayerInputHandler playerInputHandler in playerInputHandlers)
                {
                    if (playerInputHandler.playerID == ID)
                        found = true;
                }

                if (!found)
                {
                    playerID = ID;
                    return;
                }
            }
        }
    }

    #region MultiplayerMenu
    public void SetPlayerActiveMenu(GameObject menuRoot, GameObject firstSelection)
    {
        MultiplayerEventSystem.firstSelectedGameObject = firstSelection;
        MultiplayerEventSystem.playerRoot = menuRoot;

        if (firstSelection != null)
            multiplayerEventSystem.SetSelectedGameObject(firstSelection);

        if (menuRoot != null)
        {
            SetActionMap(InputMap.UI);
        }
        else
        {
            SetActionMap(SceneInputReceiverManager.Instance.GetSceneActionMap());
        }


        Debug.Log(playerInput.currentActionMap);
    }
    #endregion

    #region PlayerInput
    public void SetActionMap(InputMap map)
    {
        playerInput.SwitchCurrentActionMap(map.ToString());
        currentInputMap = map;
    }

    public void OnDeviceLost(PlayerInput playerInput)
    {
        CoopManager.Instance.OnDeviceLost(playerInput);
    }

    public void OnDeviceRegained(PlayerInput playerInput)
    {
        CoopManager.Instance.OnDeviceRegained(playerInput);
    }
    #endregion

    // La lista di tutti gli input di tutte le possibili mappe 
    #region MapInput

    #region Player

    public void MoveInput(InputAction.CallbackContext context) => CurrentReceiver.MoveInput(context);

    public void LookInput(InputAction.CallbackContext context) => CurrentReceiver.LookInput(context);

    public void AttackInput(InputAction.CallbackContext context) => CurrentReceiver.AttackInput(context);

    public void DefenseInput(InputAction.CallbackContext context) => CurrentReceiver.DefenseInput(context);

    public void UniqueAbilityInput(InputAction.CallbackContext context) => CurrentReceiver.UniqueAbilityInput(context);

    public void ExtraAbilityInput(InputAction.CallbackContext context) => CurrentReceiver.ExtraAbilityInput(context);

    public void InteractInput(InputAction.CallbackContext context) => CurrentReceiver.InteractInput(context);

    public void CancelInteractInput(InputAction.CallbackContext context) => CurrentReceiver.CancelInteractInput(context);


    public void SwitchUpInput(InputAction.CallbackContext context) => CurrentReceiver.SwitchUpInput(context);

    public void SwitchRightInput(InputAction.CallbackContext context) => CurrentReceiver.SwitchRightInput(context);

    public void SwitchDownInput(InputAction.CallbackContext context) => CurrentReceiver.SwitchDownInput(context);

    public void SwitchLeftInput(InputAction.CallbackContext context) => CurrentReceiver.SwitchLeftInput(context);

    public void MenuInput(InputAction.CallbackContext context) => CurrentReceiver.MenuInput(context);
    //{
    //    if (context.started) Utility.DebugTrace("STARTED");
    //    if (context.performed) Utility.DebugTrace("PERFORMED");
    //    if (context.canceled) Utility.DebugTrace("CANCELED");


    //    CurrentReceiver.MenuInput(context);
    //} 

    public void OptionInput(InputAction.CallbackContext context) => CurrentReceiver.OptionInput(context);

    public void MouseLookInput(InputAction.CallbackContext context) => CurrentReceiver.MouseLookInput(context);


    #endregion

    #region Minigame

    public void MoveMinigameInput(InputAction.CallbackContext context) => CurrentReceiver.MoveMinigameInput(context);

    public void ButtonEast(InputAction.CallbackContext context) => CurrentReceiver.ButtonEast(context);

    public void ButtonNorth(InputAction.CallbackContext context) => CurrentReceiver.ButtonNorth(context);

    public void ButtonWeast(InputAction.CallbackContext context) => CurrentReceiver.ButtonWeast(context);

    public void ButtonSouth(InputAction.CallbackContext context) => CurrentReceiver.ButtonSouth(context);
    #endregion


    #region UI

    public virtual void Navigate(InputAction.CallbackContext context) => CurrentReceiver.Navigate(context);

    public virtual void Submit(InputAction.CallbackContext context) => CurrentReceiver.Submit(context);

    public virtual void RandomSelection(InputAction.CallbackContext context) => CurrentReceiver.RandomSelection(context);

    public virtual void Cancel(InputAction.CallbackContext context) => CurrentReceiver.Cancel(context);

    public virtual void ScrollWheel(InputAction.CallbackContext context) => CurrentReceiver.ScrollWheel(context);

    public void UIMenuInput(InputAction.CallbackContext context) => CurrentReceiver.UIMenuInput(context);

    public void UIOptionInput(InputAction.CallbackContext context) => CurrentReceiver.UIOptionInput(context);

    public virtual void NextInput(InputAction.CallbackContext context) => CurrentReceiver.NextInput(context);

    public virtual void PreviousInput(InputAction.CallbackContext context) => CurrentReceiver.PreviousInput(context);

    public virtual void SubNextInput(InputAction.CallbackContext context) => CurrentReceiver.SubNextInput(context);

    public virtual void SubPreviousInput(InputAction.CallbackContext context) => CurrentReceiver.SubPreviousInput(context);
    
    public virtual void ChangeVisualizationInput(InputAction.CallbackContext context) => CurrentReceiver.ChangeVisualizationInput(context);

    #endregion

    #endregion
}

