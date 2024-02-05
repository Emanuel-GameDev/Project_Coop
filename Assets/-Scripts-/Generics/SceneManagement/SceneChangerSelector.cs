using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerSelector : MonoBehaviour
{
    public string selectedSceneName;

    public void ChangeScene()
    {
        
        if (!string.IsNullOrEmpty(selectedSceneName))
        {
           SceneManager.LoadScene(selectedSceneName);
        }
        else
        {
            Debug.LogError("Il nome della scena non è stato assegnato.");
        }
    }
}
