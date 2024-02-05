using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    [Header("Set Up")]
    

    [SerializeField] private List<BoxType> boxTypes = new();


    [SpaceArea(30)]
    [SerializeField] private Dialogue[] dialogues;

    private int dialogueLineIndex = 0;
    private int dialogueIndex = 0;

    private float characterPerSecond = 10;

    Dialogue.DialogueLine nextLine;

    [SerializeField]  List<UnityEvent> OnDialogueEnd;
    [SerializeField] public event Action OnDialogueEnded;

    private AudioSource audioSource;

    Coroutine  typeCoroutine;

    [Serializable]
    public class BoxType
    {
        [SerializeField] public dialogueType boxType;
        [SerializeField] public Image characterImage;
        [SerializeField] public GameObject box;

        [SerializeField] public TextMeshProUGUI nameText;
        [SerializeField] public TextMeshProUGUI contentText;

        //[SerializeField] public Image speakerFrame;
        [SerializeField] public Image dialogueFrame;
    }

    private void NextLine()
    {
        if (dialogueLineIndex < dialogues[dialogueIndex].Lines.Count - 1)
        {
            dialogueLineIndex++;
            SetUpNextLine();
            typeCoroutine = StartCoroutine(TypeLine());
        }
        else
        {

            //OnDialogueEnd[dialogueIndex]?.Invoke();
            OnDialogueEnded.Invoke();

            dialogueIndex++;

            if (dialogueIndex == dialogues.Length)
            {
                dialogueIndex--;
            }

            gameObject.SetActive(false);
        }

    }
    BoxType nextBox;
    private void SetUpNextLine()
    {
        nextLine = dialogues[dialogueIndex].GetLine(dialogueLineIndex);

        if(nextBox != null)
            nextBox.box.gameObject.SetActive(false);

        nextBox = boxTypes.Find(t => t.boxType == nextLine.dialogueType);

        nextBox.box.gameObject.SetActive(true);

        nextBox.nameText.text = nextLine.Character.Name;


        if (nextLine.overrideNameColor)
            nextBox.nameText.color = nextLine.NameColor;
        else
            nextBox.nameText.color = nextLine.Character.CharacterNameColor;


        if (nextLine.overrideNameFont)
            nextBox.nameText.font = nextLine.NameFont;
        else if (nextLine.Character.CharacterNameFont != null)
            nextBox.nameText.font = nextLine.Character.CharacterNameFont;


        if (nextLine.overrideDialogueColor)
            nextBox.contentText.color = nextLine.DialogueColor;
        else
            nextBox.contentText.color = nextLine.Character.CharacterDialogueColor;


        if (nextLine.overrideDialogueFont)
            nextBox.contentText.font = nextLine.DialogueFont;
        else if (nextLine.Character.CharacterDialogueFont != null)
            nextBox.contentText.font = nextLine.Character.CharacterDialogueFont;

        if (nextLine.overrideDialogueSpeed)
            characterPerSecond = nextLine.CharacterPerSecond;
        else
            characterPerSecond = nextLine.Character.DialogueCharacterPerSecond;

        if (nextLine.overrideDialogueVoice)
            audioSource.clip = nextLine.DialogueLineVoice;
        else
            audioSource.clip = nextLine.Character.CharacterVoice;

        nextBox.characterImage.sprite = nextLine.Character.CharacterImage;

        //if (nextBox.speakerFrame != null)
        //    nextBox.speakerFrame.color = nextLine.Character.CharacterColor;

        if (nextBox.dialogueFrame != null)
            nextBox.dialogueFrame.color = nextLine.Character.CharacterColor;





        nextBox.contentText.text = string.Empty;

    }

    public void StartDialogue() 
    {
        gameObject.SetActive(true);
        dialogueLineIndex = 0;
        SetUpNextLine();
        typeCoroutine = StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        if (audioSource.clip != null)
            audioSource.Play();

        foreach(char c in nextLine.Content.ToCharArray())
        {
            nextBox.contentText.text += c;
            yield return new WaitForSeconds(1/characterPerSecond);
        }
    }

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        
        //prova
        //StartDialogue();
    }


    //input di prova
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if(nextBox.contentText.text == dialogues[dialogueIndex].GetLine(dialogueLineIndex).Content)
                NextLine();
            else
            {
                StopCoroutine(typeCoroutine);
                nextBox.contentText.text = dialogues[dialogueIndex].GetLine(dialogueLineIndex).Content;
            }
        }
    }

    public void SetDialogue(Dialogue newDialogues)
    {
        dialogues = new Dialogue[1];
        dialogues[0] = newDialogues;
    }
}
