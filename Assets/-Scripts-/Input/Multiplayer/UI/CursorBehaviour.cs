using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorBehaviour : MonoBehaviour
{
    private Vector2 movement;
    
    [HideInInspector] public List<RectTransform> objectsToOver = new List<RectTransform>();
    private int currentIndex = -1;

    public bool objectSelected = false;
    private GameObject playerSelection;

    public void Initialize(CharacterSelectionMenu menu)
    {
        objectsToOver = menu.characterIcons;
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
            GameObject parentIcon = transform.parent.gameObject;
            
            if (!objectSelected)
            {
                objectSelected = true;
                playerSelection = parentIcon;
                Debug.Log("selected");
            }
            else
            {
                objectSelected = false;
                playerSelection = null;
                Debug.Log("Deselected");
            }

        }
    }
}
