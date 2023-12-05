using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class CharacterSelectionMenu : MonoBehaviour
{
    public void Navigate(InputAction.CallbackContext context)
    {
        Debug.Log("ewdgfds");
        Debug.Log(context.ReadValue<Vector2>());
    }
}
