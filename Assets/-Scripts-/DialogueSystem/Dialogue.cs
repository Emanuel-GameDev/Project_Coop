using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName ="Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    [Serializable]
    public class DialogueLine
    {
        [SerializeField] private DialogueCharacter character;
        [SerializeField] private string content = "";

        public bool overrideNameColor;
        [SerializeField] private Color nameColor;

        public bool overrideNameFont;
        [SerializeField] private TMP_FontAsset nameFont;

        public bool overrideDialogueColor;
        [SerializeField] private Color dialogueColor;

        public bool overrideDialogueFont;
        [SerializeField] private TMP_FontAsset dialogueFont;

        [SerializeField] private float characterPerSecond = 20;

#if UNITY_EDITOR
        public bool openInEditor = false;
        public bool overrideOptionsOpenInEditor = false;
#endif

        public DialogueCharacter Character
        {
            get { return character; }
            set { character = value; }
        }

        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        public Color NameColor
        {
            get { return nameColor; }
            set { nameColor = value; }
        }

        public Color DialogueColor
        {
            get { return dialogueColor; }
            set { dialogueColor = value; }
        }

        public TMP_FontAsset NameFont
        {
            get { return nameFont; }
            set { nameFont = value; }
        }

        public TMP_FontAsset DialogueFont
        {
            get { return dialogueFont; }
            set { dialogueFont = value; }
        }

        public float CharacterPerSecond
        {
            get { return characterPerSecond; }
            set { characterPerSecond = value; }
        }
    }

    [SerializeField] private List<DialogueLine> lines=new();

    public List<DialogueLine> Lines => lines;

    public DialogueLine GetLine(int index) => lines[index];

    public void AddLine(int index) 
    { 
        lines.Insert(index, new DialogueLine());
        lines[index].CharacterPerSecond = 20;
    }

    public void SwapLines(int index1,int index2)
    {
        (lines[index1], lines[index2]) = (lines[index2], lines[index1]);
    }
    public void RemoveLine(int index1)=>lines.RemoveAt(index1);
    public int LinesCount() => lines.Count;
    
}
