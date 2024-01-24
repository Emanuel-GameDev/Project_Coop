using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoinBehaviour : MonoBehaviour
{
    public int numberOfActivePlayers { get; private set; } = 0;

    void Start()
    {
        InputAction joinAction = new InputAction(binding: "/*/<button>");

        joinAction.started += (joinAction) =>
        {
            AddPlayer(joinAction.control.device);
        };

        joinAction.Enable();
    }

    private void AddPlayer(InputDevice device)
    {
        // Non esegue se il dispositivo è già associato ad un giocatore

        foreach (PlayerInput player in PlayerInput.all)
        {
            foreach (InputDevice playerDevice in player.devices)
            {
                if (device == playerDevice)
                    return;
            }
        }

        // Non esegue se non rientra nei parametri di dispositivi
        if (!device.displayName.Contains("Controller") && !device.displayName.Contains("Joystick")
            && !device.displayName.Contains("Gamepad") && !device.displayName.Contains("Keyboard"))
            return;

        int playerNumToAdd = PlayerInput.all.Count + 1;
        string controlScheme = "";

        if (device.displayName.Contains("Controller") || device.displayName.Contains("Gamepad"))
            controlScheme = "Gamepad";
        if (device.displayName.Contains("Joystick"))
            controlScheme = "Joystick";
        if (device.displayName.Contains("Keyboard"))
            controlScheme = "Keyboard";

        GameObject playerCursor = CharacterSelectionMenu.Instance.LoadCursor(playerNumToAdd);

        if (!playerCursor.activeInHierarchy)
        {
            PlayerInput cursor = PlayerInput.Instantiate(playerCursor, -1, controlScheme, -1, device);
            cursor.transform.SetParent(CharacterSelectionMenu.Instance.characterIcons[0]._icon.gameObject.GetComponent<RectTransform>());
            cursor.gameObject.GetComponent<RectTransform>().position = cursor.transform.parent.gameObject.GetComponent<RectTransform>().position;
            cursor.transform.localScale = new Vector3(1f, 1f, 1f);
        }

    }

    public void OnPlayerJoin()
    {
        numberOfActivePlayers = PlayerInput.all.Count;
        Debug.Log("New Player Joined! Total player number: " + numberOfActivePlayers);
    }

    public void OnPlayerLeft()
    {
        numberOfActivePlayers = PlayerInput.all.Count;
        Debug.Log("Player left the game. Total player number: " + numberOfActivePlayers);
    }
}
