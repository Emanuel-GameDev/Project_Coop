using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    SaveData saveData;

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

    public void SaveAllData()
    {
        saveData = new SaveData();

        //UpdateMapsData();
        UpdatePlayersData();

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

    public void LoadAllData()
    {
        string filePath = Application.persistentDataPath + "/SaveGames/SaveData.json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            saveData = JsonUtility.FromJson<SaveData>(json);
            if (saveData != null)
            {
                // LoadMapsData();
                LoadPlayersData();
            }

            Debug.Log("Dati caricati con successo!");
        }
        else
        {
            Debug.LogWarning("Il file di salvataggio non esiste.");
        }
    }

    public void LoadMapsData()
    {
        MapsManager.Instance.LoadMapData(saveData.maps, saveData.lastMap);
    }

    public void UpdateMapsData()
    {
        saveData.maps = MapsManager.Instance.SaveMapData();
        saveData.lastMap = MapsManager.Instance.currentMap;
    }

    private void LoadPlayersData()
    {
        foreach (PlayerCharacter player in PlayerCharacterPoolManager.Instance.AllPlayerCharacters)
        {
            player.LoadSaveData(saveData.players.Find(c => c.characterName == player.Character));
        }
    }

    private void UpdatePlayersData()
    {
        saveData.players?.Clear();

        foreach (PlayerCharacter player in PlayerCharacterPoolManager.Instance.AllPlayerCharacters)
        {
            saveData.players.Add(player.GetSaveData());
        }
    }

    public CharacterSaveData GetPlayerSaveData(ePlayerCharacter character)
    {
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
    //Mappe
    public List<MapData> maps;
    public eMapName lastMap;

    //Players
    public List<CharacterSaveData> players = new();

}

[Serializable]
public class MapData
{
    public eMapName mapName;
    public List<EncounterData> encounter = new();
}

[Serializable]
public class EncounterData
{
    public eEncounterType encounterName;
    public int position;
    public bool defeated;
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

public enum PlayerPrefsSettings
{
    FirstStart,
    Languge,
    MasterVolume,
    MusicVolume,
    SFXVolume
}
