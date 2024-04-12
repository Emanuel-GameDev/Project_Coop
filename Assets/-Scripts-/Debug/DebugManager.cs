using UnityEngine;
using UnityEngine.Rendering;

public class DebugManager : MonoBehaviour
{
    [SerializeField] bool debugMode = false;

    [SerializeField, Range(0, 10), Tooltip("Se la debug mode è attiva premi T per attivare e disattivare il cambio di timescale")]
    float timescale = 1f;
    private bool timeescaleChanged = false;

    [SerializeField] ePlayerCharacter targetCharacter;

    [SerializeField] PowerUp powerUpToGive_7;
    [SerializeField] PowerUp powerUpToGive_8;
    [SerializeField] PowerUp powerUpToGive_9;

    [SerializeField, Tooltip("DebugMode attiva e disattiva la modalità di Debug i comandi successivi funzionano solo se è abilitata.\n" +
        "TargetCharacter è il personaggio a cui verrnno dati gli Ability Upgrade o PowerUp.\n" +
        "Per dare un Ability Upgrade, selezionare il targetCharacter a cui darlo e premere il tastierino numerico da 1 a 5.\n" +
        "Per dare un Power Up usare il tastierino numerico 7,8 o 9, verrà assegnato quello corrispondete al numero.")]
    private bool guardaQuestoTooltipPerLeIstruzioni = false;

    [SerializeField] GameObject BossGameobject;

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
            if (Input.GetKeyDown(KeyCode.B))
            {
                BossGameobject.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (timeescaleChanged)
                {
                    timeescaleChanged = false;
                    Time.timeScale = 1;
                }
                else
                {
                    timeescaleChanged = true;
                    Time.timeScale = timescale;

                }
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                LoadGame();
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                SaveGame();
            }



            if(guardaQuestoTooltipPerLeIstruzioni) guardaQuestoTooltipPerLeIstruzioni = false;
        }
    }

    private void GivePowerUP(PowerUp powerUpToGive)
    {
        foreach (PlayerCharacter character in PlayerCharacterPoolManager.Instance.ActivePlayerCharacters)
        {
            if (character.Character == targetCharacter)
            {
                character.AddPowerUp(powerUpToGive);
            }
        }

    }

    private void UnlockUpgrade(AbilityUpgrade ability)
    {
        foreach (PlayerCharacter character in PlayerCharacterPoolManager.Instance.ActivePlayerCharacters)
        {
            if (character.Character == targetCharacter)
            {
                character.UnlockUpgrade(ability);
            }
        }
    }

    private void SaveGame()
    {
        Debug.Log("CallSave");
        SaveManager.Instance.SaveAllData();
    }

    private void LoadGame()
    {
        Debug.Log("CallLoad");
        SaveManager.Instance.LoadAllData();
    }

}
