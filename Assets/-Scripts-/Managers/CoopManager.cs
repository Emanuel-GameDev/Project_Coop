using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CoopManager : MonoBehaviour
{
    [SerializeField] private CharacterClass switchPlayerUp;
    [SerializeField] private CharacterClass switchPlayerRight;
    [SerializeField] private CharacterClass switchPlayerDown;
    [SerializeField] private CharacterClass switchPlayerLeft;


    private bool canSwitch = true;

    // Lista dei dispositivi degli utenti collegati
    public List<PlayerSelection> playerInputDevices = new List<PlayerSelection>();
    // Lista dei personaggi attivi
    public List<PlayerCharacter> activePlayers;


    private List<CharacterClass> internalSwitchList;

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

    public GameObject capsulePrefab;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializePlayers();
    }


    private void InitializePlayers()
    {
        activePlayers.Clear();
        foreach (PlayerSelection p in playerInputDevices)
        {
            PlayerInput GO = PlayerInput.Instantiate(capsulePrefab, p.device.deviceId, p.device.displayName, -1, p.device);

            PlayerCharacter playerCharacter = GO.gameObject.GetComponent<PlayerCharacter>();
           
            playerCharacter.SwitchCharacterClass(p.data._class);
            activePlayers.Add(playerCharacter);
        }

        HPHandler.Instance.SetActivePlayers();
        CameraManager.Instance.AddAllPlayers();
    }

    public void SetCanSwitch(bool canSwitch) 
    { 
        this.canSwitch = canSwitch;
    }

    /// <summary>
    /// Prende dal character selection menù la lista dei player selection e aggiorna i player attivi
    /// </summary>
    public void UpdateSelectedPlayers(List<PlayerSelection> list) 
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

        PlayerSelection toSwtich = null;

        foreach (PlayerSelection player in playerInputDevices)
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
