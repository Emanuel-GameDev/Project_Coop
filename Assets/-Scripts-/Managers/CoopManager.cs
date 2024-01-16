using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoopManager : MonoBehaviour
{
    [SerializeField] private CharacterClass switchPlayerUp;
    [SerializeField] private CharacterClass switchPlayerRight;
    [SerializeField] private CharacterClass switchPlayerDown;
    [SerializeField] private CharacterClass switchPlayerLeft;

    public List<PlayerCharacter> activePlayers = new List<PlayerCharacter>();
    private List<CharacterClass> internalSwitchList;

    private List<PlayerInput> playerList = new List<PlayerInput>();

    private void Start()
    {
        internalSwitchList = new List<CharacterClass>()
        {
            switchPlayerUp,
            switchPlayerRight,
            switchPlayerDown,
            switchPlayerLeft
        };

        foreach (CharacterClass ch in internalSwitchList)
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

    public void SwitchCharacter(PlayerCharacter characterToSwitch, int switchInto)
    {
        foreach (PlayerCharacter player in activePlayers)
        {
            if (player.CharacterClass == internalSwitchList[switchInto])
                return;
        }

        characterToSwitch.SwitchCharacterClass(internalSwitchList[switchInto]);
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        playerList.Add(playerInput);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {

    }

}
