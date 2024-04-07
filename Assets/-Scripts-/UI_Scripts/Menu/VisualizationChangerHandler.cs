using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizationChangerHandler : MonoBehaviour
{
    [SerializeField]
    List<List<GameObject>> visualizationObjectsGroups;

    int actualIndex = 0;

    public void ChangeVisualization()
    {
        if (visualizationObjectsGroups.Count > 0)
        {
            DeactivateAll(visualizationObjectsGroups[actualIndex]);

            actualIndex++;

            if (actualIndex >= visualizationObjectsGroups.Count)
            {
                actualIndex = 0;
            }

            ActivateAll(visualizationObjectsGroups[actualIndex]);
        }
    }

    void ActivateAll(List<GameObject> objects)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    void DeactivateAll(List<GameObject> objects)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }
}
