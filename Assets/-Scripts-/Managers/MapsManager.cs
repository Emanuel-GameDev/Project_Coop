using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapsManager : MonoBehaviour
{
    [SerializeField]
    List<MapData> levels = new List<MapData>();

    private static MapsManager _instance;
    public static MapsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MapsManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("LevelsManager");
                    _instance = singletonObject.AddComponent<MapsManager>();
                }
            }

            return _instance;
        }
    }

    public eMapName currentLevel { get; private set; } = eMapName.Level1;

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

    private void SetCurrentLevel(eMapName level)
    {
        currentLevel = level;
    }

    public eEncounterType LoadHazard(int i)
    {
        List<EncounterData> hazards = levels.Find(l => l.mapName == currentLevel).hazards;
        return hazards.Find(h => h.position == i).hazardName;
    }


    #region SaveLoadGame
    public List<MapData> SaveLevelData()
    {
        return levels;
    }

    public void LoadLevelData(List<MapData> data, eMapName currentLevel)
    {
        levels = data;
        this.currentLevel = currentLevel;
    }

    #endregion


}

[Serializable]
public enum eMapName
{
    Level1,
    Level2,
    Level3
}

[Serializable]
public enum eEncounterType
{
    Trap1,
    Trap2,
    Enemies1,
    Enemies2,
    Boss1,
    Boss2,
}