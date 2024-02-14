using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorBehaviour : MonoBehaviour
{
    private Vector2 movement;
    private GameObject selectionParent;

    // Gli oggetti su cui il cursore potrà muoversi
    [HideInInspector] public List<RectTransform> objectsToOver = new List<RectTransform>();
    private int currentIndex = 0;

    public bool objectSelected = false;
    private bool randomBtnSelected = false;

    public void Initialize(List<PlayerSelection> selectableCharacters)
    {
        // Lista di icone
        foreach (PlayerSelection item in selectableCharacters)
        {
            objectsToOver.Add(item.data._icon);
        }
    }

    public void OnCursorMove(InputAction.CallbackContext context)
    {
        if (context.started && randomBtnSelected)
        {
            Debug.LogWarning($"Player {context.control.device.deviceId} non può muoversi poiché ha" +
                $" avviato una votazione per la selezione random");

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
    /// Filtra la validità della selezione del personaggio,
    /// poi chiede al menu di aggiornare la selezione nella lista di PlayerSelections
    /// </summary>
    /// <param name="context"></param>
    public void OnSelectedButton(InputAction.CallbackContext context)
    {
        if (randomBtnSelected && context.started)
        {
            Debug.LogWarning($"Player {context.control.device.deviceId} non può selezionare poiché ha avviato" +
                $" una votazione per la selezione random");

            return;
        }

        if (context.started)
        {
            selectionParent = transform.parent.gameObject;

            // Se un'altro personaggio ha già selezionato un'oggetto return
            if (CharacterSelectionMenu.Instance.AlreadySelected(selectionParent.GetComponent<RectTransform>(), context.control.device)) return;

            // Se non ho selezionato niente, seleziono
            if (!objectSelected)
            {
                objectSelected = true;
                Debug.Log("selected");
            }
            // Se ho già selezionato, deseleziono
            else
            {
                objectSelected = false;
                Debug.Log("Deselected");
            }

            CharacterSelectionMenu.Instance.UpdatePlayerSelection(selectionParent.GetComponent<RectTransform>(), objectSelected, context.control.device);
        }
    }

    /// <summary>
    /// Chiede al menu di aggiornare il numero degli utenti che hanno dato il consenso ad usare la selezione random
    /// </summary>
    /// <param name="context"></param>
    public void OnRandomPressed(InputAction.CallbackContext context)
    {
        if (context.started && !objectSelected)
        {
            randomBtnSelected = !randomBtnSelected;
            CharacterSelectionMenu.Instance.TriggerRandomSelection(randomBtnSelected, context.control.device);
        }

    }
}
