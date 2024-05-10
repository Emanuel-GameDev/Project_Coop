using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    SaveData saveData = new();

    private static SaveManager _instance;
    public static SaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SaveManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("SaveManager");
                    _instance = singletonObject.AddComponent<SaveManager>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    #region Save
    public void SaveData()
    {
        saveData.lastScene = SceneManager.GetActiveScene().name;

        string saveFolderPath = Application.persistentDataPath + "/SaveGames";
        string filePath = saveFolderPath + "/SaveData.json";

        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(filePath, json);

        Debug.Log("Dati salvati con successo!");
    }

    public void SaveSceneData(SceneSetting setting)
    {
        SceneSetting sceneSetting = saveData.sceneSettings.Find(x => x.settingName == setting.settingName);

        if (sceneSetting != null)
            saveData.sceneSettings.Remove(sceneSetting);
        
        saveData.sceneSettings.Add(setting);

        SaveData();
    }

    public void SavePlayersData()
    {
        saveData.players?.Clear();

        foreach (PlayerCharacter player in PlayerCharacterPoolManager.Instance.AllPlayerCharacters)
        {
            saveData.players.Add(player.GetSaveData());
        }

        SaveData();
    }
    #endregion

    #region Load
    public void LoadData()
    {
        string filePath = Application.persistentDataPath + "/SaveGames/SaveData.json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            saveData = JsonUtility.FromJson<SaveData>(json);

            Debug.Log("Dati caricati con successo!");

            if (saveData == null)
            {
                Debug.Log("Nessun dato nel file di salvataggio.");
                saveData = new();
            }
        }
        else
        {
            Debug.LogWarning("Il file di salvataggio non esiste.");
        }
    }

    public void LoadAllPlayersData()
    {
        if (saveData == null)
            LoadData();

        if (saveData == null || saveData.players == null || saveData.players.Count == 0)
        {
            Debug.Log("Non ci sono dati da caricare.");
            return;
        }

        foreach (PlayerCharacter player in PlayerCharacterPoolManager.Instance.AllPlayerCharacters)
        {
            player.LoadSaveData(saveData.players.Find(c => c.characterName == player.Character));
        }
    }
    #endregion

    #region GetData

    public CharacterSaveData GetPlayerSaveData(ePlayerCharacter character)
    {
        if (saveData == null)
            LoadData();

        if (saveData != null)
        {
            foreach (CharacterSaveData player in saveData.players)
            {
                if (player.characterName == character)
                {
                    return player;
                }
            }
        }

        CharacterSaveData newSaveData = new();
        newSaveData.characterName = character;

        return newSaveData;
    }

    public SceneSetting GetSceneSetting(SceneSaveSettings setting)
    {
        foreach (SceneSetting sceneSetting in saveData.sceneSettings)
            if (sceneSetting.settingName == setting)
                return sceneSetting;
        
        return null;
    }

    #endregion

    #region PlayerPrefs

    public void SavePlayerPref(PlayerPrefsSettings setting, float value)
    {
        PlayerPrefs.SetFloat(setting.ToString(), value);
        PlayerPrefs.Save();
    }

    public void SavePlayerPref(PlayerPrefsSettings setting, int value)
    {
        PlayerPrefs.SetInt(setting.ToString(), value);
        PlayerPrefs.Save();
    }

    public void SavePlayerPref(PlayerPrefsSettings setting, string value)
    {
        PlayerPrefs.SetString(setting.ToString(), value);
        PlayerPrefs.Save();
    }

    public void SavePlayerPref(PlayerPrefsSettings setting, bool value)
    {
        PlayerPrefs.SetInt(setting.ToString(), value ? 1 : 0);
        PlayerPrefs.Save();
    }
    #endregion


    public void ClearSaveData()
    {
        saveData = new();
        SaveData();
        Utility.DebugTrace("Dati Eliminati!");
    }

}

[Serializable]
public class SaveData
{
    //SceneSettings
    public List<SceneSetting> sceneSettings = new();
    public string lastScene;

    //Players
    public List<CharacterSaveData> players = new();

}

[Serializable]
public class SceneSetting
{
    public SceneSaveSettings settingName;
    public List<SavingBoolValue> bools = new();
    public List<SavingIntValue> ints = new();
    public List<SavingFloatValue> floats = new();
    public List<SavingStringValue> strings = new();

    public SceneSetting(SceneSaveSettings settingName)
    {
        this.settingName = settingName;
    }

    #region Add
    public void AddBoolValue(string valueName, bool value)
    {
        SavingBoolValue valueData = bools.Find(x => x.valueName == valueName);
        if (valueData != null)
            valueData.value = value;
        else
            bools.Add(new SavingBoolValue(valueName, value));
    }

    public void AddIntValue(string valueName, int value)
    {
        SavingIntValue valueData = ints.Find(x => x.valueName == valueName);
        if (valueData != null)
            valueData.value = value;
        else
            ints.Add(new SavingIntValue(valueName, value));
    }

    public void AddFloatValue(string valueName, float value)
    {
        SavingFloatValue valueData = floats.Find(x => x.valueName == valueName);
        if (valueData != null)
            valueData.value = value;
        else
            floats.Add(new SavingFloatValue(valueName, value));
    }

    public void AddStringValue(string valueName, string value)
    {
        SavingStringValue valueData = strings.Find(x => x.valueName == valueName);
        if (valueData != null)
            valueData.value = value;
        else
            strings.Add(new SavingStringValue(valueName, value));
    }

    #endregion

    #region Get
    public bool GetBoolValue(string valueName)
    {
        foreach (SavingBoolValue value in bools)
        {
            if (value.valueName == valueName)
                return value.value;
        }

        return false;
    }

    public int GetIntValue(string valueName)
    {
        foreach (SavingIntValue value in ints)
        {
            if (value.valueName == valueName)
                return value.value;
        }

        return 0;
    }

    public float GetFloatValue(string valueName)
    {
        foreach (SavingFloatValue value in floats)
        {
            if (value.valueName == valueName)
                return value.value;
        }

        return 0;
    }

    public string GetStringValue(string valueName)
    {
        foreach (SavingStringValue value in strings)
        {
            if (value.valueName == valueName)
            {
                return value.value;
            }
        }

        return null;
    }
    #endregion
}

[Serializable]
public class SavingBoolValue
{
    public string valueName;
    public bool value;

    public SavingBoolValue(string valueName, bool value)
    {
        this.valueName = valueName;
        this.value = value;
    }
}

[Serializable]
public class SavingIntValue
{
    public string valueName;
    public int value;

    public SavingIntValue(string valueName, int value)
    {
        this.valueName = valueName;
        this.value = value;
    }
}

[Serializable]
public class SavingFloatValue
{
    public string valueName;
    public float value;

    public SavingFloatValue(string valueName, float value)
    {
        this.valueName = valueName;
        this.value = value;
    }
}

[Serializable]
public class SavingStringValue
{
    public string valueName;
    public string value;

    public SavingStringValue(string valueName, string value)
    {
        this.valueName = valueName;
        this.value = value;
    }
}


[Serializable]
public class CharacterSaveData
{
    //Statistiche Base
    public ePlayerCharacter characterName;
    public List<PowerUp> powerUps = new();
    public ExtraData extraData = new();
    public List<AbilityUpgrade> unlockedAbility = new();
}