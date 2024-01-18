using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using System;

public struct PlayerSelection
{
    public PlayerData data;
    public bool selected;
    public Dictionary<int, string> playerControlScheme;

    public PlayerSelection(PlayerData data, bool selected)
    {
        this.data = data;
        this.selected = selected;
        playerControlScheme = new Dictionary<int,string>();
    }

    public void Edit(bool selected)
    {
        this.selected = selected;
    }

    public void Print()
    {
        Debug.Log(data._class.ToString());
    }
}

[Serializable]
public class PlayerData
{
    public RectTransform _icon;
    public CharacterClass _class;
}

public class CharacterSelectionMenu : MonoBehaviour
{
    // Le icone che vanno inserite da Inspector
    public List<PlayerData> characterIcons = new List<PlayerData>();

    // la struct che verrà usata per capire se un personaggio è stato selezionato
    private List<PlayerSelection> selectableCharacters = new List<PlayerSelection>();

    private Button randomBtn;

    public static CharacterSelectionMenu Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    private void Start()
    {
        InitialiazeSelections();

        randomBtn = GetComponentInChildren<Button>();

        if (randomBtn == null)
            Debug.LogError("Error getting Btn reference");
        else
            randomBtn.onClick.AddListener(SelectRandom);
    }

    private void InitialiazeSelections()
    {
        for (int i = 0; i < characterIcons.Count; i++)
        {
            PlayerSelection selection = new PlayerSelection(characterIcons[i], false);
            selectableCharacters.Add(selection);
        }
    }

    /// <summary>
    /// Instanzia il cursore dalla cartella Resources
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public GameObject LoadCursor(int id)
    {
        GameObject cursor = Resources.Load<GameObject>($"CursorPrefabs/P{id}_Cursor");

        cursor.GetComponent<CursorBehaviour>().Initialize(selectableCharacters);

        return cursor;
    }

    /// <summary>
    /// Controlla se un'icona è stata selezionata da un'altro player
    /// </summary>
    /// <param name="iconToCheck"></param>
    public bool AlreadySelected(RectTransform iconToCheck)
    {
        foreach (PlayerSelection s in selectableCharacters)
        {
            if (iconToCheck == s.data._icon)
            {
                if (s.selected)
                    return true;
                else
                    return false;
            }
        }

        return false;
    }

    /// <summary>
    /// Aggiorna il bool all'interno della lista delle selezioni quando un personaggio clicca sull'icona
    /// </summary>
    /// <param name="iconToSelect"></param>
    public void UpdateSelection(RectTransform iconToSelect, bool mode)
    {
        foreach (PlayerSelection s in selectableCharacters)
        {
            if (iconToSelect == s.data._icon)
            {
                s.Edit(mode);
                s.Print();
            }
        }
    }

    public void SelectRandom()
    {
        Debug.Log("ksj");
    }
}
