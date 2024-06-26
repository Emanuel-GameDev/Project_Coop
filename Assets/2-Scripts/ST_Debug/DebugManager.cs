using System;
using UnityEngine;
using UnityEngine.Rendering;

public class DebugManager : MonoBehaviour
{
    [SerializeField] bool debugMode = false;

    [SerializeField, Range(0, 10), Tooltip("Se la debug mode � attiva premi T per attivare e disattivare il cambio di timescale")]
    float timescale = 1f;
    private bool timeescaleChanged = false;

    [SerializeField] ePlayerCharacter targetCharacter;

    [SerializeField] PowerUp powerUpToGive_7;
    [SerializeField] PowerUp powerUpToGive_8;
    [SerializeField] PowerUp powerUpToGive_9;

    [SerializeField, Tooltip(text)]
    private bool guardaQuestoTooltipPerLeIstruzioni = false;
    [SerializeField]
    string loadSceneName;

    const string text = "DebugMode attiva e disattiva la modalit� di Debug i comandi successivi funzionano solo se � abilitata.\n" +
        "TargetCharacter � il personaggio a cui verrnno dati gli Ability Upgrade o PowerUp.\n" +
        "Per dare un Ability Upgrade, selezionare il targetCharacter a cui darlo e premere il tastierino numerico da 1 a 5.\n" +
        "Per dare un Power Up usare il tastierino numerico 7,8 o 9, verr� assegnato quello corrispondete al numero.\n" + 
        "Con il tasto M si infliggono 1000 danni al personaggio selezionato uccidendolo. \n " +
        "Con N si stampa nella console il numeor di monete e chiavi \n" + " Con il tasto I si cancella i salvataggi. \n" +
        "Con J si completa tutte le sfide per testare il dumpy di fine demo.";

    [SerializeField] GameObject BossGameobject;

    [SerializeField, TextArea(5, 20)]
    private string istructions = text;

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
            if (Input.GetKeyDown(KeyCode.M))
            {
                KillPlayer();
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                ChallengeManager.Instance.selectedChallenge.AutoComplete();
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                SceneSetting sceneSetting = new(SceneSaveSettings.ChallengesSaved);
                sceneSetting.AddBoolValue(SaveDataStrings.COMPLETED, true);
                SaveManager.Instance.SaveSceneData(sceneSetting);
                sceneSetting = new(SceneSaveSettings.Passepartout);
                sceneSetting.AddBoolValue(SaveDataStrings.COMPLETED, true);
                SaveManager.Instance.SaveSceneData(sceneSetting);
                sceneSetting = new(SceneSaveSettings.SlotMachine);
                sceneSetting.AddBoolValue(SaveDataStrings.COMPLETED, true);
                SaveManager.Instance.SaveSceneData(sceneSetting);
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                SaveManager.Instance.ClearSaveData();
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                CharacterSaveData saveData = SaveManager.Instance.GetPlayerSaveData(targetCharacter);
                foreach (PlayerCharacter p in PlayerCharacterPoolManager.Instance.AllPlayerCharacters)
                {
                    p.ExtraData.coin += 9999;
                    p.ExtraData.key += 9999;
                }
                    Debug.Log($"coin: {saveData.extraData.coin}, key: {saveData.extraData.key}");
            }

            if (guardaQuestoTooltipPerLeIstruzioni) guardaQuestoTooltipPerLeIstruzioni = false;
            istructions = text;

            if (Input.GetKeyDown(KeyCode.V))
            {
                if(!string.IsNullOrEmpty(loadSceneName))
                    GameManager.Instance.LoadScene(loadSceneName);
            }
        }
    }

    private void KillPlayer()
    {
        foreach (PlayerCharacter character in PlayerCharacterPoolManager.Instance.ActivePlayerCharacters)
        {
            if (character.Character == targetCharacter)
            {
                character.TakeDamage(new DamageData(1000, null));
            }
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
        SaveManager.Instance.SavePlayersData();
        SaveManager.Instance.SaveData();
    }

    private void LoadGame()
    {
        Debug.Log("CallLoad");
        SaveManager.Instance.LoadData();
        SaveManager.Instance.LoadAllPlayersData();
    }

}
