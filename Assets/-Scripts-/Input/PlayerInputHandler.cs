using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;

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

    public void SetReceiver(InputReceiver newReceiver)
    {
        CurrentReceiver = newReceiver;
        playerInput.SwitchCurrentActionMap(SceneInputReceiverManager.Instance.GetSceneActionMap());
        if(currentCharacter != ePlayerCharacter.EmptyCharacter)
            CurrentReceiver.SetCharacter(currentCharacter);
    }

    public void SetCharacter(ePlayerCharacter character)
    {
        currentCharacter = character;
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