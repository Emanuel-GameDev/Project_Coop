using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsManager : MonoBehaviour
{
    [SerializeField]
    List<LevelData> levels = new List<LevelData>();

    private static LevelsManager _instance;
    public static LevelsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LevelsManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("LevelsManager");
                    _instance = singletonObject.AddComponent<LevelsManager>();
                }
            }

            return _instance;
        }
    }

    public eLevel currentLevel { get; private set; } = eLevel.Level1;

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

    private void SetCurrentLevel(eLevel level)
    {
        currentLevel = level;
    }

    public eHazardType LoadHazard(int i)
    {
        List<HazardData> hazards = levels.Find(l => l.levelName == currentLevel).hazards;
        return hazards.Find(h => h.position == i).hazardName;
    }


    #region SaveLoadGame
    public List<LevelData> SaveLevelData()
    {
        return levels;
    }

    public void LoadLevelData(List<LevelData> data, eLevel currentLevel)
    {
        levels = data;
        this.currentLevel = currentLevel;
    }

    #endregion


}

[Serializable]
public enum eLevel
{
    Level1,
    Level2,
    Level3
}

[Serializable]
public enum eHazardType
{
    Trap1,
    Trap2,
    Enemies1,
    Enemies2,
    Boss1,
    Boss2,
}