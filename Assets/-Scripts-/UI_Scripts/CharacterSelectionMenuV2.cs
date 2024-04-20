using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class PlayerIconSelection
{
    public ePlayerID playerID;
    public List<RectTransform> selectors;
}

public class CharacterSelectionMenuV2 : MonoBehaviour
{
    [SerializeField]
    private List<PlayerIconSelection> playerIconSelections = new List<PlayerIconSelection>();

    [SerializeField]
    private GameObject fasciaReady;

    private List<CursorBehaviourV2> activeCursors = new List<CursorBehaviourV2>();

    public static CharacterSelectionMenuV2 Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    private void Start()
    {
    }

    internal void AddToActiveCursors(CursorBehaviourV2 cursor)
    {
        activeCursors.Add(cursor);
    }

    internal List<RectTransform> GetCharacterSelectors(ePlayerID ID)
    {
        List<RectTransform> selectors = new List<RectTransform>();

        foreach (PlayerIconSelection selection in playerIconSelections)
        {
            if (selection.playerID == ID)
                selectors = selection.selectors;
        }

        return selectors;
    }

    internal void PlayerOvered(bool mode, GameObject cursor)
    {
        if (mode)
        {
            cursor.GetComponent<RectTransform>().parent.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            if (cursor.GetComponent<RectTransform>().parent != null)
                cursor.GetComponent<RectTransform>().parent.GetChild(0).gameObject.SetActive(false);
        }
    }

    internal bool TriggerPlayerSelection(bool mode, GameObject cursor)
    {
        if (mode)
        {
            // Spengo lo sprite del P overed
            cursor.GetComponent<RectTransform>().parent.GetChild(0).gameObject.SetActive(false);

            // Accendo lo sprite del P ready
            cursor.GetComponent<RectTransform>().parent.GetChild(1).gameObject.SetActive(true);

            // Setto la classe
            cursor.GetComponent<CursorBehaviourV2>().SetCharacter();

            // Controllo non necessario sullo sprite del ready per impostare il bool del cursore
            if (cursor.GetComponent<RectTransform>().parent.GetChild(1).gameObject.activeSelf)
                return true;
            else
                return false;
        }
        else
        {
            // Spengo lo sprite del P ready
            cursor.GetComponent<RectTransform>().parent.GetChild(1).gameObject.SetActive(false);

            // Accendo lo sprite del P overed
            cursor.GetComponent<RectTransform>().parent.GetChild(0).gameObject.SetActive(true);

            // Controllo non necessario sullo sprite del ready per impostare il bool del cursore
            if (cursor.GetComponent<RectTransform>().parent.GetChild(0).gameObject.activeSelf)
                return true;
            else
                return false;
        }
    }

    internal void SelectRandom(CursorBehaviourV2 cursor)
    {
        int randContainerID = UnityEngine.Random.Range(0, cursor.objectsToOver.Count);
        cursor.Select(randContainerID);
        TriggerPlayerSelection(true, cursor.gameObject);
    }

    internal bool AlreadySelected(CursorBehaviourV2 cursor)
    {
        RectTransform cursorParent = cursor.gameObject.GetComponent<RectTransform>().parent.parent.gameObject.GetComponent<RectTransform>();
        bool occupied = false;

        foreach (CursorBehaviourV2 item in activeCursors)
        {
            RectTransform itemParent = item.gameObject.GetComponent<RectTransform>().parent.parent.gameObject.GetComponent<RectTransform>();

            // Check per assicurarmi di non star controllando lo stesso cursore del paramentro
            if (cursor == item) continue;

            // Check per vedere se, in un personaggio sono attivi pi� cursori
            if (cursorParent == itemParent)
            {
                // Check per vedere se qualche altro cursore ha selezionato il pg
                if (item.objectSelected)
                {
                    occupied = true;
                    return occupied;
                }
                else
                    occupied = false;
            }
        }

        return occupied;
    }

    internal bool AllReady()
    {
        bool allReady = true;

        foreach (CursorBehaviourV2 cursor in activeCursors)
        {
            if (!cursor.objectSelected)
                allReady = false;
        }

        return allReady;
    }

    internal Button TriggerFasciaReady(bool mode)
    {
        if (mode)
        {
            fasciaReady.SetActive(true);
            return fasciaReady.GetComponentInChildren<Button>();
        }
        else
        {
            fasciaReady.SetActive(false);
            return null;
        }
    }

    public void SelectionOver(ePlayerID ID)
    {
        // Check se � il player 1 a premere
        if (ID == ePlayerID.Player1)
        {
            Debug.Log("Selection Over");
        }
    }
}
