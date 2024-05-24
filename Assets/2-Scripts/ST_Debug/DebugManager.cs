using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

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

    [SerializeField, Tooltip(text)]
    private bool guardaQuestoTooltipPerLeIstruzioni = false;
    [SerializeField]
    string loadSceneName;

    const string text = "DebugMode attiva e disattiva la modalità di Debug i comandi successivi funzionano solo se è abilitata.\n" +
        "[Tastierino Numerico 1-5] Assegna Ability Upgrade al Target Character.\n" +
        "[Tastierino Numerico 7,8 o 9] Verrà assegnato il PowerUp corrispondete al numero al Target Character.\n" +
        "[M] Infligge 1000 danni al Target Character. \n" +
        "[N] Assegna 9999 monete e chiavi al Target Character. \n" +
        "[I] Cancella i salvataggi. \n" +
        "[J] Completa tutte le sfide per testare il dumpy di fine demo. \n" +
        "[O] Apre la cartella dei salvataggi. \n" +
        "[L e K] Rispettivamente Salva e Carica il gioco. \n" +
        "[V] Carica la scena scritta in Loasd Scene Name. \n" +
        "[G] Completa la Challenge attuale (solo in ChallengeScene). \n";

    [SerializeField] GameObject BossGameobject;

    [SerializeField, TextArea(5, 100)]
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
                if (SceneManager.GetActiveScene().name == "ChallengeScene")
                    ChallengeManager.Instance.selectedChallenge.AutoComplete();
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                SaveManager.Instance.SaveSetting(SaveDataStrings.PASSEPARTOUT_MINIGAME_COMPLETED, true);
                SaveManager.Instance.SaveSetting(SaveDataStrings.FOOLSLOT_MINIGAME_COMPLETED, true);
                SaveManager.Instance.SaveSetting("AllFirstZoneChallengesCompleted", true);
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
                if (!string.IsNullOrEmpty(loadSceneName))
                    GameManager.Instance.LoadScene(loadSceneName);
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                string saveFolderPath = Application.persistentDataPath + "/SaveGames";
                if (Directory.Exists(saveFolderPath))
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = saveFolderPath,
                        UseShellExecute = true,
                        Verb = "open"
                    });
                }
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
