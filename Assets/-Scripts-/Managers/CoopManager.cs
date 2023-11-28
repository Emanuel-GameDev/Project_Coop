using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoopManager : MonoBehaviour
{
    [SerializeField] private CharacterData switchPlayerUp;
    [SerializeField] private CharacterData switchPlayerRight;
    [SerializeField] private CharacterData switchPlayerDown;
    [SerializeField] private CharacterData switchPlayerLeft;

    private List<PlayerCharacter> activePlayers = new List<PlayerCharacter>();
    private List<CharacterData> internalSwitchList;

    public InputActionMap playerActionMap { get; private set; }
    public InputActionMap uiActionMap { get; private set; }

    private void Start()
    {
        internalSwitchList = new List<CharacterData>()
        {
            switchPlayerUp,
            switchPlayerRight,
            switchPlayerDown,
            switchPlayerLeft
        };

        foreach (CharacterData ch in internalSwitchList)
        {
            if (ch == null)
            {
                Debug.LogError("Missing player to switch Reference. PlayerManager/CoopManager");
                return;
            }
        }

        PlayerInputManager manager = GetComponent<PlayerInputManager>();
        playerActionMap = manager.playerPrefab.GetComponent<PlayerInput>().actions.FindActionMap("Player");
        uiActionMap = manager.playerPrefab.GetComponent<PlayerInput>().actions.FindActionMap("UI");

    }



    public void SwitchCharacter(Character characterToSwitch, int switchInto)
    {
        foreach (PlayerCharacter player in activePlayers)
        {
            if (player.CharacterData == internalSwitchList[switchInto])
                return;
        }

        characterToSwitch.SetCharacterData(internalSwitchList[switchInto]);
    }

    public void AddPlayer()
    {
        PlayerCharacter[] foundPlayersComponents = FindObjectsOfType<PlayerCharacter>();

        foreach (PlayerCharacter p in foundPlayersComponents)
        {
            if (!activePlayers.Contains(p))
                activePlayers.Add(p);
        }
        
    }

}
