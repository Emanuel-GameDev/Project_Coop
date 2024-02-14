using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerTrigger : MonoBehaviour
{
    [SerializeField]
    private string destinationSceneName;

    [SerializeField]
    private float unloadSceneDelay = 10f;

    private int objectCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<IInteracter>() != null)
        {
            if(objectCount == 0)
            {
                GameManager.Instance.LoadSceneInbackground(destinationSceneName);
            }
            objectCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<IInteracter>() != null)
        {
            objectCount--;
            if (objectCount <= 0)
            {
                objectCount = 0;
                StartCoroutine(UnloadSceneAfterDelay());
            }
        }
    }

    IEnumerator UnloadSceneAfterDelay()
    {
        yield return new WaitForSeconds(unloadSceneDelay);
        if(objectCount <= 0)
        {
            GameManager.Instance.CancelSceneLoad(destinationSceneName);
        }
    }


    public void ChangeScene()
    {
        GameManager.Instance.ChangeScene(destinationSceneName);
    }


}
