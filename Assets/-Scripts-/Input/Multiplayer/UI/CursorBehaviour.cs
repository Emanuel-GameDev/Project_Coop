using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorBehaviour : MonoBehaviour
{
    private Vector2 movement;

    [HideInInspector] public List<RectTransform> objectsToOver = new List<RectTransform>();
    private int currentIndex = 0;

    public bool objectSelected = false;

    public void Initialize(List<PlayerSelection> selections)
    {
        foreach (PlayerSelection selection in selections)
        {
            objectsToOver.Add(selection.data._icon);
        }
    }

    public void OnCursorMove(InputAction.CallbackContext context)
    {
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

    public void OnSelectedButton(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            TriggerSelection();
        }
    }

    private void TriggerSelection()
    {
        RectTransform parentRect = transform.parent.gameObject.GetComponent<RectTransform>();

        // Se un'altro personaggio ha già selezionato un'oggetto return
        if (CharacterSelectionMenu.Instance.AlreadySelected(parentRect)) return;

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

        CharacterSelectionMenu.Instance.UpdateSelection(parentRect, objectSelected);
    }
}
