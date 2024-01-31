using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildSceneManager : EditorWindow
{
    private InBuildScenes inBuildScenes;
    private NotBuildScenes notBuildScenes;
    private List<string> allScenesInProject;

    [MenuItem("Tools/Build Scene Manager")]
    public static void ShowWindow()
    {
        GetWindow<BuildSceneManager>("Build Scene Manager");
    }

    private void OnEnable()
    {
        inBuildScenes = AssetDatabase.LoadAssetAtPath<InBuildScenes>("Assets/-Scripts-/Generics/SceneManagement/InBuildScenes.asset");
        notBuildScenes = AssetDatabase.LoadAssetAtPath<NotBuildScenes>("Assets/-Scripts-/Generics/SceneManagement/NotBuildScenes.asset");

        if (inBuildScenes == null)
        {
            inBuildScenes = ScriptableObject.CreateInstance<InBuildScenes>();
            AssetDatabase.CreateAsset(inBuildScenes, "Assets/-Scripts-/Generics/SceneManagement/InBuildScenes.asset");
            AssetDatabase.SaveAssets();
        }

        if (notBuildScenes == null)
        {
            notBuildScenes = ScriptableObject.CreateInstance<NotBuildScenes>();
            AssetDatabase.CreateAsset(notBuildScenes, "Assets/-Scripts-/Generics/SceneManagement/NotBuildScenes.asset");
            AssetDatabase.SaveAssets();
        }

        // Ottieni la lista di tutte le scene nel progetto
        allScenesInProject = GetScenePathsInProject();

        // Distribuisci le scene tra le due liste
        DistributeScenes();
    }

    private List<string> GetScenePathsInProject()
    {
        List<string> scenePaths = new List<string>();

        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string assetPath in allAssetPaths)
        {
            if (assetPath.EndsWith(".unity") && !assetPath.Contains("/Editor/"))
            {
                scenePaths.Add(assetPath);
            }
        }

        return scenePaths;
    }

    private void DistributeScenes()
    {
        // Distribuisci le scene tra le due liste
        foreach (string scenePath in allScenesInProject)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (!inBuildScenes.scenesInBuild.Contains(sceneName) && !notBuildScenes.scenesNotInBuild.Contains(sceneName))
            {
                if (EditorBuildSettings.scenes.Any(scene => scene.path == scenePath))
                {
                    inBuildScenes.scenesInBuild.Add(sceneName);
                }
                else
                {
                    notBuildScenes.scenesNotInBuild.Add(sceneName);
                }
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Build Scene Manager", EditorStyles.boldLabel);

        DisplaySceneList("Scenes In Build", inBuildScenes.scenesInBuild);
        DisplaySceneList("Scenes Not In Build", notBuildScenes.scenesNotInBuild);

        GUILayout.Space(20);

        if (GUILayout.Button("Reload"))
        {
            Reload();
        }
    }

    private void Reload()
    {
        inBuildScenes.scenesInBuild.Clear();
        notBuildScenes.scenesNotInBuild.Clear();
        allScenesInProject = GetScenePathsInProject();
        DistributeScenes();

        EditorUtility.SetDirty(inBuildScenes);
        EditorUtility.SetDirty(notBuildScenes);
        AssetDatabase.SaveAssets();

        Debug.Log("Changes applied!");
    }

    private void DisplaySceneList(string label, List<string> sceneList)
    {
        GUILayout.Label(label, EditorStyles.boldLabel);

        // Utilizza GUILayout per visualizzare la lista delle scene
        for (int i = 0; i < sceneList.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20); // Aggiunge uno spazio di 20 pixel di rientro
            GUILayout.Label(System.IO.Path.GetFileNameWithoutExtension(sceneList[i]), EditorStyles.label);
            GUILayout.EndHorizontal();
        }
    }
}
