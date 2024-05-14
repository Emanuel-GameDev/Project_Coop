using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPositionEntrance : MonoBehaviour
{
    public List<SpawnPositionData> posData;

    [SerializeField]
    private Color gizmoColor = Color.blue;


    private void Start()
    {
        Inizialize();
        SceneSpawnPositionHandler.Instance.AddEntrance(this);
    }


    public void Inizialize()
    {
        posData = new List<SpawnPositionData>();
        
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            posData.Add(new(child.position, true));
        }
    }

    public void SetLastEntrance()
    {
        SpawnPositionManager.Instance.AddLastSceneEnrance(this);
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(transform.GetChild(i).position, 0.25f);
        }
    }


}

[Serializable]
public class SpawnPositionData
{
    [HideInInspector]
    public Vector3 spawnPos;

    public bool free = true;

    public SpawnPositionData(Vector3 spawnPos, bool free)
    {
        this.spawnPos = spawnPos;

        this.free = free;
    }
}

