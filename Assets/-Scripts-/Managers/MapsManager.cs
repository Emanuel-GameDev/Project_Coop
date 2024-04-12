using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapsManager : MonoBehaviour
{
    [SerializeField]
    List<MapData> maps = new List<MapData>();

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

    public eMapName currentMap { get; private set; } = eMapName.Map1;

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

    private void SetCurrentLevel(eMapName map)
    {
        currentMap = map;
    }

    public eEncounterType Loadencounter(int i)
    {
        List<EncounterData> encounter = maps.Find(l => l.mapName == currentMap).encounter;
        return encounter.Find(h => h.position == i).encounterName;
    }


    #region SaveLoadGame
    public List<MapData> SaveMapData()
    {
        return maps;
    }

    public void LoadMapData(List<MapData> data, eMapName currentLevel)
    {
        maps = data;
        this.currentMap = currentLevel;
    }

    #endregion


}

[Serializable]
public enum eMapName
{
    Map1,
    Map2,
    Map3
}

[Serializable]
public enum eEncounterType
{
    MinigameStonkMachine,
    MinigamePassepartout,
    Challenge1,
    Challenge2,
    Challenge3,
    Challenge4
}