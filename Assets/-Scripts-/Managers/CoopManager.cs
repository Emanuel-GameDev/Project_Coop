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

    [SerializeField, Tooltip("Imposta se i giocatori possono aggiungersi o meno")] 
    private bool playerCanJoin = true;
    [SerializeField, Tooltip("Imposta quanti secondi possono passare prima che dalla disconnessione del controller venga rimosso il player")] 
    private float DelayBeforeRemoveDisconnectedPlayers = 30f;
    [SerializeField,Tooltip("Imposta il prefab del PlayerInput")] 
    private GameObject playerInputPrefab;
   
    private PlayerInputManager inputManager;
    private List<PlayerInputHandler> playerInputHandlers;

    public List<ePlayerCharacter> ActiveEPlayerCharacters
    {
        get
        {
            List<ePlayerCharacter> players = new();
            if (playerInputHandlers != null)
            {
                foreach (PlayerInputHandler player in playerInputHandlers)
                {
                    if(player.currentCharacter != ePlayerCharacter.EmptyCharacter)
                        players.Add(player.currentCharacter);
                }
            }
            return players;
        }
    }

    public List<PlayerCharacter> ActivePlayerCharacters => PlayerCharacterPoolManager.Instance.ActivePlayerCharacters;

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

    /// <summary>
    /// Metodo chiamato quando un nuovo giocatore si unisce al gioco. Crea un nuovo PlayerInputHandler,
    /// imposta dell'ID del giocatore e configura l'InputReceiver prendendolo dallo SceneInputReceiverManager.
    /// </summary>
    /// <param name="playerInput">L'input del giocatore che si � unito al gioco.</param>
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

            if (newPlayerInputHandler.CurrentReceiver.GetGameObject().GetComponent<PlayerCharacterController>())
                PubSub.Instance.Notify(EMessageType.characterJoined, newPlayerInputHandler.CurrentReceiver.GetGameObject().GetComponent<PlayerCharacterController>());
                
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
