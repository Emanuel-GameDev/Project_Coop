using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoopManager : MonoBehaviour
{
    [SerializeField] private CharacterData switchPlayerUp;
    [SerializeField] private CharacterData switchPlayerRight;
    [SerializeField] private CharacterData switchPlayerDown;
    [SerializeField] private CharacterData switchPlayerLeft;

    public List<PlayerCharacter> activePlayers = new List<PlayerCharacter>();
    private List<CharacterData> internalSwitchList;

    private List<PlayerInput> playerList = new List<PlayerInput>();

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

    }

    public void JoinPlayer(int numPlayers)
    {
        for (int i = 0; i < numPlayers; i++)
        {
            GameManager.Instance.manager.JoinPlayer();
        }
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

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        playerList.Add(playerInput);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {

    }

}
