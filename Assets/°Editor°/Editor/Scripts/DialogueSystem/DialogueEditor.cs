using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEditor.VersionControl;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEditor.Localization;
using UnityEngine.Localization;

public class DialogueEditor : EditorWindow
{
    [MenuItem("Tools/Dialogue Editor")]
    private static void OpenWindow()
    {
        GetWindow<DialogueEditor>();
    }

    private int _selectedName;
    private int _selectedElement;
    private Vector2 _scrollView;

    private int newLineIndex;
    private string _newDialogueName;

    bool refreshDialogues = true;

    string[] dialoguesIds;
    string[] dialogues;
    string[] dialogueNames;

    string lastSearched;
    string searchedNames;
    List<string> namesFound=new List<string>();
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
        refreshDialogues = true;
    }

    private void OnGUI()
    {
        if (refreshDialogues)
        {
            dialoguesIds = AssetDatabase.FindAssets("t:Dialogue");
            dialogues = new string[dialoguesIds.Length];
            dialogueNames = new string[dialoguesIds.Length];


            for (int i = 0; i < dialoguesIds.Length; i++)
            {
                dialogues[i] = AssetDatabase.GUIDToAssetPath(dialoguesIds[i]);

                dialogueNames[i] = Path.GetFileNameWithoutExtension(dialogues[i]);
            }

            refreshDialogues = false;
        }


        EditorGUILayout.BeginHorizontal();

        _newDialogueName=EditorGUILayout.TextField(_newDialogueName);
        if (GUILayout.Button("Add new dialogue",GUILayout.MaxWidth(200)))
        {
            CreateNewDialogue();
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
        

        if (GUILayout.Button("Refresh"))
        {
            refreshDialogues=true;
            tableCollection = LocalizationEditorSettings.GetStringTableCollection("Dialogue");
            table = tableCollection.StringTables[1];


        }

        GUILayout.Space(10);

        if (dialogues == null)
            return;

        if (dialogues.Length <= 0)
            return;

        if (_selectedElement >= dialoguesIds.Length)
            _selectedElement = dialoguesIds.Length - 1;


        Dialogue selectedDialogue = AssetDatabase.LoadAssetAtPath<Dialogue>(dialogues[_selectedElement]);

        if (selectedDialogue == null)
            return;



        //searchedNames = EditorGUILayout.TextField("Search",searchedNames);

        //if (lastSearched != searchedNames)
        //{
        //    namesFound.Clear();

        //    if (!string.IsNullOrEmpty(searchedNames))
        //    {
        //        foreach(string dialogue in dialogueNames)
        //        {
        //            if (dialogue.Contains(searchedNames)) 
        //                namesFound.Add(dialogue);
        //        }
        //    }
        //    else
        //    {
        //        foreach (string dialogue in dialogueNames)
        //        {
        //            namesFound.Add(dialogue);
        //        }
        //    }
        //    lastSearched = searchedNames;
        //}



        _selectedElement = EditorGUILayout.Popup(_selectedElement, dialogueNames);
       // _selectedName = EditorGUILayout.Popup(_selectedElement, namesFound.ToArray());

        //if(_selectedName<0 || _selectedName > namesFound.Count)
        //      _selectedName=0;

        //      for (int i = 0; i <= dialogueNames.Length; i++)
        //      {
        //          if (dialogueNames[i] == namesFound[_selectedName])
        //          {
        //              _selectedElement = i;
        //              break;
        //          }
        //      }


        GUILayout.Space(20);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("New line above"))
        {
            selectedDialogue.AddLine(0);

            table.AddEntry($"{selectedDialogue.name}LineID:{selectedDialogue.Lines.Count-1}", "");


            for (int i = selectedDialogue.Lines.Count - 1; i >= 0; i--)
            {
                if (i > 0)
                    table.GetEntry($"{selectedDialogue.name}LineID:{i}").Value = table.GetEntry($"{selectedDialogue.name}LineID:{i - 1}").Value;
                else
                    table.GetEntry($"{selectedDialogue.name}LineID:{i}").Value = "";

                selectedDialogue.Lines[i].Content.SetReference("Dialogue", $"{selectedDialogue.name}LineID:{i}");
                
            }


            EditorUtility.SetDirty(selectedDialogue);
        }

        if (GUILayout.Button("New line below"))
        {

            selectedDialogue.AddLine(selectedDialogue.Lines.Count);
            table.AddEntry($"{selectedDialogue.name}LineID:{selectedDialogue.Lines.Count - 1}", "");
           
            //if (table.GetEntry($"{selectedDialogue.name}LineID:{selectedDialogue.Lines.Count - 1}") != null)
            //    Debug.Log($"{selectedDialogue.name}LineID:{selectedDialogue.Lines.Count - 1}");
            //else
            //    Debug.Log("null");
            

            selectedDialogue.Lines[selectedDialogue.Lines.Count - 1].Content.SetReference("Dialogue", $"{selectedDialogue.name}LineID:{selectedDialogue.Lines.Count - 1}");


            EditorUtility.SetDirty(selectedDialogue);
        }

        GUILayout.Space(20);

        if (GUILayout.Button("New line at:"))
        {
            selectedDialogue.AddLine(newLineIndex-1);


            table.AddEntry($"{selectedDialogue.name}LineID:{selectedDialogue.Lines.Count - 1}", "");


            for (int i = selectedDialogue.Lines.Count - 1; i >= newLineIndex-1; i--)
            {
                if (i > newLineIndex-1)
                    table.GetEntry($"{selectedDialogue.name}LineID:{i}").Value = table.GetEntry($"{selectedDialogue.name}LineID:{i - 1}").Value;
                else
                    table.GetEntry($"{selectedDialogue.name}LineID:{i}").Value = "";

                selectedDialogue.Lines[i].Content.SetReference("Dialogue", $"{selectedDialogue.name}LineID:{i}");

            }

        }

        newLineIndex =  EditorGUILayout.IntField(newLineIndex, GUILayout.MaxWidth(50));
        newLineIndex = Mathf.Clamp(newLineIndex,1,selectedDialogue.Lines.Count);

        EditorGUILayout.EndHorizontal();

        if(currentLocale == null)
        {
            currentLocale = LocalizationEditorSettings.GetLocale(LocalizationSettings.SelectedLocale.Identifier);
        }

        if(table==null)
        {
            tableCollection = LocalizationEditorSettings.GetStringTableCollection("Dialogue");
            table = tableCollection.StringTables[1];
        }

        for (int i = 0; i < selectedDialogue.Lines.Count; i++)
        {
            if (table.GetEntry($"{selectedDialogue.name}LineID:{i}") == null)
            {
                StringTableEntry entry = table.AddEntry($"{selectedDialogue.name}LineID:{i}", "");

                selectedDialogue.Lines[i].Content.SetReference("Dialogue", $"{selectedDialogue.name}LineID:{i}");
            }

            if(!selectedDialogue.Lines[i].Content.ContainsKey($"{selectedDialogue.name}LineID:{i}"))
            {
                selectedDialogue.Lines[i].Content.SetReference("Dialogue",$"{selectedDialogue.name}LineID:{i}");
            }
        }

        DialoguesLineGUI(selectedDialogue);

    }

    StringTableCollection tableCollection;
    StringTable table;
    Locale currentLocale;

    StringTableEntry entry;

    private void DialoguesLineGUI(Dialogue selectedDialogue)
    {
        
            if (table == null)
                return;
        _scrollView = EditorGUILayout.BeginScrollView(_scrollView);


        for (int i = 0; i < selectedDialogue.Lines.Count; i++) 
        {
            Dialogue.DialogueLine line = selectedDialogue.Lines[i];
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            entry = table.GetEntry($"{selectedDialogue.name}LineID:{i}");


            string characterName = (bool)line.Character ? line.Character.Name : "[null]";
            string content = entry.Value.Length < 50 ? entry.Value : entry.Value.Substring(0, 50);

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

                entry.Value = EditorGUILayout.TextField("Text content", entry.Value);
                //line.Content. = EditorGUILayout.TextField("Text content", line.Content[$"{selectedDialogue.name}LineID:{i}"].ToString());



                GUILayout.Space(10);

                line.overrideOptionsOpenInEditor = EditorGUILayout.Foldout(line.overrideOptionsOpenInEditor, "Custom functions");
                EditorGUIUtility.fieldWidth = 50;
                EditorGUIUtility.labelWidth = 200;

                if (line.overrideOptionsOpenInEditor)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.BeginHorizontal();
                    if (line.overrideNameColor = EditorGUILayout.Toggle("Override name color", line.overrideNameColor, GUILayout.ExpandWidth(true)))
                        line.NameColor = EditorGUILayout.ColorField(line.NameColor);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (line.overrideNameFont = EditorGUILayout.Toggle("Override name font", line.overrideNameFont))
                        line.NameFont = EditorGUILayout.ObjectField(line.NameFont, typeof(TMP_FontAsset), false) as TMP_FontAsset;
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (line.overrideDialogueColor = EditorGUILayout.Toggle("Override dialogue color", line.overrideDialogueColor))
                        line.DialogueColor = EditorGUILayout.ColorField(line.DialogueColor);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (line.overrideDialogueFont = EditorGUILayout.Toggle("Override dialogue font", line.overrideDialogueFont))
                        line.DialogueFont = EditorGUILayout.ObjectField(line.DialogueFont, typeof(TMP_FontAsset), false) as TMP_FontAsset;
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (line.overrideDialogueVoice = EditorGUILayout.Toggle("Override dialogue voice", line.overrideDialogueVoice))
                        line.DialogueLineVoice = EditorGUILayout.ObjectField(line.DialogueLineVoice, typeof(AudioClip), false) as AudioClip;
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (line.overrideDialogueSpeed = EditorGUILayout.Toggle("Override dialogue speed", line.overrideDialogueSpeed))
                        line.CharacterPerSecond = EditorGUILayout.FloatField(line.CharacterPerSecond);
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel--;
                }

                if (line.CharacterPerSecond < 0.1f)
                {
                    line.CharacterPerSecond = 0.1f;
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

                    for(int l = i; l < selectedDialogue.LinesCount(); l++)
                    {

                        table.GetEntry($"{selectedDialogue.name}LineID:{l}").Value = table.GetEntry($"{selectedDialogue.name}LineID:{l + 1}").Value;
                        

                        selectedDialogue.Lines[l].Content.SetReference("Dialogue", $"{selectedDialogue.name}LineID:{l}");

                    }

                    tableCollection.RemoveEntry($"{selectedDialogue.name}LineID:{selectedDialogue.Lines.Count}");

                }

                    EditorGUILayout.EndHorizontal();
            }

            


            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(selectedDialogue);
                EditorUtility.SetDirty(table);
                EditorUtility.SetDirty(table.SharedData);
            }

            EditorGUILayout.EndVertical();

        }



        EditorGUILayout.EndScrollView();
    }
}
