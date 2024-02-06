using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CoopManager : MonoBehaviour
{
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

    [SerializeField] private GameObject playerInputPrefab;
    [SerializeField] private CharacterClass switchPlayerUp;
    [SerializeField] private CharacterClass switchPlayerRight;
    [SerializeField] private CharacterClass switchPlayerDown;
    [SerializeField] private CharacterClass switchPlayerLeft;
    public CharacterClass SwitchPlayerUp => switchPlayerUp;
    public CharacterClass SwitchPlayerRight => switchPlayerRight;
    public CharacterClass SwitchPlayerDown => switchPlayerDown;
    public CharacterClass SwitchPlayerLeft => switchPlayerLeft;
   
    private bool canSwitchCharacter = true;

    private PlayerInputManager inputManager;
    private List<PlayerInputHandler> playerInputHandlers;

    private List<CharacterClass> internalSwitchList;

    public List<PlayerCharacter> ActivePlayers
    {
        get
        {
            List<PlayerCharacter> players = new();
            if(playerInputHandlers != null)
            {
                foreach (PlayerInputHandler player in playerInputHandlers)
                {
                    if (player.CurrentReceiver != null && player.CurrentReceiver is PlayerCharacter)
                    {
                        players.Add((PlayerCharacter)player.CurrentReceiver);
                    }
                }
            }
            return players;
        }
    }

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
        inputManager = GetComponent<PlayerInputManager>();
        InizializeSwitchList();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void InizializeSwitchList()
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

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if(playerInputHandlers == null)
            playerInputHandlers = new List<PlayerInputHandler>();

        playerInput.gameObject.transform.parent = transform;
        PlayerInputHandler newPlayerInputHandler = playerInput.gameObject.GetComponent<PlayerInputHandler>();
        if (newPlayerInputHandler != null)
        {
            playerInputHandlers.Add(newPlayerInputHandler);
            newPlayerInputHandler.SetReceiver(SceneInputReceiverManager.Instance.GetSceneInputReceiver(newPlayerInputHandler));
        }
        else
            Debug.LogError("Missing PlayerInputHandler Component");
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializePlayers();
    }


    private void InitializePlayers()
    {
        foreach (PlayerInputHandler player in playerInputHandlers)
        {
            player.SetReceiver(SceneInputReceiverManager.Instance.GetSceneInputReceiver(player));
        }
        HPHandler.Instance.SetActivePlayers();
        CameraManager.Instance.AddAllPlayers();
    }

    public void SetCanSwitch(bool canSwitch) 
    { 
        this.canSwitchCharacter = canSwitch;
    }

    internal bool CanSwitchCharacter(CharacterClass targetClass)
    {
        if (!canSwitchCharacter)
            return false;

        foreach(PlayerInputHandler player in playerInputHandlers)
        {
            if (player.currentCharacter == targetClass.Character)
                return false;
        }

        return true;
        
    }

    public CharacterClass GetCharacterClass(ePlayerCharacter character)
    {
        foreach(CharacterClass ch in internalSwitchList)
        {
            if(ch.Character == character)
                return ch;
        }
        return null;
    }
}
