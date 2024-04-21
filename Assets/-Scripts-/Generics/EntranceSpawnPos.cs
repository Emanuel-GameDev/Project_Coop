using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceSpawnPos : MonoBehaviour
{
    public Transform entranceReferencePoint;

    //[HideInInspector]
    public List<SpawnPosData> posData;

    public EntranceSpawnPos(Transform basePos)
    {
        entranceReferencePoint = basePos;
        Initialize();
    }

    internal void Initialize()
    {
        posData = new List<SpawnPosData>();

        if (entranceReferencePoint != null)
        {
            // prendo i figli della ref pos 

            for (int i = 0; i < entranceReferencePoint.transform.childCount; i++)
            {
                SpawnPosData newSpawnPos = new SpawnPosData(entranceReferencePoint.transform.GetChild(i).position, true);

                posData.Add(newSpawnPos);
            }
        }
        else
        {
            // Creo dei placeholder
            List<Vector3> plaholderPositions = new List<Vector3>() { Vector3.up, Vector3.down, Vector3.right, Vector3.left };

            for (int i = 0; i < PlayerCharacterPoolManager.Instance.AllPlayerCharacters.Count; i++)
            {
                SpawnPosData newSpawnPos = new SpawnPosData(plaholderPositions[i], true);

                posData.Add(newSpawnPos);
            }
        }
    }
}
