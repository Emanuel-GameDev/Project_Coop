using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] PowerUp powerUpToGive;
    [SerializeField] Character player;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.AddPowerUp(powerUpToGive);
        }
    }
}
