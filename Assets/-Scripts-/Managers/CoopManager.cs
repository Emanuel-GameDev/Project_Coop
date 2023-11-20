using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoopManager : MonoBehaviour
{
    [SerializeField] Character character;

    [SerializeField] private CharacterData switchPlayerUp;
    [SerializeField] private CharacterData switchPlayerRight;
    [SerializeField] private CharacterData switchPlayerDown;
    [SerializeField] private CharacterData switchPlayerLeft;


    public void Switch1_performed(InputAction.CallbackContext context)
    {
        Debug.Log(context.action);
        if(context.performed)
            character.SetCharacterData(switchPlayerUp);
    }

    public void Switch2_performed(InputAction.CallbackContext context)
    {
        Debug.Log(context.action);
        if (context.performed)
            character.SetCharacterData(switchPlayerRight);
    }

    public void Switch3_performed(InputAction.CallbackContext context)
    {
        Debug.Log(context.action);
        if (context.performed)
            character.SetCharacterData(switchPlayerDown);
    }
    public void Switch4_performed(InputAction.CallbackContext context)
    {
        Debug.Log(context.action);
        if (context.performed)
            character.SetCharacterData(switchPlayerLeft);
    }

}
