using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoopManager : MonoBehaviour
{
    [SerializeField] private CharacterClass switchPlayerUp;
    [SerializeField] private CharacterClass switchPlayerRight;
    [SerializeField] private CharacterClass switchPlayerDown;
    [SerializeField] private CharacterClass switchPlayerLeft;
    private bool canSwitch = true;
    public List<PlayerSelection> playerInputDevices = new List<PlayerSelection>();
    public List<PlayerCharacter> activePlayers;
    private List<CharacterClass> internalSwitchList;

    private List<PlayerInput> playerList = new List<PlayerInput>();

    private static CoopManager _instance;
    public static CoopManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CoopManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("CoopManager");
                    _instance = singletonObject.AddComponent<CoopManager>();
                }
            }

            return _instance;
        }
    }

    public CharacterClass SwitchPlayerUp => switchPlayerUp;
    public CharacterClass SwitchPlayerRight => switchPlayerRight;
    public CharacterClass SwitchPlayerDown => switchPlayerDown;
    public CharacterClass SwitchPlayerLeft => switchPlayerLeft;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    private void Start()
    {
        internalSwitchList = new List<CharacterClass>()
        {
            SwitchPlayerUp,
            SwitchPlayerRight,
            SwitchPlayerDown,
            SwitchPlayerLeft
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
            GameManager.Instance.playerInputManager.JoinPlayer();
        }
    }

    
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        playerList.Add(playerInput);
        GameManager.Instance.PlayerIsJoined(playerInput);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        playerList.Remove(playerInput);
        GameManager.Instance.PlayerAsLeft(playerInput);
    }

    public void SetCanSwitch(bool canSwitch) 
    { 
        this.canSwitch = canSwitch;
    }

    /// <summary>
    /// Prende dal character selection menù la lista dei player selection e aggiorna i player attivi
    /// </summary>
    public void UpdateSelectedPalyers(List<PlayerSelection> list) 
    {
        playerInputDevices.Clear(); 

        foreach (PlayerSelection player in list) 
        {
            if (player.selected)
                playerInputDevices.Add(player);
        }
    }

    //quando si istanzia il prefab di un giocatore se è un player character va buttato nella lista activePlayers, da valutare se spostare la lista nel GameManager

    internal bool CanSwitchCharacter(CharacterClass switchPlayerUp, PlayerCharacter playerCharacter)
    {
        if (!canSwitch)
            return false;
        
        PlayerSelection toSwtich = new();
        
        foreach(PlayerSelection player in playerInputDevices)
        {
            if (player.data._class == switchPlayerUp)
                return false;
            if (player.data._class == playerCharacter.CharacterClass)
                toSwtich = player;
        }
        toSwtich.data._class = switchPlayerUp;

        return true;
    }
}
