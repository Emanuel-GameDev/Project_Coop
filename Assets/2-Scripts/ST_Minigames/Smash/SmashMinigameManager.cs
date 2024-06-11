using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] int startCountdown = 3;
    [SerializeField] int timeToSmash = 5;
    [SerializeField] float startingSpeedForResults = 1;
    [SerializeField] float timeForResults = 5;
    [SerializeField] float speedFalloff = 1;

    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI startCountdownText;


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

        //StartPlay();
        startCountdownText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(StartCountdown(startCountdown));
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
        StartCoroutine(EndGameCount());
    }

    IEnumerator EndGameCount()
    {
        foreach (SmashPlayer player in listOfCurrentPlayer)
        {
            player.canCount = false;

            player.countText.gameObject.SetActive(true);
            player.countText.text = "0";
        }

        int currentNumber = 0;

        listOfCurrentPlayer.Sort();

        float speed = 0;
        int biggest = listOfCurrentPlayer[listOfCurrentPlayer.Count - 1].smashCount;
        float incrementPerSecond = biggest / timeForResults;

        float speedDecrement = timeForResults / incrementPerSecond;

        speed = startingSpeedForResults;

        while (listOfCurrentPlayer.TrueForAll(p=>p.smashCount>=currentNumber))
        {
            yield return new WaitForSeconds((1/incrementPerSecond) * /*Time.deltaTime * */speed);

            currentNumber++;
            speedDecrement = timeForResults / speed;
            speed = speedDecrement;

            foreach (SmashPlayer player in listOfCurrentPlayer)
            {
                if (player.smashCount >= currentNumber)
                {
                    player.countText.gameObject.SetActive(true);
                    player.countText.text = currentNumber.ToString();

                }
            }

        }

        Debug.Log("Fine");

    }

    IEnumerator StartCountdown(int seconds)
    {
        int counter = seconds;
        startCountdownText.gameObject.SetActive(true);
        startCountdownText.text = counter.ToString();

        while (counter > 0)
        {
            yield return new WaitForSeconds(1);
            counter--;
            startCountdownText.text = counter.ToString();
        }

        startCountdownText.gameObject.SetActive(false);
        StartPlay();
    }

    IEnumerator TimerMinigame(int seconds)
    {
        int counter = seconds;
        timerText.gameObject.SetActive(true);
        timerText.text = counter.ToString(); 

        while (counter > 0)
        {
            yield return new WaitForSeconds(1);
            counter--;
            timerText.text = counter.ToString();
        }

        timerText.gameObject.SetActive(true);
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
