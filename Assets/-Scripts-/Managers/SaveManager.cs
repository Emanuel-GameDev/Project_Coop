using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    SaveData saveData;
    
    
    
    public void SaveData()
    {
        saveData = new SaveData();
        //saveData.players.Add()

        string filePath = Application.persistentDataPath + "/SaveGames/SaveData.json";
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(filePath, json);

        Debug.Log("Dati PowerUp salvati con successo!");
    }

    public void LoadData()
    {
        string filePath = Application.persistentDataPath + "/SaveGames/SaveData.json";
        string json = File.ReadAllText(filePath);
        saveData = JsonUtility.FromJson<SaveData>(json);

        Debug.Log("Dati PowerUp caricati con successo!");
    }

}

[Serializable]
public class SaveData
{
    //Livelli
    public List<LevelData> levels;


    //Players
    public List<ClassData> players;

}

[Serializable]
public  class LevelData
{
    public string levelName;
    public List<HazardData> hazards;
}

[Serializable]
public class HazardData
{
    public string hazardName;
    public bool defeated;
}

[Serializable]
public class ClassData
{
    //Statistiche Base
    public string className;
    public List<PowerUp> powerUps;
    public List<AbilityUpgrade> unlockedAbility;
    public int unusedKey;

    //Statisiche di Gioco
    public float totalDamageDone;
    public float totalDamageTaken;
    public float totalHealDone;
    public float totalHealReceived;

    public int enemysKilled;
    public int perfectDodgeDone;
    public int perfectGuadDone;
    public int minigameWon;
}