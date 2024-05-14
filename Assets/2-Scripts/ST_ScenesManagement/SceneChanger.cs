using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    [SerializeField]
    public string destinationSceneName;

    public void ChangeScene()
    {
        GameManager.Instance.ChangeScene(destinationSceneName);
    }
}
