using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    SaveData saveData;
    
    public void SaveAllData()
    {
        saveData = new SaveData();

        UpdateLevelsData();
        UpdatePlayersData();

        string filePath = Application.persistentDataPath + "/SaveGames/SaveData.json";
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(filePath, json);

        Debug.Log("Dati PowerUp salvati con successo!");
    }

    public void LoadAllData()
    {
        string filePath = Application.persistentDataPath + "/SaveGames/SaveData.json";
        string json = File.ReadAllText(filePath);
        saveData = JsonUtility.FromJson<SaveData>(json);

        LoadLevelsData();
        LoadPlayersData();

        Debug.Log("Dati PowerUp caricati con successo!");
    }

    public void LoadLevelsData()
    {
        LevelsManager.Instance.LoadLevelData(saveData.levels, saveData.lastLevel);
    }
    public void UpdateLevelsData()
    {
        saveData.levels = LevelsManager.Instance.SaveLevelData();
        saveData.lastLevel = LevelsManager.Instance.currentLevel;
    }

    private void LoadPlayersData()
    {
        foreach (PlayerCharacter player in GameManager.Instance.coopManager.activePlayers)
        {
            player.CharacterClass.LoadClassData(saveData.players.Find(c => c.className == player.CharacterClass.GetType().ToString()));
        }
    }

    private void UpdatePlayersData()
    {
        saveData.players.Clear();

        foreach (PlayerCharacter player in GameManager.Instance.coopManager.activePlayers)
        {
            saveData.players.Add(player.CharacterClass.SaveClassData());
        }
    }

}

[Serializable]
public class SaveData
{
    //Livelli
    public List<LevelData> levels;
    public eLevel lastLevel;

    //Players
    public List<ClassData> players;

}

[Serializable]
public  class LevelData
{
    public eLevel levelName;
    public List<HazardData> hazards;
}

[Serializable]
public class HazardData
{
    public eHazardType hazardName;
    public int position;
    public bool defeated;
}

[Serializable]
public class ClassData
{
    //Statistiche Base
    public string className;
    public PowerUpData powerUpsData;
    public ExtraData extraData;
    public List<AbilityUpgrade> unlockedAbility;
}