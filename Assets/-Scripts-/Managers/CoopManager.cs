using System;
using System.Collections;
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

    [SerializeField] private bool playerCanJoin = true;
    [SerializeField] private float DelayBeforeRemoveDisconnectedPlayers = 30f;
    [SerializeField] private GameObject playerInputPrefab;
   
    private PlayerInputManager inputManager;
    private List<PlayerInputHandler> playerInputHandlers;

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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //InitializePlayers();
        
    }

    public List<PlayerInputHandler> GetActiveHandlers()
    {
        return playerInputHandlers;
    }

    public PlayerInputHandler GetPlayer(ePlayerID playerID)
    {
        return playerInputHandlers.Find(player => player.playerID == playerID);
    }

    #region Player Management

    public void SetPlayerCanJoin(bool canJoin)
    {
        playerCanJoin = canJoin;
        if (playerCanJoin)
        {
            inputManager.EnableJoining();
        }
        else
        {
            inputManager.DisableJoining();
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if(playerInputHandlers == null)
            playerInputHandlers = new List<PlayerInputHandler>();

        playerInput.gameObject.transform.SetParent(transform);
        PlayerInputHandler newPlayerInputHandler = playerInput.gameObject.GetComponent<PlayerInputHandler>();
        if (newPlayerInputHandler != null)
        {
            playerInputHandlers.Add(newPlayerInputHandler);
            newPlayerInputHandler.SetPlayerID(playerInputHandlers);
            newPlayerInputHandler.SetReceiver(SceneInputReceiverManager.Instance.GetSceneInputReceiver(newPlayerInputHandler));
        }
        else
            Debug.LogError("Missing PlayerInputHandler Component");
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        PlayerInputHandler leftingPlayerInputHandler = playerInput.gameObject.GetComponent<PlayerInputHandler>();
        if (leftingPlayerInputHandler != null)
        {
            leftingPlayerInputHandler.CurrentReceiver.Dismiss();
            playerInputHandlers.Remove(leftingPlayerInputHandler);
            Destroy(leftingPlayerInputHandler.gameObject);
        }

    }

    #endregion  

    public void InitializePlayers()
    {
        if (playerInputHandlers == null ||
            playerInputHandlers.Count == 0) return;

        foreach (PlayerInputHandler player in playerInputHandlers)
        {
            InputReceiver pReceiver = SceneInputReceiverManager.Instance.GetSceneInputReceiver(player);
            player.SetReceiver(pReceiver);
        }

        CameraManager.Instance.AddAllPlayers();
        HPHandler.Instance.SetActivePlayers();
    }

    public void OnDeviceLost(PlayerInput playerInput)
    {
        StartCoroutine(DisconnectPlayer(playerInput));
    }

    public void OnDeviceRegained(PlayerInput playerInput)
    {
        StopCoroutine(DisconnectPlayer(playerInput));
    }

    IEnumerator DisconnectPlayer(PlayerInput playerInput)
    {
        yield return new WaitForSeconds(DelayBeforeRemoveDisconnectedPlayers);
        OnPlayerLeft(playerInput);
    }

}
