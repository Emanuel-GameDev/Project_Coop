using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    [Header("Set Up")]
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI contentText;

    [SerializeField] private Image speakerFrame;
    [SerializeField] private Image dialogueFrame;

    private AudioSource audioSource;
    

    [SpaceArea(30)]
    [SerializeField] private Dialogue[] dialogues;

    private int dialogueLineIndex = 0;
    private int dialogueIndex = 0;

    private float characterPerSecond = 10;

    Dialogue.DialogueLine nextLine;

    private void NextLine()
    {
        if (dialogueLineIndex < dialogues[dialogueIndex].Lines.Count - 1)
        {
            dialogueLineIndex++;
            SetUpNextLine();
            StartCoroutine(TypeLine());
        }
        else
        {
            dialogueIndex++;

            if (dialogueIndex == dialogues.Length)
            {
                dialogueIndex--;
            }

            gameObject.SetActive(false);
        }

    }

    private void SetUpNextLine()
    {
        nextLine = dialogues[dialogueIndex].GetLine(dialogueLineIndex);

        nameText.text = nextLine.Character.Name;


        if (nextLine.overrideNameColor)
            nameText.color = nextLine.NameColor;
        else
            nameText.color = nextLine.Character.CharacterNameColor;


        if (nextLine.overrideNameFont)
            nameText.font = nextLine.NameFont;
        else if (nextLine.Character.CharacterNameFont != null)
            nameText.font = nextLine.Character.CharacterNameFont;


        if (nextLine.overrideDialogueColor)
            contentText.color = nextLine.DialogueColor;
        else
            contentText.color = nextLine.Character.CharacterDialogueColor;


        if (nextLine.overrideDialogueFont)
            contentText.font = nextLine.DialogueFont;
        else if (nextLine.Character.CharacterDialogueFont != null)
            contentText.font = nextLine.Character.CharacterDialogueFont;

        if (nextLine.overrideDialogueSpeed)
            characterPerSecond = nextLine.CharacterPerSecond;
        else
            characterPerSecond = nextLine.Character.DialogueCharacterPerSecond;

        if (nextLine.overrideDialogueVoice)
            audioSource.clip = nextLine.DialogueLineVoice;
        else
            audioSource.clip = nextLine.Character.CharacterVoice;

        characterImage.sprite = nextLine.Character.CharacterImage;


        speakerFrame.color = nextLine.Character.CharacterColor;
        dialogueFrame.color = nextLine.Character.CharacterColor;

        characterPerSecond = nextLine.CharacterPerSecond;



        contentText.text = string.Empty;

    }

    private void StartDialogue() 
    {
        dialogueLineIndex = 0;
        SetUpNextLine();
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        if (audioSource.clip != null)
            audioSource.Play();

        foreach(char c in nextLine.Content.ToCharArray())
        {
            contentText.text += c;
            yield return new WaitForSeconds(1/characterPerSecond);
        }
    }

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();

        //prova
        StartDialogue();
    }


    //input di prova
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(contentText.text == dialogues[dialogueIndex].GetLine(dialogueLineIndex).Content)
                NextLine();
            else
            {
                StopAllCoroutines();
                contentText.text = dialogues[dialogueIndex].GetLine(dialogueLineIndex).Content;
            }
        }
    }
}
