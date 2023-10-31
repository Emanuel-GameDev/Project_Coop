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
        if (Input.GetKeyDown(KeyCode.U))
        {
            player.UnlockUpgrade(AbilityUpgrade.Ability1);
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
