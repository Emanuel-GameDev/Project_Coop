using System.Collections;
using System.Collections.Generic;
using TMPro;
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

        EditorGUILayout.LabelField("GENERAL INFO", EditorStyles.boldLabel);
        _character.Name = EditorGUILayout.TextField("Character name", _character.Name);
        _character.CharacterImage = (Sprite) EditorGUILayout.ObjectField("Character image", _character.CharacterImage, typeof(Sprite), false);
        _character.CharacterColor = EditorGUILayout.ColorField("Character frame color",_character.CharacterColor);
        _character.CharacterVoice = EditorGUILayout.ObjectField("Character voice audio", _character.CharacterVoice, typeof(AudioClip), false) as AudioClip;

        GUILayout.Space(20);

        EditorGUILayout.LabelField("NAME INFO", EditorStyles.boldLabel);
        _character.CharacterNameColor = EditorGUILayout.ColorField("Character name color", _character.CharacterNameColor);
        _character.CharacterNameFont = EditorGUILayout.ObjectField("Character name font", _character.CharacterNameFont, typeof(TMP_FontAsset), false) as TMP_FontAsset;

        GUILayout.Space(20);

        EditorGUILayout.LabelField("DIALOGUES INFO", EditorStyles.boldLabel);
        _character.CharacterDialogueColor = EditorGUILayout.ColorField("Character dialogue color", _character.CharacterDialogueColor);
        _character.CharacterDialogueFont = EditorGUILayout.ObjectField("Character dialogue font", _character.CharacterDialogueFont, typeof(TMP_FontAsset), false) as TMP_FontAsset;
        _character.DialogueCharacterPerSecond = EditorGUILayout.FloatField("Dialogue speed in characters per second", _character.DialogueCharacterPerSecond);

        if (_character.DialogueCharacterPerSecond < 0.1f)
            _character.DialogueCharacterPerSecond = 0.1f;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create"))
        {
            DialogueCharacter asset = ScriptableObject.CreateInstance<DialogueCharacter>();
            AssetDatabase.CreateAsset(asset, $"Assets/_Prefabs_/DialogueSystem/Data/Characters/{_character.Name}.asset");


            asset.Name = _character.Name;
            asset.CharacterImage = _character.CharacterImage;
            asset.CharacterColor = _character.CharacterColor;

            asset.CharacterNameColor = _character.CharacterNameColor;
            asset.CharacterNameFont = _character.CharacterNameFont;

            asset.CharacterDialogueColor = _character.CharacterDialogueColor;
            asset.CharacterDialogueFont = _character.CharacterDialogueFont;

            asset.CharacterVoice = _character.CharacterVoice;
            asset.DialogueCharacterPerSecond = _character.DialogueCharacterPerSecond;

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
