using System.Collections.Generic;
using UnityEngine;

public class VisualizationChangerHandler : MonoBehaviour, IVisualizationChanger
{
    [SerializeField, ReorderableList, Tooltip("Lista dei gruppi da cambiare ad ogni pressione del tasto")]
    List<GameObject> visualizationObjects;

    int actualIndex = 0;
    GameObject actualObject;

    private void Awake()
    {
        foreach (GameObject visualizationObject in visualizationObjects)
        {
            Deactivate(visualizationObject);
        }
        Activate(visualizationObjects[actualIndex]);
    }


    public void ChangeVisualization()
    {
        if (visualizationObjects.Count > 0)
        {
            Deactivate(visualizationObjects[actualIndex]);

            actualIndex++;

            if (actualIndex >= visualizationObjects.Count)
            {
                actualIndex = 0;
            }

            Activate(visualizationObjects[actualIndex]);

            Debug.Log("ChangeVisualization: " + actualIndex);
        }
    }

    void Activate(GameObject objectToActivate)
    {
        objectToActivate.SetActive(true);
        actualObject = objectToActivate;
    }

    void Deactivate(GameObject objectToActivate)
    {
        objectToActivate.SetActive(false);
        actualObject = null;
    }

    public void ChangeToObject(GameObject objectToActivate)
    {
        if (actualObject != objectToActivate)
        {
            if (visualizationObjects.Contains(actualObject))
            {
                while (actualObject != objectToActivate)
                {
                    ChangeVisualization();
                }
            }
        }
    }

    public void ChangeToDefault()
    {
        ChangeToObject(visualizationObjects[0]);
    }

}