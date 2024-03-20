using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterPoolManager : MonoBehaviour
{
    private static CharacterPoolManager _instance;
    public static CharacterPoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CharacterPoolManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("CharacterPoolManager");
                    _instance = singletonObject.AddComponent<CharacterPoolManager>();
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
        }
    }

    private List<PlayerCharacter> freeCharacters = new List<PlayerCharacter>();

    private void Start()
    {
        InizializeList();
    }

    #region Switching Character

    private void InizializeList()
    {
        foreach(PlayerCharacterData characterData in GameManager.Instance.GetCharacterDataList())
        {
            PlayerCharacter playerCharacter = Instantiate(characterData.CharacterPrefab, transform).GetComponent<PlayerCharacter>();
            freeCharacters.Add(playerCharacter);
            playerCharacter.Inizialize(); //DA RIVEDERE #MODIFICATO
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
            //DA RIVEDERE #MODIFICATO
            //playerCharacter.SwitchCharacter(searchedCharacter);
            //freeCharacters.Remove(searchedCharacter);
            //searchedCharacter.gameObject.SetActive(true);
            //PubSub.Instance.Notify(EMessageType.characterSwitched, playerCharacter);
            //return;
        }
    }

    public void ReturnCharacter(PlayerCharacter playerCharacter)
    {
        playerCharacter.gameObject.transform.parent = transform;
        playerCharacter.gameObject.transform.localPosition = Vector3.zero; 
        playerCharacter.gameObject.SetActive(false);
        freeCharacters.Add(playerCharacter);
    }

    public void GetFreeRandomCharacter(PlayerCharacter playerCharacter)
    {
        SwitchCharacter(playerCharacter , freeCharacters[Random.Range(0, freeCharacters.Count)].Character);
    }

    #endregion

}
