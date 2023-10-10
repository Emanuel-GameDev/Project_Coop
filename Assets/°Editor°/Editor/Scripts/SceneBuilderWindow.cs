using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SceneBuilderWindow : EditorWindow
{
    [MenuItem("Tools/Scene Builder")]
    private static void OpenWindow()
    {
        GetWindow<SceneBuilderWindow>();
    }

    private Dictionary<string, GameObject> instantiatedObjects = new Dictionary<string, GameObject>();

    private void OnGUI()
    {
        if (GUILayout.Button("Initialize Scene"))
        {
            string[] startingElements = AssetDatabase.FindAssets("l:StartingElements");
            foreach (string element in startingElements)
            {
                // Ottieni il percorso dell'asset
                string assetPath = AssetDatabase.GUIDToAssetPath(element);

                // Controlla se l'oggetto è già stato istanziato
                if (instantiatedObjects.ContainsKey(assetPath))
                {
                    GameObject existingObject = instantiatedObjects[assetPath];

                    // Controlla se l'oggetto esiste ancora nella scena
                    if (existingObject != null)
                    {
                        // Riutilizza l'oggetto esistente
                        Selection.activeGameObject = existingObject;
                        continue; // Vai al prossimo elemento
                    }
                    else
                    {
                        // Rimuovi l'associazione se l'oggetto non esiste più
                        instantiatedObjects.Remove(assetPath);
                    }
                }

                // Se siamo arrivati a questo punto, istanzia un nuovo oggetto
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                GameObject instantiatedObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

                // Aggiungi l'oggetto alla lista degli istanziati
                instantiatedObjects.Add(assetPath, instantiatedObject);
            }
        }
    }
}
