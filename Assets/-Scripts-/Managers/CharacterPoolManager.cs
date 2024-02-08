using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] 
    private List<CharacterClass> characters = new List<CharacterClass>();

    private List<CharacterClass> freeCharacters = new List<CharacterClass>();

    private void Start()
    {
        InizializeList();
    }

    #region Switching Character

    private void InizializeList()
    {
        foreach (CharacterClass character in characters)
        {
            CharacterClass characterClass = GameObject.Instantiate(character, transform);
            freeCharacters.Add(characterClass);
        }
    }

    public void SwitchCharacter(PlayerCharacter playerCharacter, ePlayerCharacter targetClass)
    {
        if (SceneInputReceiverManager.Instance.CanSwitchCharacter)
            return;

        CharacterClass searchedClass = freeCharacters.Find(c => c.Character == targetClass);
        if (searchedClass != null)
        {
            playerCharacter.SwitchCharacterClass(searchedClass);
            freeCharacters.Remove(searchedClass);
            return;
        }
    }

    public void ReturnCharacter(CharacterClass characterClass)
    {
        characterClass.gameObject.transform.parent = transform;
        characterClass.gameObject.SetActive(false);
        freeCharacters.Add(characterClass);
    }

    #endregion

}
