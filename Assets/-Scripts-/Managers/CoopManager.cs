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

    public InputActionMap playerActionMap { get; private set; }
    public InputActionMap uiActionMap { get; private set; }

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

        PlayerInputManager manager = GetComponent<PlayerInputManager>();
        playerActionMap = manager.playerPrefab.GetComponent<PlayerInput>().actions.FindActionMap("Player");
        uiActionMap = manager.playerPrefab.GetComponent<PlayerInput>().actions.FindActionMap("UI");

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
