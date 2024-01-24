
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{ 
    public SceneAsset scenaDaCaricare;

    public void CaricaScena()
    {
        if (scenaDaCaricare != null)
        {
            string nomeScena = scenaDaCaricare.name;
            SceneManager.LoadScene(nomeScena);
        }
        else
        {
            Debug.LogError("La variabile scenaDaCaricare non è stata assegnata.");
        }
    }
}

