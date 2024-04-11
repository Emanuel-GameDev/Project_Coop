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

        UpdateMapsData();
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

        LoadMapsData();
        LoadPlayersData();

        Debug.Log("Dati PowerUp caricati con successo!");
    }

    public void LoadMapsData()
    {
        MapsManager.Instance.LoadLevelData(saveData.levels, saveData.lastLevel);
    }
    public void UpdateMapsData()
    {
        saveData.levels = MapsManager.Instance.SaveLevelData();
        saveData.lastLevel = MapsManager.Instance.currentLevel;
    }

    private void LoadPlayersData()
    {
        foreach (PlayerCharacter player in PlayerCharacterPoolManager.Instance.ActivePlayerCharacters)
        {
            player.LoadSaveData(saveData.players.Find(c => c.characterName == player.Character));
        }
    }

    private void UpdatePlayersData()
    {
        saveData.players.Clear();

        foreach (PlayerCharacter player in PlayerCharacterPoolManager.Instance.ActivePlayerCharacters)
        {
            saveData.players.Add(player.GetSaveData());
        }
    }

}

[Serializable]
public class SaveData
{
    //Livelli
    public List<MapData> levels;
    public eMapName lastLevel;

    //Players
    public List<CharacterSaveData> players;

}

[Serializable]
public  class MapData
{
    public eMapName mapName;
    public List<EncounterData> hazards;
}

[Serializable]
public class EncounterData
{
    public eEncounterType hazardName;
    public int position;
    public bool defeated;
}

[Serializable]
public class CharacterSaveData
{
    //Statistiche Base
    public ePlayerCharacter characterName;
    public PowerUpData powerUpsData;
    public ExtraData extraData;
    public List<AbilityUpgrade> unlockedAbility;
}