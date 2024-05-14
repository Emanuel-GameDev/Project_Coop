using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSpawnPositionHandler : MonoBehaviour
{
    private static SceneSpawnPositionHandler _instance;
    public static SceneSpawnPositionHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SceneSpawnPositionHandler>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("SceneSpawnPositionHandler");
                    _instance = singletonObject.AddComponent<SceneSpawnPositionHandler>();
                }
            }

            return _instance;
        }
    }

    [SerializeField]
    private SpawnPositionEntrance defaultEntrance;

    [SerializeField]
    private List<SpawnPositionEntrance> spawnPositions;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void AddEntrance(SpawnPositionEntrance entrance)
    {
        spawnPositions.Add(entrance);
    }

    public SpawnPositionEntrance GetSpawnPosition()
    {
        if (defaultEntrance != null)
            return defaultEntrance;
        else
            return CreateEntrance();
    }

    private SpawnPositionEntrance CreateEntrance()
    {
        GameObject newGo = Instantiate(new GameObject("SpawnEntrance"));
        SpawnPositionEntrance entrance = newGo.AddComponent<SpawnPositionEntrance>();
        List<Vector3> plaholderPositions = new List<Vector3>() { Vector3.up, Vector3.down, Vector3.right, Vector3.left };
        foreach (Vector3 position in plaholderPositions)
        {
            Instantiate(new GameObject("SpawnPosition"), position, Quaternion.identity, newGo.transform);
        }
        entrance.Inizialize();
        return entrance;
    }

}
