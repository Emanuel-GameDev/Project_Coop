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
    public List<PlayerCharacter> activePlayers = new List<PlayerCharacter>();
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
            GameManager.Instance.playerInputManager.JoinPlayer();
        }
    }

    public void SwitchCharacter(PlayerCharacter characterToSwitch, int switchInto)
    {
        if (!canSwitch)
            return;
        
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

}
