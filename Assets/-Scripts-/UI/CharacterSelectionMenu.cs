using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerSelection
{
    public PlayerData data;
    public bool selected;
    public InputDevice device;

    public PlayerSelection(PlayerData data, bool iconSelected, bool randomBtnSelected)
    {
        this.data = data;
        selected = iconSelected;
        device = null;
    }

    public void EditIcon(bool iconSelected)
    {
        selected = iconSelected;
    }

    public void EditDevice(InputDevice device)
    {
        this.device = (device != null) ? device : null;

        Debug.Log(device.name);
    }

    public void Print()
    {
        if (data == null || device == null)
        {
            Debug.LogError("NoDevice");
            return;
        }


        Debug.Log(
            "classe: " + data._class.ToString() + "\n" +
            "id dispositivo: " + device.deviceId + "\n" +
            "nome dispositivo: " + device.name + "\n");
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

    private List<InputDevice> devices = new List<InputDevice>();    

    private int randomSelectionCounter = 0;

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

    public List<PlayerSelection> GetCharactersSelected()
    {
        return selectableCharacters;
    }

    /// <summary>
    /// Aggiungo alla lista di player selections delle player selection vuote che andranno riempite
    /// man mano che un utente sceglie il personaggio
    /// </summary>
    private void InitialiazeSelections()
    {
        for (int i = 0; i < characterIcons.Count; i++)
        {
            PlayerSelection selection = new PlayerSelection(characterIcons[i], false, false);
            selectableCharacters.Add(selection);
        }
    }

    // forse è da cmabiare perché pensavo di avere una list di dispositivi nel coopmanager
    public void AddToDevices(InputDevice deviceToAdd)
    {
        devices.Add(deviceToAdd);
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
    public void UpdatePlayerSelection(RectTransform iconToSelect, bool mode, InputDevice whoSelected)
    {
        // Iter sulle selections
        foreach (PlayerSelection s in selectableCharacters)
        {
            // Check sull'icona
            if (iconToSelect == s.data._icon)
            {
                s.EditIcon(mode);
                s.EditDevice(whoSelected);

                s.Print();
            }
        }

        if (PlayersDoneSelecting() == PlayerInputManager.instance.playerCount)
        {
            EndSelection();
        }
    }

    private int PlayersDoneSelecting()
    {
        int count = 0;

        foreach (PlayerSelection player in selectableCharacters)
        {
            if (AlreadySelected(player.data._icon))
                count++;

        }

        return count;
    }

    #region RandomSelection

    /// <summary>
    /// Serve ad incrementare o decrementare il conto degli utenti che hanno premuto il tasto random
    /// </summary>
    /// <param name="mode"></param>
    public void TriggerRandomSelection(bool mode, InputDevice whoSelected)
    {
        if (mode)
        {
            randomSelectionCounter++;
            Debug.Log($"Player_{whoSelected.deviceId} vuole avviare la selezione random");
        }
        else
        {
            randomSelectionCounter--;
            Debug.Log($"Player_{whoSelected.deviceId} ha rimosso il consenso alla selezione random");
        }

        UpdateRandomBtnSelection();
    }

    public void UpdateRandomBtnSelection()
    {
        int totalPlayerCount = PlayerInputManager.instance.playerCount;

        if (randomSelectionCounter == totalPlayerCount)
        {
            Debug.Log("selezione random attivata");

            RandomSelection();
        }
        else
        {
            Debug.Log("Non abbastanza consensi per attivare selezione random");
        }
    }

    /// <summary>
    /// Prende una lista di tutti i dispositivi connessi, e li assegna ad un player selection
    /// </summary>
    private void RandomSelection()
    {
        // N.B ora uso una lista che viene riempita ogni volta che uno si connette, magarti vorrei avere
        // una funzione nel coopManager tipo che ha lui una lista e quando la voglio gliela chiedo
        // Intendo devices

        int temp = 0;

        foreach (InputDevice device in devices)
        {
            int rand = 0;
            do
            {
                rand = Random.Range(0, selectableCharacters.Count);
            }
            while (selectableCharacters[rand].selected);

            temp = rand;

            selectableCharacters[rand].EditIcon(true);
            selectableCharacters[rand].EditDevice(device);
            selectableCharacters[rand].Print();

        }

        selectableCharacters[temp].Print();

        EndSelection();
    }

    #endregion

    public void EndSelection()
    {
        Debug.Log("Selezione completa");
        CoopManager.Instance.UpdateSelectedPlayers(selectableCharacters);
        GameManager.Instance.LoadNextScene();
    }


}
