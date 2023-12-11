using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEditor.VersionControl;
using UnityEngine;
using TMPro;

public class DialogueEditor : EditorWindow
{
    [MenuItem("Tools/Dialogue Editor")]
    private static void OpenWindow()
    {
        GetWindow<DialogueEditor>();
    }

    private int _selectedElement;
    private Vector2 _scrollView;

    private int newLineIndex;
    private string _newDialogueName;


    private void CreateNewDialogue()
    {
        if (string.IsNullOrEmpty(_newDialogueName))
        {
            EditorUtility.DisplayDialog("Error", "Invalid name", "Ok");
            return;
        }

        Dialogue asset = ScriptableObject.CreateInstance<Dialogue>();
        AssetDatabase.CreateAsset(asset, $"Assets/_Prefabs_/DialogueSystem/Data/Dialogues/{_newDialogueName}.asset");
        AssetDatabase.SaveAssets();
    }

    private void OnGUI()
    {

        string[] dialoguesIds = AssetDatabase.FindAssets("t:Dialogue");
        string[] dialogues = new string[dialoguesIds.Length];
        string[] dialogueNames = new string[dialoguesIds.Length];


        for (int i = 0; i < dialoguesIds.Length; i++)
        {
            dialogues[i] = AssetDatabase.GUIDToAssetPath(dialoguesIds[i]);

            dialogueNames[i] = Path.GetFileNameWithoutExtension(dialogues[i]);
        }


        EditorGUILayout.BeginHorizontal();

        _newDialogueName=EditorGUILayout.TextField(_newDialogueName);
        if (GUILayout.Button("Add new dialogue",GUILayout.MaxWidth(200)))
        {
            CreateNewDialogue();
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);


        if (dialogues.Length <= 0)
            return;

        if (_selectedElement >= dialoguesIds.Length)
            _selectedElement = dialoguesIds.Length - 1;


        Dialogue selectedDialogue = AssetDatabase.LoadAssetAtPath<Dialogue>(dialogues[_selectedElement]);

        if (selectedDialogue == null)
            return;

        _selectedElement = EditorGUILayout.Popup(_selectedElement, dialogueNames);



        GUILayout.Space(20);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("New line above"))
        {
            selectedDialogue.AddLine(0);
            EditorUtility.SetDirty(selectedDialogue);
        }

        if (GUILayout.Button("New line below"))
        {
            selectedDialogue.AddLine(selectedDialogue.Lines.Count);
            EditorUtility.SetDirty(selectedDialogue);
        }

        GUILayout.Space(20);

        if (GUILayout.Button("New line at:"))
        {
            selectedDialogue.AddLine(newLineIndex-1);
        }
        newLineIndex =  EditorGUILayout.IntField(newLineIndex, GUILayout.MaxWidth(50));
        newLineIndex = Mathf.Clamp(newLineIndex,1,selectedDialogue.Lines.Count);

        EditorGUILayout.EndHorizontal();

        DialoguesLineGUI(selectedDialogue);

    }

    private void DialoguesLineGUI(Dialogue selectedDialogue)
    {
        
        _scrollView = EditorGUILayout.BeginScrollView(_scrollView);


        for (int i = 0; i < selectedDialogue.Lines.Count; i++) 
        {
            Dialogue.DialogueLine line = selectedDialogue.Lines[i];
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            string characterName = (bool)line.Character ? line.Character.Name : "[null]";
            string content = line.Content.Length < 50 ? line.Content : line.Content.Substring(0, 50);

            line.openInEditor = EditorGUILayout.Foldout(line.openInEditor, $"Line: {i + 1} ({characterName}: {content})");

            if (line.openInEditor)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.BeginHorizontal();

                line.Character = EditorGUILayout.ObjectField("Character",
                    line.Character,
                    typeof(DialogueCharacter),
                    false) as DialogueCharacter;

                if (GUILayout.Button("+", GUILayout.MaxWidth(20)))
                {

                    DialogueCharacterEditor characterEditor = ScriptableObject.CreateInstance<DialogueCharacterEditor>();
                    characterEditor.SetDialogueLine(selectedDialogue, line);
                    characterEditor.ShowUtility();
                }

                    EditorGUILayout.EndHorizontal();

                line.Content = EditorGUILayout.TextField("Text content", line.Content);



                line.CharacterPerSecond = EditorGUILayout.FloatField("Text speed",line.CharacterPerSecond);

                if(line.CharacterPerSecond < 0.1f)
                {
                    line.CharacterPerSecond = 0.1f;
                }


                GUILayout.Space(10);

                line.overrideOptionsOpenInEditor = EditorGUILayout.Foldout(line.overrideOptionsOpenInEditor, "Custom functions");

                if(line.overrideOptionsOpenInEditor)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.BeginHorizontal();
                    if (line.overrideNameColor = EditorGUILayout.Toggle("Override name color", line.overrideNameColor, GUILayout.Width(200)))
                        line.NameColor = EditorGUILayout.ColorField("Name color", line.NameColor);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (line.overrideNameFont = EditorGUILayout.Toggle("Override name font", line.overrideNameFont, GUILayout.Width(200)))
                        line.NameFont = EditorGUILayout.ObjectField("Name font", line.NameFont, typeof(TMP_FontAsset), false) as TMP_FontAsset;
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (line.overrideDialogueColor = EditorGUILayout.Toggle("Override dialogue color", line.overrideDialogueColor, GUILayout.Width(200)))
                        line.DialogueColor = EditorGUILayout.ColorField("Dialogue color", line.DialogueColor);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (line.overrideDialogueFont = EditorGUILayout.Toggle("Override dialogue font", line.overrideDialogueFont, GUILayout.Width(200)))
                        line.DialogueFont = EditorGUILayout.ObjectField("Dialogue font", line.DialogueFont, typeof(TMP_FontAsset), false) as TMP_FontAsset;
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel--;
                }




                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();


                if(GUILayout.Button("Move above",EditorStyles.miniButtonLeft) && i != 0)
                {
                    selectedDialogue.SwapLines(i, i - 1);
                }

                if (GUILayout.Button("Move Below", EditorStyles.miniButtonMid) && i != selectedDialogue.LinesCount() - 1)
                {
                    selectedDialogue.SwapLines(i, i + 1);
                }

                if (GUILayout.Button("Remove",EditorStyles.miniButtonRight) && EditorUtility.DisplayDialog("Warning","Do you want to remove this line?","Yes","No"))
                {
                    selectedDialogue.RemoveLine(i);
                }

                    EditorGUILayout.EndHorizontal();
            }

            


            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(selectedDialogue);
            }

            EditorGUILayout.EndVertical();

        }



        EditorGUILayout.EndScrollView();
    }
}
