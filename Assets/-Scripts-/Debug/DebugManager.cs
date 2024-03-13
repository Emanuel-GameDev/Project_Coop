using System;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] bool debugMode = false;

    [SerializeField] ePlayerCharacter targetCharacter;

    [SerializeField] PowerUp powerUpToGive_7;
    [SerializeField] PowerUp powerUpToGive_8;
    [SerializeField] PowerUp powerUpToGive_9;

    [SerializeField, Tooltip("DebugMode attiva e disattiva la modalità di Debug i comandi successivi funzionano solo se è abilitata.\n" +
        "TargetCharacter è il personaggio a cui verrnno dati gli Ability Upgrade o PowerUp.\n" +
        "Per dare un Ability Upgrade, selezionare il targetCharacter a cui darlo e premere il tastierino numerico da 1 a 5.\n" +
        "Per dare un Power Up usare il tastierino numerico 7,8 o 9, verrà assegnato quello corrispondete al numero.")]
    private bool guardaQuestoTooltipPerLeIstruzioni = false;



    private void Update()
    {
        if (debugMode)
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                UnlockUpgrade(AbilityUpgrade.Ability1);
            }

            if (Input.GetKeyDown(KeyCode.Keypad2)) 
            { 
                UnlockUpgrade(AbilityUpgrade.Ability2); 
            }

            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                UnlockUpgrade(AbilityUpgrade.Ability3);
            }

            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                UnlockUpgrade(AbilityUpgrade.Ability4);
            }

            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                UnlockUpgrade(AbilityUpgrade.Ability5);
            }

            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                GivePowerUP(powerUpToGive_7);
            }

            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                GivePowerUP(powerUpToGive_8);
            }

            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                GivePowerUP(powerUpToGive_9);
            }

        }
    }

    private void GivePowerUP(PowerUp powerUpToGive)
    {
        List<PlayerInputHandler> players = CoopManager.Instance.GetActiveHandlers();
        if (players != null && players.Count > 0)
        {
            foreach (PlayerInputHandler player in CoopManager.Instance.GetActiveHandlers())
            {
                if (player.currentCharacter == targetCharacter && player.CurrentReceiver is PlayerCharacter character)
                {
                    character.AddPowerUp(powerUpToGive);
                }
            }
        }
    }

    private void UnlockUpgrade(AbilityUpgrade ability)
    {
        List<PlayerInputHandler> players = CoopManager.Instance.GetActiveHandlers();
        if ( players != null && players.Count > 0)
        {
            foreach (PlayerInputHandler player in players)
            {
                if (player.currentCharacter == targetCharacter && player.CurrentReceiver is PlayerCharacter character)
                {
                    character.UnlockUpgrade(ability);
                }
            }
        }
    }



    //[Header("Debug Items")]
    //[SerializeField] GameObject debugCanvas;
    //[SerializeField] TMP_Text debugText;



    //public void PrintOnConsole(string text)
    //{

    //}


}
