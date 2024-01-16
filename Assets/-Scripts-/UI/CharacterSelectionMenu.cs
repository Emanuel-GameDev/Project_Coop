using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;

public struct PlayerSelection
{
    public GameObject selectableIcon;
    // classe del character
    public bool selected;
    public Dictionary<int, string> playerControlScheme;

    public PlayerSelection(GameObject selectableIcon, bool selected)
    {
        this.selectableIcon = selectableIcon;
        this.selected = selected;
        playerControlScheme = new Dictionary<int,string>();
    }
}

public class CharacterSelectionMenu : MonoBehaviour
{
    public List<RectTransform> characterIcons = new List<RectTransform>();

    private List<PlayerSelection> selectedPlayers = new List<PlayerSelection>();

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
            PlayerSelection placeHolderSelection = new PlayerSelection(characterIcons[i].gameObject, false);
            selectedPlayers.Add(placeHolderSelection);
        }

        foreach (PlayerSelection ps in selectedPlayers)
        {
            Debug.Log(ps.selectableIcon.name + "  " + ps.selected.ToString());
        }
    }

    public GameObject LoadCursor(int id)
    {
        GameObject cursor = Resources.Load<GameObject>($"CursorPrefabs/P{id}_Cursor");

        cursor.GetComponent<CursorBehaviour>().Initialize(this);

        return cursor;
    }
}
