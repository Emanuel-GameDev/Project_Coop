using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [SerializeField]
    private DeathScreen deathScreen;
    private int deadPlayers = 0;
    public bool showDeathScreen = true;

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

        PubSub.Instance.RegisterFunction(EMessageType.sceneLoading, SaveCharacter);
    }

    #region Switching Character
    private void InizializeList()
    {
        foreach (PlayerCharacterData characterData in GameManager.Instance.GetCharacterDataList())
        {
            PlayerCharacter playerCharacter = Instantiate(characterData.CharacterPrefab, transform).GetComponent<PlayerCharacter>();
            freeCharacters.Add(playerCharacter);
            //playerCharacter.Inizialize(); //DA RIVEDERE #MODIFICATO
            playerCharacter.gameObject.SetActive(false);
        }
        SaveManager.Instance.LoadAllPlayersData();

    }

    public void SwitchCharacter(PlayerCharacter playerCharacter, ePlayerCharacter targetCharacter)
    {
        if (!SceneInputReceiverManager.Instance.CanSwitchCharacter)
            return;

        PlayerCharacter searchedCharacter = freeCharacters.Find(c => c.Character == targetCharacter);
        if (searchedCharacter != null)
        {
            PubSub.Instance.Notify(EMessageType.switchingCharacters, new PlayerCharacter[] { playerCharacter, searchedCharacter });
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
        playerCharacter.ResetAllAnimatorTriggers();
        freeCharacters.Remove(playerCharacter);
        activeCharacters.Add(playerCharacter);
        //if (newPlayerInputHandler.CurrentReceiver.GetGameObject().GetComponent<PlayerCharacterController>())
        PubSub.Instance.Notify(EMessageType.characterActivated, playerCharacter);
        CameraManager.Instance.AddTarget(playerCharacter.transform);
    }

    public void ReturnCharacter(PlayerCharacter playerCharacter)
    {
        playerCharacter.gameObject.transform.parent = transform;
        playerCharacter.gameObject.transform.localPosition = Vector3.zero;
        playerCharacter.gameObject.SetActive(false);
        freeCharacters.Add(playerCharacter);
        activeCharacters.Remove(playerCharacter);
        PubSub.Instance.Notify(EMessageType.characterDeactivated, playerCharacter);
        CameraManager.Instance.RemoveTarget(playerCharacter.transform);
    }

    //DA RIVEDERE #MODIFICATO
    public ePlayerCharacter GetFreeRandomCharacter()
    {
        ePlayerCharacter searchedCharacter = freeCharacters[Random.Range(0, freeCharacters.Count)].Character;

        return searchedCharacter;
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
        }
        return searchedCharacter;
    }

    #endregion

    #region PlayerDeadManager

    public void PlayerIsDead()
    {
        deadPlayers++;
        if (deadPlayers >= activeCharacters.Count)
        {
            deadPlayers = 0;
            if(showDeathScreen)
            ActivateDeathScreen();
        }

    }

    public void PlayerIsRessed()
    {
        deadPlayers--;
        if (deadPlayers <= 0)
            deadPlayers = 0;
    }

    public void ActivateDeathScreen()
    {
        if (deathScreen != null)
            deathScreen.ShowDeathScreen();
        else
            GameManager.Instance.ChangeScene(SceneManager.GetActiveScene().name);
    }

    #endregion

    public void SaveCharacter(object obj)
    {
        SaveManager.Instance.SavePlayersData();
    }

    public void HealAllPlayerFull()
    {
        foreach (PlayerCharacter p in AllPlayerCharacters)
        {
            p.TakeHeal(new DamageData(99999, null));
        }
    }
}
