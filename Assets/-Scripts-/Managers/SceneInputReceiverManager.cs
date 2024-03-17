using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInputReceiverManager : MonoBehaviour
{
    private static SceneInputReceiverManager _instance;
    public static SceneInputReceiverManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SceneInputReceiverManager>();

                if (_instance == null)
                {
                    Debug.LogError("No SceneInputReceiverManager found in the scene. Please add one.");
                }
            }

            return _instance;
        }
    }

    [SerializeField, Tooltip("Imposta la mappatura di comandi da utilizzare per la scena corrente")]
    eInputMap sceneInputMap;
    [SerializeField, Tooltip("Imposta il Prefab che riceve gli input per la scena corrente")] 
    GameObject currentSceneInputReceiverPrefab;
    [SerializeField, Tooltip("Imposta il punto di spawn del Prefab nella scena")] 
    Transform receiverSpawnPoint;
    [SerializeField, Tooltip("Determina se nella scena corrente è possibile cambiare personaggio")] 
    bool canSwitchCharacter;
    public bool CanSwitchCharacter => canSwitchCharacter;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError("SceneInputReceiverManager already exists. Deleting duplicate.");
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        //CoopManager.Instance.InitializePlayers();
    }

    private void Start()
    {
        CoopManager.Instance.InitializePlayers();
    }

    public InputReceiver GetSceneInputReceiver(PlayerInputHandler newPlayerInputHandler)
    {
        if(currentSceneInputReceiverPrefab == null)
        {
            Debug.LogError("No Prefab found in the scene. Please add one.");
            return null;
        }
        else
        {
            GameObject newGO = GameObject.Instantiate(currentSceneInputReceiverPrefab);
            newGO.transform.SetPositionAndRotation(receiverSpawnPoint.position, receiverSpawnPoint.rotation);
            CameraManager.Instance.AddTarget(newGO.transform);
            InputReceiver newInputReceiver = newGO.GetComponent<InputReceiver>();
            if(newInputReceiver == null)
            {
                Debug.LogError("No InputReceiver found in the Prefab. Please add one.");
                return null;
            }
            newInputReceiver.SetInputHandler(newPlayerInputHandler);
           
            return newInputReceiver;
        }
    }

    public eInputMap GetSceneActionMap()
    {
        return sceneInputMap;
    }
}

public enum eInputMap
{
    Player,
    UI,
    Minigame
}