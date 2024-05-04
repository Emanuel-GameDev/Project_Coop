using JetBrains.Annotations;
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
        SaveSceneData(setting, SceneManager.GetActiveScene().name);
    }

    public void SaveSceneData(SceneSetting setting, string sceneName)
    {
        if (setting == null)
            return;

        SceneSaveData sceneData = null;
        foreach(SceneSaveData scene in saveData.sceneData)
        {
            if(scene.sceneName == sceneName)
            {
                sceneData = scene;
                break;
            }
        }

        if(sceneData == null)
        {
            sceneData = new SceneSaveData();
            sceneData.sceneName = sceneName;
            saveData.sceneData.Add(sceneData);
        }

        SceneSetting sceneSetting = null;
        foreach(SceneSetting scSetting in sceneData.sceneSettings)
        {
            if(scSetting.settingName == setting.settingName)
            {
                sceneSetting = scSetting;
                break;
            }
        }

        if(sceneSetting != null)
        {
            sceneData.sceneSettings.Remove(sceneSetting);
        }

        sceneData.sceneSettings.Add(setting);

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
                Debug.Log("Nessun dato nel file di salvataggio.");
        }
        else
        {
            Debug.LogWarning("Il file di salvataggio non esiste.");
        }
    }

    public void LoadAllPlayersData()
    {
        if(saveData == null)
            LoadData();
        
        if(saveData == null || saveData.players == null || saveData.players.Count == 0)
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
        if(saveData == null)
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

    public SceneSaveData GetSceneData()
    {
        return GetSceneData(SceneManager.GetActiveScene().name);
    }

    public SceneSaveData GetSceneData(string sceneName)
    {
        foreach (SceneSaveData sceneData in saveData.sceneData)
        {
            if (sceneData.sceneName == sceneName)
            {
               return sceneData;
            }
        }

        return null;
    }

    public SceneSetting GetSceneSetting(SceneSaveSettings setting)
    {
        return GetSceneData().sceneSettings.Find(x => x.settingName == setting);
    }

    public SceneSetting GetSceneSetting(SceneSaveSettings setting, string sceneName)
    {
        return GetSceneData(sceneName).sceneSettings.Find(x => x.settingName == setting);
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

}

[Serializable]
public class SaveData
{
    //Scene
    public List<SceneSaveData> sceneData = new();
    public string lastScene;

    //Players
    public List<CharacterSaveData> players = new();

}

[Serializable]
public class SceneSaveData
{
    public string sceneName;
    public List<SceneSetting> sceneSettings = new();
}

[Serializable]
public class SceneSetting
{
    public SceneSaveSettings settingName;
    public string stringValue;
    public int intValue;
    public float floatValue;
    public bool boolValue;
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