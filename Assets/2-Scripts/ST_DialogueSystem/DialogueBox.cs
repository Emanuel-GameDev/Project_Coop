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
    [SerializeField] Slider skipSlider;
    [SerializeField] Animation characterAnimation;
    [SerializeField] Animation boxAnimation;
    [SerializeField] Animation boxBackAnimation;

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
            EndDialogue();
        }

    }

    private void EndDialogue()
    {
        //foreach (Transform t in HPHandler.Instance.HpContainerTransform)
        //{
        //    foreach (Image i in t.gameObject.GetComponentsInChildren<Image>())
        //    {
        //        i.color = new Color(1, 1, 1, 1);
        //    }


        //    foreach(TextMeshProUGUI text in t.gameObject.GetComponentsInChildren<TextMeshProUGUI>())
        //    {
        //        text.color = new Color(1, 1, 1, 1);
        //    }

        //}

        HPHandler.Instance.SetHudVisible(true);


        foreach (PlayerInputHandler handler in GameManager.Instance.CoopManager.GetComponentsInChildren<PlayerInputHandler>())
        {
            handler.GetComponent<PlayerInput>().actions.FindActionMap("Player").Enable();
            InputAction action = handler.GetComponent<PlayerInput>().actions.FindActionMap("Player").FindAction("Dialogue");
            //InputAction action = handler.GetComponent<PlayerInput>().actions.FindAction("Dialogue");
            action.Disable();
            action.performed -= NextLineInput;
            action.canceled -= NextLineInputCancelled;
        }

        //controllare
        if (OnDialogueEnd.Count > 0)
            OnDialogueEnd[dialogueIndex]?.Invoke();

        dialogueIndex++;

        if (dialogueIndex == dialogues.Length)
        {
            dialogueIndex--;
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

        //skipDictionary.Clear();
        gameObject.SetActive(false);
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



        Dialogue.DialogueLine previousLine = new();

        if (dialogueLineIndex > 0)
            previousLine = dialogues[dialogueIndex].GetLine(dialogueLineIndex - 1);
        
        if (nextLine.Character != previousLine.Character)
        {
            characterAnimation.Play("DialogueBoxCharacterEntrance");
            boxAnimation.Play("DialogueBoxEntrance");
            boxBackAnimation.Play("DialogueBoxBackIdle");
        }
        else
        {
            if(!boxBackAnimation.isPlaying)
                boxBackAnimation.Play("DialogueBoxBackIdle");
        }

    }
    //Dictionary<InputAction, bool> skipDictionary;

    public void StartDialogue()
    {
        //if(skipDictionary== null)
        //{
        //    skipDictionary = new Dictionary<InputAction, bool>();
        ////}

        //skipDictionary.Clear();
        skipSlider.gameObject.SetActive(false);

        //foreach(Transform t in HPHandler.Instance.HpContainerTransform)
        //{
        //    foreach(Image i in t.gameObject.GetComponentsInChildren<Image>())
        //    {
        //        i.color = new Color(1, 1, 1, 0);
        //    }

        //    foreach (TextMeshProUGUI text in t.gameObject.GetComponentsInChildren<TextMeshProUGUI>())
        //    {
        //        text.color = new Color(1, 1, 1, 0);
        //    }
        //}

        HPHandler.Instance.SetHudVisible(false);
        


        foreach (PlayerInputHandler handler in GameManager.Instance.CoopManager.GetComponentsInChildren<PlayerInputHandler>())
        {
            handler.GetComponent<PlayerInput>().actions.FindActionMap("Player").Disable();
            // handler.GetComponent<PlayerInput>().actions.FindActionMap("Player").FindAction("Menu").Enable();
            // handler.GetComponent<PlayerInput>().actions.FindActionMap("Player").FindAction("Option").Enable();
            InputAction action = handler.GetComponent<PlayerInput>().actions.FindActionMap("Player").FindAction("Dialogue");

            action.Enable();
            action.performed += NextLineInput;
            action.canceled += NextLineInputCancelled;

            //skipDictionary.Add(action, false);
        }


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
            StartCoroutine(Utility.WaitForPlayers(StartDialogue));
            //StartDialogue();
        }

        if (timer < 0.1)
        {
            //guardare se da problemi
            timer += Time.deltaTime;
        }

        if (timerActive)
        {
            skipSlider.value +=  Time.deltaTime / timeToPressToSkip;

            if (skipSlider.value > 0.1f)
                skipSlider.gameObject.SetActive(true);
        }

    }

    private void NextLineInputCancelled(InputAction.CallbackContext context)
    {
        //skipDictionary[context.action] = false;
        StopSkip();
    }

    bool startSkip = false;
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


        //skipDictionary[obj.action] = true;
        //startSkip=true;

        //foreach (bool b in skipDictionary.Values)
        //{
        //    if (!b)
                //startSkip = false;
        //}

        //if(startSkip)
        //{
           StartSkip();
        //}
    }
    Coroutine skipCoroutine;
    private void StartSkip()
    {
        if(gameObject.activeSelf)
        {
            skipCoroutine = StartCoroutine(SkipCoroutine());
            timerActive = true;
        }
    }

    private void StopSkip()
    {
        if(skipCoroutine != null)
        {
            StopCoroutine(skipCoroutine);
            skipSlider.gameObject.SetActive(false);
            timerActive = false;
            skipSlider.value = 0;
        }
    }

    public float timeToPressToSkip = 3;
    bool timerActive=false;
    IEnumerator SkipCoroutine()
    {
        yield return new WaitForSecondsRealtime(timeToPressToSkip);
        skipSlider.gameObject.SetActive(false);
        EndDialogue();
    }

    public void SetDialogue(Dialogue newDialogues)
    {
        dialogues = new Dialogue[1];
        dialogues[0] = newDialogues;
        dialogueIndex = 0;
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
