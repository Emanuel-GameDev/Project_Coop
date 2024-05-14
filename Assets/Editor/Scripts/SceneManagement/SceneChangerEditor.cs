using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(SceneChanger))]
public class SceneChangerEditor : Editor
{
    SerializedProperty destinationSceneNameProp;
    string[] sceneNames;
    int selectedSceneIndex = 0;

    void OnEnable()
    {
        destinationSceneNameProp = serializedObject.FindProperty("destinationSceneName");
        sceneNames = GetSceneNames();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SceneChanger sceneChanger = (SceneChanger)target;

        EditorGUILayout.PropertyField(destinationSceneNameProp);

        selectedSceneIndex = EditorGUILayout.Popup("Select Destination Scene", selectedSceneIndex, sceneNames);

        if (GUILayout.Button("Set Destination Scene"))
        {
            if (selectedSceneIndex >= 0 && selectedSceneIndex < sceneNames.Length)
            {
                destinationSceneNameProp.stringValue = sceneNames[selectedSceneIndex];
                serializedObject.ApplyModifiedProperties();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private string[] GetSceneNames()
    {
        int sceneCount = EditorBuildSettings.scenes.Length;
        string[] names = new string[sceneCount];
        for (int i = 0; i < sceneCount; i++)
        {
            names[i] = System.IO.Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path);
        }
        return names;
    }

    private int GetSelectedSceneIndex(string sceneName)
    {
        for (int i = 0; i < sceneNames.Length; i++)
        {
            if (sceneNames[i] == sceneName)
            {
                return i;
            }
        }
        return -1;
    }
}
