using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneChangerSelector))]
public class SceneChangerSelectorEditor : Editor
{
    SerializedProperty selectedSceneNameProp;

    InBuildScenes inBuildScenes;
    NotBuildScenes notBuildScenes;
    int selectedInBuildIndex = 0;
    int selectedNotBuildIndex = 0;
    bool useNotBuildScenes = false;

    void OnEnable()
    {
        selectedSceneNameProp = serializedObject.FindProperty("selectedSceneName");
        LoadScriptableObjects();
    }

    void LoadScriptableObjects()
    {
        inBuildScenes = AssetDatabase.LoadAssetAtPath<InBuildScenes>("Assets/-Scripts-/Generics/SceneManagement/InBuildScenes.asset");
        notBuildScenes = AssetDatabase.LoadAssetAtPath<NotBuildScenes>("Assets/-Scripts-/Generics/SceneManagement/NotBuildScenes.asset");
    }

    public override void OnInspectorGUI()
    {
        // Aggiorna la rappresentazione seriale dell'oggetto target
        serializedObject.Update();

        // Personalizza la visualizzazione per selectedSceneName
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Selected Scene: {selectedSceneNameProp.stringValue}", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();

        bool showBuildList = inBuildScenes != null && inBuildScenes.scenesInBuild.Count > 0;
        bool showNotBuildList = notBuildScenes != null && notBuildScenes.scenesNotInBuild.Count > 0;

        if (showBuildList && showNotBuildList)
        {
            useNotBuildScenes = EditorGUILayout.Toggle("Use NotBuild Scenes", useNotBuildScenes);

            selectedInBuildIndex = EditorGUILayout.Popup("Select from InBuild List", selectedInBuildIndex, inBuildScenes.scenesInBuild.ToArray());
            selectedNotBuildIndex = EditorGUILayout.Popup("Select from NotBuild List", selectedNotBuildIndex, notBuildScenes.scenesNotInBuild.ToArray());

            if (!useNotBuildScenes)
                selectedSceneNameProp.stringValue = inBuildScenes.scenesInBuild[selectedInBuildIndex];
            else
                selectedSceneNameProp.stringValue = notBuildScenes.scenesNotInBuild[selectedNotBuildIndex];
        }
        else
        {
            if (showBuildList)
            {
                selectedInBuildIndex = EditorGUILayout.Popup("Select from InBuild List", selectedInBuildIndex, inBuildScenes.scenesInBuild.ToArray());
                selectedSceneNameProp.stringValue = inBuildScenes.scenesInBuild[selectedInBuildIndex];
            }

            if (showNotBuildList)
            {
                selectedNotBuildIndex = EditorGUILayout.Popup("Select from NotBuild List", selectedNotBuildIndex, notBuildScenes.scenesNotInBuild.ToArray());
                selectedSceneNameProp.stringValue = notBuildScenes.scenesNotInBuild[selectedNotBuildIndex];
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
