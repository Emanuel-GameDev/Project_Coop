using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{



    //debug
    [SerializeField] bool startImmediatly = false;

    [Header("Set Up")]


    [SerializeField] private List<BoxType> boxTypes = new();


    [SpaceArea(30)]
    [SerializeField] private Dialogue[] dialogues;

    private int dialogueLineIndex = 0;
    private int dialogueIndex = 0;

    private float characterPerSecond = 10;

    Dialogue.DialogueLine nextLine;

    [SerializeField] List<UnityEvent> OnDialogueEnd;
    [SerializeField] public event Action OnDialogueEnded;

    private AudioSource audioSource;

    Coroutine typeCoroutine;

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
        [SerializeField] public Image dialogueChecker;
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

            //controllare
            if(OnDialogueEnd.Count > 0) 
            OnDialogueEnd[dialogueIndex]?.Invoke();

            dialogueIndex++;

            if (dialogueIndex == dialogues.Length)
            {
                dialogueIndex--;
            }


            foreach(PlayerInputHandler handler in GameManager.Instance.coopManager.GetComponentsInChildren<PlayerInputHandler>())
            {
                handler.GetComponent<PlayerInput>().actions.Enable();
                InputAction action = handler.GetComponent<PlayerInput>().actions.FindActionMap("Player").FindAction("Dialogue");
                //InputAction action = handler.GetComponent<PlayerInput>().actions.FindAction("Dialogue");
                action.Disable();
                action.performed -= NextLineInput;
            }

            //if(OnDialogueEnd.Count>0)
            OnDialogueEnded?.Invoke();

            //foreach (PlayerCharacter character in GameManager.Instance.coopManager.activePlayers)
            //{
            //    character.GetComponent<PlayerInput>().actions.FindAction("Dialogue").Disable();
            //    character.GetComponent<PlayerInput>().actions.FindAction("Dialogue").started -= NextLineInput;
            //}

            //foreach (PlayerCharacter character in GameManager.Instance.coopManager.ActivePlayers)
            //{
            //    character.GetComponent<PlayerInput>().actions.FindAction("Dialogue").Disable();
            //}


            gameObject.SetActive(false);
        }

    }
    BoxType nextBox;
    private void SetUpNextLine()
    {




        nextLine = dialogues[dialogueIndex].GetLine(dialogueLineIndex);

        if (nextBox != null)
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
        {
            nextBox.dialogueFrame.color = nextLine.Character.CharacterColor;

        }

        if (nextBox.dialogueChecker != null)
        {
            nextBox.dialogueChecker.color = nextLine.Character.CharacterColor;
            nextBox.dialogueChecker.enabled = false;
        }


        nextBox.contentText.text = string.Empty;

        //Animator boxAnimator = nextBox.box.GetComponent<Animator>();
        //Animator characterImageAnimator = nextBox.characterImage.gameObject.GetComponent<Animator>();
        //if (boxAnimator != null)
        //{
        //    boxAnimator.SetTrigger("NextLine");
        //    boxAnimator.ResetTrigger("NextLine");

            
        //    if(characterImageAnimator != null)
        //    {
        //        if (dialogueLineIndex > 0)
        //        {
               
        //            if (nextLine.Character != dialogues[dialogueIndex].GetLine(dialogueLineIndex-1).Character)
        //            {
        //                //characterImageAnimator.SetTrigger("CharacterChanged");
        //                //characterImageAnimator.ResetTrigger("CharacterChanged");
        //            }

        //        }

        //    }

        //}


    }

    List<InputAction> input;

    public void StartDialogue()
    {

        foreach (PlayerInputHandler handler in GameManager.Instance.coopManager.GetComponentsInChildren<PlayerInputHandler>())
        {
            handler.GetComponent<PlayerInput>().actions.Disable();

            InputAction action = handler.GetComponent<PlayerInput>().actions.FindAction("Dialogue");

            action.Enable();
            action.performed += NextLineInput; 
        }

        //foreach (PlayerCharacter character in GameManager.Instance.coopManager.ActivePlayers)
        //{
        //    character.GetComponent<PlayerInput>().actions.FindAction("Dialogue").Enable();
        //    Debug.Log("Enable");
        //}


        gameObject.SetActive(true);
        dialogueLineIndex = 0;
        SetUpNextLine();
        typeCoroutine = StartCoroutine(TypeLine());
    }


    IEnumerator TypeLine()
    {
        if (audioSource.clip != null)
            audioSource.Play();

        foreach (char c in nextLine.Content.GetLocalizedString().ToCharArray())
        {
            nextBox.contentText.text += c;

            if (!char.IsWhiteSpace(c))
                yield return new WaitForSecondsRealtime(1 / characterPerSecond);
                

        }

        if(nextBox.dialogueChecker != null)
            nextBox.dialogueChecker.enabled = true;
    }
    bool registered = false;


    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();

    }

    float timer;

    bool oneTime = false;
    private void Update()
    {
        if (startImmediatly && !oneTime)
        {
            oneTime = true;
            StartDialogue();
        }

        if (timer < 0.1)
        {
            //guardare se da problemi
            timer += Time.deltaTime;
        }

    }
   

    private void NextLineInput(InputAction.CallbackContext obj)
    {
        if (timer < 0.1)
            return;

        timer = 0;

        if (nextBox.contentText.text == dialogues[dialogueIndex].GetLine(dialogueLineIndex).Content.GetLocalizedString())
            NextLine();
        else
        {
            StopCoroutine(typeCoroutine);
            nextBox.contentText.text = dialogues[dialogueIndex].GetLine(dialogueLineIndex).Content.GetLocalizedString();

            if (nextBox.dialogueChecker != null)
            {
                nextBox.dialogueChecker.enabled = true;
            }
        }

    }

    public void SetDialogue(Dialogue newDialogues)
    {
        dialogues = new Dialogue[1];
        dialogues[0] = newDialogues;
    }

    public void RemoveAllDialogueEnd()
    {
        OnDialogueEnd.Clear();
    }
    public void AddDialogueEnd(UnityEvent unityEvent)
    {
        OnDialogueEnd.Add(unityEvent);
    }
}
