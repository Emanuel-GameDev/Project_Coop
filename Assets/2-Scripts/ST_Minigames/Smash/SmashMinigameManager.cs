using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SmashMinigameManager : MonoBehaviour
{
    private static SmashMinigameManager _instance;
    public static SmashMinigameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SmashMinigameManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("SmashMinigameManager");
                    _instance = singletonObject.AddComponent<SmashMinigameManager>();
                }
            }

            return _instance;
        }
    }

    internal bool canCount = false;
    internal List<SmashPlayer> listOfCurrentPlayer;

    [Header("Settings")]
    [SerializeField] int timeToSmash = 5;

    [Header("Dialogue Settings")]
    [SerializeField]
    private GameObject dialogueBox;
    [SerializeField]
    private Dialogue winDialogue;
    [SerializeField]
    private Dialogue loseDialogue;
    [SerializeField]
    private UnityEvent onWinDialogueEnd;
    [SerializeField]
    private UnityEvent onLoseDialogueEnd;

    private DialogueBox _dialogueBox;


    private void Awake()
    {
        listOfCurrentPlayer = new List<SmashPlayer>();

        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        if (dialogueBox != null)
        {
            _dialogueBox = dialogueBox.GetComponent<DialogueBox>();
        }
        else
            Debug.LogError("DialogueBox is null");

        //ChangeGameVolume(true);
    }

    private void Start()
    {
        //StartCoroutine(WaitForPlayers());

        StartPlay();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartPlay();
        }
    }


    private void StartPlay()
    {
        foreach (SmashPlayer player in listOfCurrentPlayer)
        {
            player.ResetCounter();
            player.canCount = true;
            Debug.Log("Start");
        }

        StartCoroutine(TimerMinigame(timeToSmash));
    }
    private void StopPlay()
    {
        foreach (SmashPlayer player in listOfCurrentPlayer)
        {
            player.canCount = false;
            Debug.Log(player.smashCount);
        }

    }

    IEnumerator TimerMinigame(int seconds)
    {
        int counter = seconds;
        while (counter > 0)
        {
            yield return new WaitForSeconds(1);
            counter--;
        }

        StopPlay();
        // DoStuff();
    }


    IEnumerator WaitForPlayers()
    {
        yield return new WaitUntil(() => CoopManager.Instance.GetActiveHandlers() != null && CoopManager.Instance.GetActiveHandlers().Count > 0);
        dialogueBox.SetActive(true);
        _dialogueBox.StartDialogue();
    }


}
