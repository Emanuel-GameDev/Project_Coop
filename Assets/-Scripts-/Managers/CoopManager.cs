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

    public void JoinPlayer(int numPlayers)
    {
        for (int i = 0; i < numPlayers; i++)
        {
            GameManager.Instance.manager.JoinPlayer();
        }
    }

    public bool SwitchCharacter(Character characterToSwitch, int switchInto)
    {
        foreach (PlayerCharacter player in activePlayers)
        {
            if (player.CharacterData == internalSwitchList[switchInto])
                return false;
        }

        characterToSwitch.SetCharacterData(internalSwitchList[switchInto]);

        return true;
    }

    private void InitializeCharacterData(PlayerCharacter newCharacter)
    {
        for (int i = 0; i < internalSwitchList.Count; i++)
        {
            if (SwitchCharacter(newCharacter, i))
                return;
        }
    }

    public void AddPlayer()
    {
        PlayerCharacter[] foundPlayersComponents = FindObjectsOfType<PlayerCharacter>();

        for (int i = 0; i < foundPlayersComponents.Length; i++)
        {
            if (!activePlayers.Contains(foundPlayersComponents[i]))
            {
                activePlayers.Add(foundPlayersComponents[i]);

                if (i > 1)
                    InitializeCharacterData(foundPlayersComponents[i]);
            }
        }

    }

}
