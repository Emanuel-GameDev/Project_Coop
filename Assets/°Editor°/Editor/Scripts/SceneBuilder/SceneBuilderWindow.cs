using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBuilderWindow : EditorWindow
{
    private List<GameObject> prefabsToInstantiate = new List<GameObject>();

    [MenuItem("Tools/Scene Builder")]
    private static void OpenWindow()
    {
        SceneBuilderWindow window = GetWindow<SceneBuilderWindow>();
        window.titleContent = new GUIContent("Scene Builder");
        window.ScanAndDisplayPrefabList();
    }

    private void OnGUI()
    {
        GUIStyle boldStyle = new GUIStyle(EditorStyles.boldLabel);
        boldStyle.fontSize = 14; // Imposta la dimensione del carattere

        EditorGUILayout.LabelField("Base Prefabs:", boldStyle);

        foreach (GameObject prefab in prefabsToInstantiate)
        {
            EditorGUILayout.BeginHorizontal();

            bool isPrefabInScene = CheckIfPrefabInScene(prefab);

            GUIStyle style = new GUIStyle();
            style.normal.textColor = isPrefabInScene ? Color.green : Color.red;

            EditorGUILayout.LabelField(isPrefabInScene ? "✔" : "✘", style, GUILayout.Width(10));

            EditorGUILayout.LabelField(prefab.name);

            if (!isPrefabInScene && GUILayout.Button("Instantiate", GUILayout.Width(80)))
            {
                InstantiatePrefab(prefab);
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Instantiate missing prefabs"))
        {
            foreach (GameObject prefab in prefabsToInstantiate)
            {
                if (!CheckIfPrefabInScene(prefab))
                {
                    InstantiatePrefab(prefab);
                }
            }
        }
    }

    private void ScanAndDisplayPrefabList()
    {
        prefabsToInstantiate.Clear();

        string[] startingElements = AssetDatabase.FindAssets("l:StartingElements");

        foreach (string element in startingElements)
        {
            string elementPath = AssetDatabase.GUIDToAssetPath(element);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(elementPath);

            if (prefab != null)
            {
                prefabsToInstantiate.Add(prefab);
            }
        }

        Repaint();
    }

    private bool CheckIfPrefabInScene(GameObject prefab)
    {
        GameObject[] sceneObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject sceneObject in sceneObjects)
        {
            GameObject prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(sceneObject);

            if (prefabRoot == prefab)
            {
                return true;
            }
        }

        return false;
    }

    private void InstantiatePrefab(GameObject prefab)
    {
        GameObject newPrefabInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

        if (newPrefabInstance != null)
        {
            Debug.Log(prefab.name + " è stato istanziato nella scena.");
        }
        else
        {
            Debug.LogError("Impossibile istanziare " + prefab.name + " nella scena.");
        }

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }
}
