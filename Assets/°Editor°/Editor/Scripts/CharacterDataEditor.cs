using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterData))]
public class CharacterDataEditor : Editor
{
    private SerializedProperty characterClass;

    private void OnEnable()
    {
        characterClass = serializedObject.FindProperty("characterClass");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();

        EditorGUILayout.Space();

        // Display properties of the CharacterClass
        if (characterClass.objectReferenceValue != null)
        {
            string className = characterClass.objectReferenceValue.GetType().Name;
            EditorGUILayout.LabelField($"{className} Properties", EditorStyles.boldLabel);
            DrawCharacterClassProperties();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawCharacterClassProperties()
    {
        SerializedObject charClassSerializedObject = new SerializedObject(characterClass.objectReferenceValue);
        charClassSerializedObject.Update();

        SerializedProperty iterator = charClassSerializedObject.GetIterator();
        bool enterChildren = true;
        while (iterator.NextVisible(enterChildren))
        {
            if (iterator.name != "m_Script")
            {
                EditorGUILayout.PropertyField(iterator, true);
            }
            enterChildren = false;
        }

        charClassSerializedObject.ApplyModifiedProperties();
    }

}
