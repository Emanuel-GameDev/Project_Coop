using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorBehaviourV2 : InputReceiver
{
    // Gli oggetti su cui il cursore potr� muoversi
    [HideInInspector] public List<RectTransform> objectsToOver = new List<RectTransform>();
    private int currentIndex = -1;

    private ePlayerID playerID;
    internal ePlayerID PlayerID => playerID;

    private Button confirmBtn;
    private Vector2 movement;
    private bool canMove = false;
    internal bool objectSelected = false;

    private void Start()
    {
        CharacterSelectionMenuV2.Instance.AddToActiveCursors(this);

        if (playerInputHandler != null)
        {
            playerID = playerInputHandler.playerID;
        }

        objectsToOver = CharacterSelectionMenuV2.Instance.GetCharacterSelectors(playerID);

        canMove = true;
    }

    public override void Navigate(InputAction.CallbackContext context)
    {
        if (!canMove) return;

        movement = context.ReadValue<Vector2>();

        if (context.started && !objectSelected)
        {
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
            else if (movement.y > 0)
                return;
            else if (movement.y < 0)
                return;

            Select(currentIndex);
        }
    }

    internal void Select(int id)
    {
        int actualID;

        if (id != currentIndex)
            actualID = id;
        else
            actualID = currentIndex;

        // Spengo il P che prima era acceso
        CharacterSelectionMenuV2.Instance.PlayerOvered(false, gameObject);

        // Sposto il cursore e resetto pos
        if (currentIndex < 0) return;

        GetComponent<RectTransform>().SetParent(objectsToOver[actualID]);
        GetComponent<RectTransform>().position = GetComponent<RectTransform>().parent.position;

        // Accendo il P relativo
        CharacterSelectionMenuV2.Instance.PlayerOvered(true, gameObject);
    }

    /// <summary>
    /// Filtra la validit� della selezione del personaggio,
    /// poi chiede al menu di aggiornare la selezione nella lista di PlayerSelections
    /// </summary>
    /// <param name="context"></param>
    public override void Submit(InputAction.CallbackContext context)
    {
        if (context.started && confirmBtn != null)
        {
            // Aggiungi la funzione SelectionOver come listener all'evento onClick del pulsante
            confirmBtn.onClick.AddListener(() => CharacterSelectionMenuV2.Instance.SelectionOver(playerID));

            // Esegui il comportamento di selezione normale del pulsante
            confirmBtn.onClick.Invoke();
        }

        if (context.started && GetComponent<RectTransform>().parent != null)
        {
            // Se un'altro personaggio ha gi� selezionato un'oggetto return
            if (CharacterSelectionMenuV2.Instance.AlreadySelected(this))
                return;

            // Se non ho selezionato niente, seleziono
            if (!objectSelected)
            {
                bool response = CharacterSelectionMenuV2.Instance.TriggerPlayerSelection(true, gameObject);

                if (response)
                    objectSelected = true;
            }
        }

        //Check se tutti sono ready
        if (CharacterSelectionMenuV2.Instance.AllReady())
        {
            confirmBtn = CharacterSelectionMenuV2.Instance.TriggerFasciaReady(true);
        }
        else
        {
            confirmBtn = CharacterSelectionMenuV2.Instance.TriggerFasciaReady(false);
        }
    }

    public override void Cancel(InputAction.CallbackContext context)
    {
        if (objectSelected)
        {
            bool response = CharacterSelectionMenuV2.Instance.TriggerPlayerSelection(false, gameObject);

            if (response)
                objectSelected = false;
        }

        //Check se tutti sono ready (serve per sicurezza, per spegnere la fascia)
        if (CharacterSelectionMenuV2.Instance.AllReady())
        {
            confirmBtn = CharacterSelectionMenuV2.Instance.TriggerFasciaReady(true);
        }
        else
        {
            confirmBtn = CharacterSelectionMenuV2.Instance.TriggerFasciaReady(false);
        }
    }

    public override void RandomSelection(InputAction.CallbackContext context)
    {
        if (context.started && !objectSelected)
        {
            CharacterSelectionMenuV2.Instance.SelectRandom(this);
        }
    }

    public override void SetCharacter(ePlayerCharacter character)
    {
        base.SetCharacter(character);
    }
}
