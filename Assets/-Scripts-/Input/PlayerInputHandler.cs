using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;
    public PlayerInput PlayerInput => playerInput;

    public ePlayerID playerID { private set; get; } = ePlayerID.NotSet;

    public ePlayerCharacter startingCharacter { private set; get; } = ePlayerCharacter.EmptyCharacter;
    public ePlayerCharacter currentCharacter { private set; get; }

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

    public void SetActionMap(eInputMap map)
    {
        playerInput.SwitchCurrentActionMap(map.ToString());
    }

    public void SetReceiver(InputReceiver newReceiver)
    {
        CurrentReceiver = newReceiver;
        SetActionMap(SceneInputReceiverManager.Instance.GetSceneActionMap());
        if (currentCharacter != ePlayerCharacter.EmptyCharacter)
            CurrentReceiver.SetCharacter(currentCharacter);

    }

    public void SetCharacter(ePlayerCharacter character)
    {
        currentCharacter = character;
    }

   
    public bool SetStartingCharacter(ePlayerCharacter character) 
    {
        if (startingCharacter == ePlayerCharacter.EmptyCharacter)
        {
            startingCharacter = character;
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
        foreach(ePlayerID ID in Enum.GetValues(typeof(ePlayerID)))
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

    public void OnDeviceLost(PlayerInput playerInput)
    {
        CoopManager.Instance.OnDeviceLost(playerInput);
    }

    public void OnDeviceRegained(PlayerInput playerInput)
    {
        CoopManager.Instance.OnDeviceRegained(playerInput);
    }

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

    public void JoinInput(InputAction.CallbackContext context) => CurrentReceiver.JoinInput(context);

    public void SwitchUpInput(InputAction.CallbackContext context) => CurrentReceiver.SwitchUpInput(context);

    public void SwitchRightInput(InputAction.CallbackContext context) => CurrentReceiver.SwitchRightInput(context);

    public void SwitchDownInput(InputAction.CallbackContext context) => CurrentReceiver.SwitchDownInput(context);

    public void SwitchLeftInput(InputAction.CallbackContext context) => CurrentReceiver.SwitchLeftInput(context);

    public void MenuInput(InputAction.CallbackContext context) => CurrentReceiver.MenuInput(context);

    public void OptionInput(InputAction.CallbackContext context) => CurrentReceiver.OptionInput(context);


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

    
    #endregion

    #endregion
}


public enum ePlayerCharacter
{
    EmptyCharacter,
    Brutus,
    Caina,
    Cassius,
    Jude
}

public enum ePlayerID
{
    NotSet,
    Player1,
    Player2,
    Player3,
    Player4
}
