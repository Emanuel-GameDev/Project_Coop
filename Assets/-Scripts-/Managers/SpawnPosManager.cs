using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnPosData
{
    [HideInInspector]
    public Vector3 spawnPos;

    public bool free = true;

    public SpawnPosData(Vector3 spawnPos, bool free)
    {
        this.spawnPos = spawnPos;

        this.free = free;
    }
}

public class SpawnPosManager : MonoBehaviour
{
    [SerializeField]
    private Transform startingSpawnPos;

    private EntranceSpawnPos defaultSpawnPos;
    private EntranceSpawnPos currentSpawnPos;

    public static SpawnPosManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ActivateEntrance(Transform entrancePos)
    {
        EntranceSpawnPos entrance = new EntranceSpawnPos(entrancePos);
        currentSpawnPos = entrance;
    }

    private void Start()
    {
        CheckStartingPos();
    }

    internal SpawnPosData GetFreePos()
    {
        SpawnPosData spawnPos;

        if (currentSpawnPos != null)
        {
            spawnPos = currentSpawnPos.posData.Find(x => x.free == true);
            spawnPos.free = false;
            return spawnPos;
        }
        else
        {
            CheckStartingPos();

            spawnPos = defaultSpawnPos.posData.Find(x => x.free == true);
            spawnPos.free = false;
            return spawnPos;
        }
    }

    private void CheckStartingPos()
    {
        if (startingSpawnPos == null)
        {
            GameObject newGO = new GameObject("StartSpawnPos_Added");
            defaultSpawnPos = newGO.AddComponent<EntranceSpawnPos>();

            defaultSpawnPos.Initialize();
        }
        else if (defaultSpawnPos == null)
        {
            defaultSpawnPos = startingSpawnPos.gameObject.AddComponent<EntranceSpawnPos>();
            defaultSpawnPos.entranceReferencePoint = startingSpawnPos;
            defaultSpawnPos.Initialize();
        }
    }
}
