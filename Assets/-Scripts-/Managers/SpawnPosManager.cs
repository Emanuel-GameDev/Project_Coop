using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EntranceSpawnPos
{
    public Transform entranceReferencePoint;

    [HideInInspector]
    public List<SpawnPosData> posData;

    public EntranceSpawnPos(Transform basePos)
    {
        entranceReferencePoint = basePos;
        Initialize();
    }

    internal void Initialize()
    {
        posData = new List<SpawnPosData>();

        for (int i = 0; i < entranceReferencePoint.transform.childCount; i++)
        {
            SpawnPosData newSpawnPos = new SpawnPosData(entranceReferencePoint.transform.GetChild(i), true);

            posData.Add(newSpawnPos);
        }
    }
}

[Serializable]
public class SpawnPosData
{
    [HideInInspector]
    public Vector3 spawnPos;

    public bool free = true;

    public SpawnPosData(Transform spawnPos, bool free)
    {
        this.spawnPos = spawnPos.position;

        this.free = free;
    }
}

public class SpawnPosManager : MonoBehaviour
{
    [SerializeField]
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
        defaultSpawnPos.Initialize();
    }

    internal SpawnPosData GetFreePos()
    {
        if (currentSpawnPos != null)
        {
            Debug.Log(currentSpawnPos.entranceReferencePoint.ToString());
            return currentSpawnPos.posData.Find(x => x.free == true);
        }
        else
            return defaultSpawnPos.posData.Find(x => x.free == true);
    }

    //private void OnDrawGizmos()
    //{
    //    foreach (EntranceSpawnPos data in entrancesBasePos)
    //    {
    //        foreach (SpawnPosData pos in data.posData)
    //        {
    //            Gizmos.color = Color.magenta;
    //            Gizmos.DrawWireSphere(pos.spawnPos, 1f);
    //        }
    //    }

    //    foreach ()
    //}
}
