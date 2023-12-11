using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueCharacterEditor : EditorWindow
{
    private DialogueCharacter _character;
    private Dialogue.DialogueLine _dialogueLine;
    private Dialogue _dialogue;

    public void SetDialogueLine(Dialogue dialogue, Dialogue.DialogueLine line)
    {
        _dialogue=dialogue;
        _dialogueLine = line;
    }


    private void OnGUI()
    {
        if (_character == null)
        {
            _character = ScriptableObject.CreateInstance<DialogueCharacter>();
        }

        _character.Name = EditorGUILayout.TextField("Character name", _character.Name);
        _character.CharacterImage = (Sprite) EditorGUILayout.ObjectField("Character image", _character.CharacterImage, typeof(Sprite), false);
        _character.CharacterColor = EditorGUILayout.ColorField("Character frame color",_character.CharacterColor);
        _character.CharacterNameColor = EditorGUILayout.ColorField("Character name color", _character.CharacterNameColor);
        _character.CharacterDialogueColor = EditorGUILayout.ColorField("Character dialogue color", _character.CharacterDialogueColor);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create"))
        {
            DialogueCharacter asset = ScriptableObject.CreateInstance<DialogueCharacter>();
            AssetDatabase.CreateAsset(asset, $"Assets/_Prefabs_/DialogueSystem/Data/Characters/{_character.Name}.asset");


            asset.Name = _character.Name;
            asset.CharacterImage = _character.CharacterImage;
            asset.CharacterColor = _character.CharacterColor;
            asset.CharacterNameColor = _character.CharacterNameColor;
            asset.CharacterDialogueColor = _character.CharacterDialogueColor;

            _dialogueLine.Character = _character;


            AssetDatabase.SaveAssets();

            EditorUtility.SetDirty(asset);
            Close();
        }

        if (GUILayout.Button("Cancel"))
        {
            Close();
        }
        EditorGUILayout.EndHorizontal();
    }

}
