using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPositionManager : MonoBehaviour
{
    private static SpawnPositionManager _instance;
    public static SpawnPositionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SpawnPositionManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("SpawnPositionManager");
                    _instance = singletonObject.AddComponent<SpawnPositionManager>();
                }
            }

            return _instance;
        }
    }

    private Dictionary<string, SpawnPositionEntrance> scenesEntrances = new();

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

    internal SpawnPositionData GetFreePos()
    {
        Utility.DebugTrace("Chiamata");
        
        string sceneName = SceneManager.GetActiveScene().name;

        SpawnPositionEntrance entrance = null;

        if (scenesEntrances.ContainsKey(sceneName))
            entrance = scenesEntrances[sceneName];
        else
            entrance = SceneSpawnPositionHandler.Instance.GetSpawnPosition();

        SpawnPositionData spawnPos;

        spawnPos = entrance.posData.Find(x => x.free == true);
        spawnPos.free = false;
        return spawnPos;
    }

    public void AddLastSceneEnrance(SpawnPositionEntrance entrance)
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (scenesEntrances.ContainsKey(sceneName))
        {
            scenesEntrances[sceneName] = entrance;
        }
        else
        {
            scenesEntrances.Add(sceneName, entrance);
        }
    }

}
