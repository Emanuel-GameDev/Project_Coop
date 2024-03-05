using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerSelection
{
    public PlayerData data;
    public bool selected;
    public InputReceiver whoSelected;

    public PlayerSelection(PlayerData data, bool iconSelected, InputReceiver receiver)
    {
        this.data = data;
        selected = iconSelected;
        this.whoSelected = receiver;
    }

    public void EditIcon(bool iconSelected)
    {
        selected = iconSelected;
    }

    public void AssignReceiver(InputReceiver receiver)
    {
        if (receiver == null) return;

        whoSelected = receiver;
    }

    public void Select()
    {
        if(whoSelected == null) return;

        whoSelected.SetCharacter(data._class);
    }

}

[Serializable]
public class PlayerData
{
    public RectTransform _icon;
    public ePlayerCharacter _class;
}

public class CharacterSelectionMenu : MonoBehaviour
{
    // Le icone che vanno inserite da Inspector
    public List<PlayerData> characterIcons = new List<PlayerData>();

    // la struct che verrà usata per capire se un personaggio è stato selezionato
    private List<PlayerSelection> selectableCharacters = new List<PlayerSelection>();

    private int randomSelectionCounter = 0;
    public bool randomVoteActive
    {
        get { 

            if (randomSelectionCounter == 0) return false;
            else return true;
        }
        private set { }
    }

    [SerializeField]
    private List<TextMeshProUGUI> playerInfoTexts = new List<TextMeshProUGUI>();

    private List<TextMeshProUGUI> privatePlayerTexts;

    [SerializeField]
    private TextMeshProUGUI displayInfo;

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

        privatePlayerTexts = playerInfoTexts;
        //GameManager.Instance.LoadSceneInbackground("TestLeo2D");
    }

    public List<RectTransform> GetCharacterIcons()
    {
        List<RectTransform> icons = new List<RectTransform>();

        for (int i = 0; i < selectableCharacters.Count; i++)
        {
            icons.Add(selectableCharacters[i].data._icon);
        }

        return icons;
    }

    public void AssignInfoText(CursorBehaviour cursor)
    {
        cursor.infoText = privatePlayerTexts[privatePlayerTexts.Count - 1];
        privatePlayerTexts.Remove(cursor.infoText);
    }

    /// <summary>
    /// Aggiungo alla lista di player selections delle player selection vuote che andranno riempite
    /// man mano che un utente sceglie il personaggio
    /// </summary>
    private void InitialiazeSelections()
    {
        for (int i = 0; i < characterIcons.Count; i++)
        {
            PlayerSelection selection = new PlayerSelection(characterIcons[i], false, null);
            selectableCharacters.Add(selection);
        }
    }

    /// <summary>
    /// Controlla se un'icona è stata selezionata da un'altro player
    /// </summary>
    /// <param name="iconToCheck"></param>
    public bool AlreadySelected(InputReceiver receiver, RectTransform iconToCheck)
    {
        foreach (PlayerSelection s in selectableCharacters)
        {
            if (iconToCheck == s.data._icon)
            {
                if (receiver != null)
                {
                    if (receiver == s.whoSelected) return false;
                }

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
    public void UpdatePlayerSelection(InputReceiver receiver, RectTransform iconToSelect, bool mode)
    {
        // Iter sulle selections
        foreach (PlayerSelection s in selectableCharacters)
        {
            // Check sull'icona
            if (iconToSelect == s.data._icon)
            {
                s.EditIcon(mode);
                s.AssignReceiver(receiver);
                s.Select();
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
            if (AlreadySelected(null, player.data._icon))
                count++;

        }

        return count;
    }

    #region RandomSelection

    /// <summary>
    /// Serve ad incrementare o decrementare il conto degli utenti che hanno premuto il tasto random
    /// </summary>
    /// <param name="mode"></param>
    public void TriggerRandomSelection(bool mode)
    {
        if (mode)
        {
            randomSelectionCounter++;

            Debug.Log($"Player_NUM vuole avviare la selezione random");
            displayInfo.text = "Player_NUM vuole avviare la selezione random, selezione personaggi disattivata (solo chi ha fatto la richiesta può annullarla)";

        }
        else
        {
            randomSelectionCounter--;

            Debug.Log($"Player_NUM ha rimosso il consenso alla selezione random");
            displayInfo.text = "Consenso rimosso";
        }

        UpdateRandomBtnSelection();
    }

    public void UpdateRandomBtnSelection()
    {
        int totalPlayerCount = PlayerInputManager.instance.playerCount;

        if (randomSelectionCounter == totalPlayerCount)
        {
            Debug.Log("selezione random attivata");
            displayInfo.text = "Selezione confermata da tutti i giocatori";

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
        List<PlayerInputHandler> handlers = CoopManager.Instance.GetActualHandlers();
        
        foreach (PlayerInputHandler handler in handlers)
        {
            int rand = 0;
            do
            {
                rand = Random.Range(0, selectableCharacters.Count);
            }
            while (selectableCharacters[rand].selected);

            selectableCharacters[rand].EditIcon(true);
            selectableCharacters[rand].AssignReceiver(handler.CurrentReceiver);
            selectableCharacters[rand].Select();
        }

        EndSelection();
    }

    #endregion

    public void EndSelection()
    {
        Debug.Log("Selezione completa");

        //GameManager.Instance.ActivateLoadedScene("TestLeo2D");

        //disabilito un attimo -fede
        //GameManager.Instance.LoadScene("TestLeo2D");

        GameManager.Instance.LoadNextScene();
    }


}
