using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorBehaviour : DefaultInputReceiver
{
    private Vector2 movement;
    private GameObject selectionParent;
    public TextMeshProUGUI infoText;

    // Gli oggetti su cui il cursore potr� muoversi
    [HideInInspector] public List<RectTransform> objectsToOver;
    private int currentIndex = 0;

    public bool objectSelected = false;
    private bool randomBtnSelected = false;

    private void Start()
    {
        List<RectTransform> icons = CharacterSelectionMenu.Instance.GetCharacterIcons();

        //objectsToOver.Clear();
        objectsToOver = new List<RectTransform>();

        foreach (RectTransform icon in icons)
        {
            objectsToOver.Add(icon);
        }

        GetComponent<RectTransform>().SetParent(icons[0].gameObject.GetComponent<RectTransform>());
        GetComponent<RectTransform>().localPosition = Vector3.zero;
        GetComponent<RectTransform>().localScale = new Vector2(1f, 1f);

        CharacterSelectionMenu.Instance.AssignInfoText(this);
    }

    public override void Navigate(InputAction.CallbackContext context)
    {
        if (context.started && randomBtnSelected)
        {
            Debug.LogWarning($"Player {context.control.device.deviceId} non pu� muoversi poich� ha" +
                $" avviato una votazione per la selezione random");

            infoText.text = $"Player {context.control.device.deviceId} non pu� muoversi poich� ha" +
                $" avviato una votazione per la selezione random";

            return;
        }    

        // Bottone premuto
        if (context.started && !objectSelected)
        {
            movement = context.ReadValue<Vector2>();

            if (movement.x > 0)
            {
                // Premuto "D", spostati all'oggetto successivo
                currentIndex = (currentIndex + 1) % objectsToOver.Count;
            }
            else if (movement.x < 0)
            {
                // Premuto "A", spostati all'oggetto precedente
                currentIndex = (currentIndex - 1 + objectsToOver.Count) % objectsToOver.Count;
            }

            // Aggiorna la posizione del cursore in base all'oggetto corrente nella lista
            GetComponent<RectTransform>().position = objectsToOver[currentIndex].position;
            transform.SetParent(objectsToOver[currentIndex]);
        }
    }

    /// <summary>
    /// Filtra la validit� della selezione del personaggio,
    /// poi chiede al menu di aggiornare la selezione nella lista di PlayerSelections
    /// </summary>
    /// <param name="context"></param>
    public override void Submit(InputAction.CallbackContext context)
    {
        if (CharacterSelectionMenu.Instance.randomVoteActive && context.started)
        {
            Debug.LogWarning($"Player {context.control.device.deviceId} non pu� selezionare poich� ha avviato" +
                $" una votazione per la selezione random");
            infoText.text = $"Player {context.control.device.deviceId} non pu� selezionare poich� ha avviato" +
                $" una votazione per la selezione random";

            return;
        }

        if (context.started)
        {
            selectionParent = transform.parent.gameObject;

            // Se un'altro personaggio ha gi� selezionato un'oggetto return
            if (CharacterSelectionMenu.Instance.AlreadySelected(this, selectionParent.GetComponent<RectTransform>()))
                return;

            // Se non ho selezionato niente, seleziono
            if (!objectSelected)
            {
                objectSelected = true;
                Debug.Log("selected");
                infoText.text = "selected";
            }
            // Se ho gi� selezionato, deseleziono
            else
            {
                objectSelected = false;
                Debug.Log("Deselected");
                infoText.text = "deselected";
            }

            CharacterSelectionMenu.Instance.UpdatePlayerSelection(this, selectionParent.GetComponent<RectTransform>(), objectSelected);
        }
    }

    public override void SetCharacter(ePlayerCharacter character)
    {
        base.SetCharacter(character);

        playerInputHandler.SetCharacter(character);
    }

    /// <summary>
    /// Chiede al menu di aggiornare il numero degli utenti che hanno dato il consenso ad usare la selezione random
    /// </summary>
    /// <param name="context"></param>
    public override void RandomSelection(InputAction.CallbackContext context)
    {
        if (context.started && !objectSelected)
        {
            randomBtnSelected = !randomBtnSelected;
            CharacterSelectionMenu.Instance.TriggerRandomSelection(randomBtnSelected);
        }

    }


}
