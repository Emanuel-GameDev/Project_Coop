using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class PlayerCharacterPoolManager : MonoBehaviour
{
    private static PlayerCharacterPoolManager _instance;
    public static PlayerCharacterPoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerCharacterPoolManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("PlayerCharacterPoolManager");
                    _instance = singletonObject.AddComponent<PlayerCharacterPoolManager>();
                }
            }

            return _instance;
        }
    }

    private List<PlayerCharacter> freeCharacters = new List<PlayerCharacter>();
    private List<PlayerCharacter> activeCharacters = new List<PlayerCharacter>();
    public List<PlayerCharacter> ActivePlayerCharacters => activeCharacters;
    public List<PlayerCharacter> FreePlayerCharacters => freeCharacters;
    public List<PlayerCharacter> AllPlayerCharacters
    {
        get
        {
            List<PlayerCharacter> allCharacters = new List<PlayerCharacter>();
            allCharacters.AddRange(activeCharacters);
            allCharacters.AddRange(freeCharacters);
            return allCharacters;
        }

    }

    private int deadPlayers = 0;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            InizializeList();
        }
    }

    #region Switching Character

    private void InizializeList()
    {
        foreach(PlayerCharacterData characterData in GameManager.Instance.GetCharacterDataList())
        {
            PlayerCharacter playerCharacter = Instantiate(characterData.CharacterPrefab, transform).GetComponent<PlayerCharacter>();
            freeCharacters.Add(playerCharacter);
            //playerCharacter.Inizialize(); //DA RIVEDERE #MODIFICATO
            playerCharacter.gameObject.SetActive(false);
        }
    }

    public void SwitchCharacter(PlayerCharacter playerCharacter, ePlayerCharacter targetCharacter)
    {
        if (!SceneInputReceiverManager.Instance.CanSwitchCharacter)
            return;

        PlayerCharacter searchedCharacter = freeCharacters.Find(c => c.Character == targetCharacter);
        if (searchedCharacter != null)
        {
            playerCharacter.characterController.SetPlayerCharacter(searchedCharacter);
            playerCharacter.characterController = null;
            ActivateCharacter(searchedCharacter, playerCharacter.transform);
            PubSub.Instance.Notify(EMessageType.characterSwitched, searchedCharacter);

            ReturnCharacter(playerCharacter);

            TargetManager.Instance.ChangeTarget(playerCharacter, searchedCharacter);
        }
    }

    private void ActivateCharacter(PlayerCharacter playerCharacter, Transform spawnPosition)
    {
        ActivateCharacter(playerCharacter, spawnPosition.position);
    }

    private void ActivateCharacter(PlayerCharacter playerCharacter, Vector3 spawnPosition)
    {
        playerCharacter.gameObject.transform.parent = null;
        playerCharacter.gameObject.transform.position = spawnPosition;
        playerCharacter.gameObject.SetActive(true);
        playerCharacter.SetSwitchCooldown();
        freeCharacters.Remove(playerCharacter);
        activeCharacters.Add(playerCharacter);
        //if (newPlayerInputHandler.CurrentReceiver.GetGameObject().GetComponent<PlayerCharacterController>())
        PubSub.Instance.Notify(EMessageType.characterJoined, playerCharacter);
        CameraManager.Instance.AddTarget(playerCharacter.transform);
    }

    public void ReturnCharacter(PlayerCharacter playerCharacter)
    {
        playerCharacter.gameObject.transform.parent = transform;
        playerCharacter.gameObject.transform.localPosition = Vector3.zero; 
        playerCharacter.gameObject.SetActive(false);
        freeCharacters.Add(playerCharacter);
        activeCharacters.Remove(playerCharacter);

        CameraManager.Instance.RemoveTarget(playerCharacter.transform);
    }

    //DA RIVEDERE #MODIFICATO
    public ePlayerCharacter GetFreeRandomCharacter()
    {
        ePlayerCharacter searchedCharacter = freeCharacters[Random.Range(0, freeCharacters.Count)].Character;
        
        return searchedCharacter;


        //PlayerCharacter searchedCharacter = freeCharacters[Random.Range(0, freeCharacters.Count)];
        //if(searchedCharacter == null) return null;

        //ActivateCharacter(searchedCharacter, SpawnPositionManager.Instance.GetFreePos().spawnPos); //transform.position); 
        //return searchedCharacter;
    }

    public PlayerCharacter GetCharacter(ePlayerCharacter targetCharacter, Transform position)
    {
        return GetCharacter(targetCharacter, position.position);
    }

    public PlayerCharacter GetCharacter(ePlayerCharacter targetCharacter, Vector3 position)
    {
        PlayerCharacter searchedCharacter = freeCharacters.Find(c => c.Character == targetCharacter);
        if (searchedCharacter != null)
        {
            ActivateCharacter(searchedCharacter, position);
            //PubSub.Instance.Notify(EMessageType.characterJoined, searchedCharacter);
        }
        return searchedCharacter;
    }

    #endregion

    #region PlayerDeadManager

    public void PlayerIsDead()
    {
        deadPlayers++;
        if(deadPlayers >= activeCharacters.Count)
        {
            deadPlayers = 0;
            GameManager.Instance.LoadScene(SceneManager.GetActiveScene().name);
        }
            
    }

    public void PlayerIsRessed()
    {
        deadPlayers--;
        if (deadPlayers <= 0)
            deadPlayers = 0;
    }

    #endregion


}
