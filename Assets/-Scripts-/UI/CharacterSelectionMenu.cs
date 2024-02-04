using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.InputSystem.Users;

public struct PlayerSelection
{
    public PlayerData data;
    public bool selected;
    public bool randomBtnSelected;
    public int deviceID;
    public string deviceName;
    public InputDevice device;

    public PlayerSelection(PlayerData data, bool iconSelected, bool randomBtnSelected)
    {
        this.data = data;
        this.selected = iconSelected;
        this.randomBtnSelected = randomBtnSelected;
        deviceID = 0;
        deviceName = "";
        device = null;
    }

    public void EditIcon(bool iconSelected)
    {
        this.selected = iconSelected;
    }

    public void EditDevice(InputDevice device)
    {
        // Ottenere l'ID univoco dell'InputDevice come chiave
        deviceID = device.deviceId;

        // Ottenere il nome del dispositivo come valore
        deviceName = device.name;

        this.device = device;
    }

    public void Print()
    {
        Debug.Log(
            "classe: " + data._class.ToString() + "\n" +
            "id dispositivo: " + deviceID + "\n" +
            "nome dispositivo: " + deviceName + "\n");
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
    }

    private void InitialiazeSelections()
    {
        for (int i = 0; i < characterIcons.Count; i++)
        {
            PlayerSelection selection = new PlayerSelection(characterIcons[i], false, false);
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
    public void UpdateSelection(RectTransform iconToSelect, bool mode, InputDevice whoSelected)
    {
        foreach (PlayerSelection s in selectableCharacters)
        {
            if (iconToSelect == s.data._icon)
            {
                s.EditIcon(mode);
                s.EditDevice(whoSelected);
                s.Print();
            }
        }
    }

    public void UpdateRandomSelection()
    {

    }

    public void EndSelection()
    {
        CoopManager.Instance.UpdateSelectedPalyers(selectableCharacters);
    }


}
