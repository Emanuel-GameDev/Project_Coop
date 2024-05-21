using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    SaveData saveData = new();

    int saveSlotIndex = 0;

    public string SaveSlot => "SaveData" + saveSlotIndex;

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
        saveData.lastSceneName = SceneManager.GetActiveScene().name;

        string saveFolderPath = Application.persistentDataPath + "/SaveGames";
        string filePath = saveFolderPath + $"/{SaveSlot}.json";

        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(filePath, json);

        Debug.Log("Dati salvati con successo!");
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

    public void SaveSetting(string setting, int value)
    {
        saveData.settings.AddIntValue(setting, value);
        SaveData();
    }

    public void SaveSetting(string setting, bool value)
    {
        saveData.settings.AddBoolValue(setting, value);
        SaveData();
    }

    public void SaveSetting(string setting, string value)
    {
        saveData.settings.AddStringValue(setting, value);
        SaveData();
    }

    public void SaveSetting(string setting, float value)
    {
        saveData.settings.AddFloatValue(setting, value);
        SaveData();
    }

    public void SaveChallenges(List<ChallengeData> data)
    {
        ChallengesSaveData chSaveData = saveData.challenges.Find(x => x.sceneName == SceneManager.GetActiveScene().name);
        if (chSaveData == null)
        {
            chSaveData = new ChallengesSaveData(SceneManager.GetActiveScene().name);
            saveData.challenges.Add(chSaveData);
        }

        foreach (ChallengeData d in data)
        {
            chSaveData.AddChalleges(d);
        }

        SaveData();
    }

    public void SaveChallenge(ChallengeData data)
    {
        ChallengesSaveData chSaveData = saveData.challenges.Find(x => x.sceneName == SceneManager.GetActiveScene().name);
        if (chSaveData != null)
        {
            chSaveData.AddChalleges(data);
        }
        else
        {
            chSaveData = new ChallengesSaveData(SceneManager.GetActiveScene().name);
            chSaveData.AddChalleges(data);
            saveData.challenges.Add(chSaveData);
        }

        SaveData();
    }
    #endregion

    #region Load
    public void LoadData()
    {
        string filePath = Application.persistentDataPath + $"/SaveGames/{SaveSlot}.json";

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

    public bool TryLoadSetting<T>(string setting, out T value)
    {
        Type typeOfT = typeof(T);

        if (typeOfT == typeof(int))
        {
            foreach (IntSetting v in saveData.settings.ints)
            {
                if (v.settingName == setting)
                {
                    value = (T)(object)v.value;
                    return true;
                }
            }
        }
        else if (typeOfT == typeof(bool))
        {
            foreach (BoolSetting v in saveData.settings.bools)
            {
                if (v.settingName == setting)
                {
                    value = (T)(object)v.value;
                    return true;
                }
            }
        }
        else if (typeOfT == typeof(string))
        {
            foreach (StringSetting v in saveData.settings.strings)
            {
                if (v.settingName == setting)
                {
                    value = (T)(object)v.value;
                    return true;
                }
            }
        }
        else if (typeOfT == typeof(float))
        {
            foreach (FloatSetting v in saveData.settings.floats)
            {
                if (v.settingName == setting)
                {
                    value = (T)(object)v.value;
                    return true;
                }
            }
        }

        value = default(T);
        return false;
    }

    public T LoadSetting<T>(string setting)
    {
        return TryLoadSetting<T>(setting, out T value) ? value : default(T);
    }

    public List<ChallengeData> LoadChallenges()
    {
        foreach (ChallengesSaveData data in saveData.challenges)
        {
            if (data.sceneName == SceneManager.GetActiveScene().name)
                return data.savedChalleges;
        }

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

    public void SetSaveSlot(int slot)
    {
        saveSlotIndex = slot;
    }

    public int GetSaveSlot()
    {
        return saveSlotIndex;
    }
}

[Serializable]
public class SaveData
{
    public string lastSceneName;
    public string lastMapZoneSceneName;
    //Settings
    public Settings settings = new();

    //Challenges
    public List<ChallengesSaveData> challenges = new();

    //Players
    public List<CharacterSaveData> players = new();

}

[Serializable]
public class Settings
{
    public List<BoolSetting> bools = new();
    public List<IntSetting> ints = new();
    public List<FloatSetting> floats = new();
    public List<StringSetting> strings = new();

    #region Add
    public void AddBoolValue(string settingName, bool value)
    {
        BoolSetting valueData = bools.Find(x => x.settingName == settingName);
        if (valueData != null)
            valueData.value = value;
        else
            bools.Add(new BoolSetting(settingName, value));
    }

    public void AddIntValue(string settingName, int value)
    {
        IntSetting valueData = ints.Find(x => x.settingName == settingName);
        if (valueData != null)
            valueData.value = value;
        else
            ints.Add(new IntSetting(settingName, value));
    }

    public void AddFloatValue(string settingName, float value)
    {
        FloatSetting valueData = floats.Find(x => x.settingName == settingName);
        if (valueData != null)
            valueData.value = value;
        else
            floats.Add(new FloatSetting(settingName, value));
    }

    public void AddStringValue(string settingName, string value)
    {
        StringSetting valueData = strings.Find(x => x.settingName == settingName);
        if (valueData != null)
            valueData.value = value;
        else
            strings.Add(new StringSetting(settingName, value));
    }

    #endregion
}

[Serializable]
public class BoolSetting
{
    public string settingName;
    public bool value;

    public BoolSetting(string settingName, bool value)
    {
        this.settingName = settingName;
        this.value = value;
    }
}

[Serializable]
public class IntSetting
{
    public string settingName;
    public int value;

    public IntSetting(string settingName, int value)
    {
        this.settingName = settingName;
        this.value = value;
    }
}

[Serializable]
public class FloatSetting
{
    public string settingName;
    public float value;

    public FloatSetting(string settingName, float value)
    {
        this.settingName = settingName;
        this.value = value;
    }
}

[Serializable]
public class StringSetting
{
    public string settingName;
    public string value;

    public StringSetting(string settingName, string value)
    {
        this.settingName = settingName;
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

[Serializable]
public class ChallengesSaveData
{
    public string sceneName;
    public List<ChallengeData> savedChalleges = new();

    public ChallengesSaveData(string sceneName)
    {
        this.sceneName = sceneName;
    }

    public ChallengesSaveData(string sceneName, List<ChallengeData> savedChalleges) : this(sceneName)
    {
        this.savedChalleges = savedChalleges;
    }

    internal void AddChalleges(ChallengeData data)
    {
        ChallengeData chData = savedChalleges.Find(x => x.challengeName == data.challengeName);
        if (chData != null)
        {
            chData.completed = data.completed;
            chData.rank = data.rank;
        }
        else
        {
            savedChalleges.Add(data);
        }
    }
}


[Serializable]
public class ChallengeData
{
    public ChallengeName challengeName;
    public bool completed;
    public int rank;

    public ChallengeData(ChallengeName challengeName, bool completed, int rank)
    {
        this.challengeName = challengeName;
        this.completed = completed;
        this.rank = rank;
    }
}