using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] PowerUp powerUpToGive;
    [SerializeField] Character player;

    [SerializeField] PlayerInputManager manager;

    public bool canJoin = false;

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.AddPowerUp(powerUpToGive);
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            player.UnlockUpgrade(AbilityUpgrade.Ability1);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            player.UnlockUpgrade(AbilityUpgrade.Ability2);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            player.UnlockUpgrade(AbilityUpgrade.Ability3);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            player.UnlockUpgrade(AbilityUpgrade.Ability4);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            player.UnlockUpgrade(AbilityUpgrade.Ability5);
        }


        if (canJoin)
        {
           manager.joinAction.action.Enable();
        }
        else
        {
            manager.joinAction.action.Disable();
        }
    }
}
