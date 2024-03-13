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

    private void OnTriggerEnter2D(Collider2D other)
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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<IInteracter>() != null)
        {
            objectCount--;
            if (objectCount <= 0)
            {
                objectCount = 0;
                if(gameObject.activeInHierarchy)
                    StartCoroutine(UnloadSceneAfterDelay());
            }
        }
    }

    IEnumerator UnloadSceneAfterDelay()
    {
        yield return new WaitForSeconds(unloadSceneDelay);
        if(objectCount <= 0)
        {
            //GameManager.Instance.CancelSceneLoad(destinationSceneName);
        }
    }


    public void ChangeScene()
    {
        GameManager.Instance.ChangeScene(destinationSceneName);
    }


}
