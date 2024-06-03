using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Experimental.GraphView.GraphView;

public class TrashPressManager : MonoBehaviour
{
    private static TrashPressManager _instance;
    public static TrashPressManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TrashPressManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("TrashPressManager");
                    _instance = singletonObject.AddComponent<TrashPressManager>();
                }
            }

            return _instance;
        }
    }

    [Header("GameFlow Settings")]
    [SerializeField] int pressPhaseDuration;
    [SerializeField] int trashPhaseDuration;
    [SerializeField] int pressTrashPhaseDuration;

    [Header("Press Settings")]
    [SerializeField] List<GameObject> pressSpawnPoints;
    [SerializeField] float pressTimerAttackPreview;
    [SerializeField] float pressTimerBetweenAttacks;
    [SerializeField] float pressSpeedIncreaseOnPlayerDeath;

    [Header("TrashRain Settings")]
    [SerializeField] List<GameObject> trashSpawnPoints;
    [SerializeField] float minTrashFallSpeed;
    [SerializeField] float maxTrashFallSpeed;
    [SerializeField] float trashTimerBetweenAttacks;

    [Header("Player Settings")]
    [SerializeField] List<GameObject> playerSpawnPoints;
    [SerializeField] GameObject playerPrefab;
    List<TrashPressPlayer> players = new();

    [Header("Dialogue Settings")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private Dialogue winDialogue;
    [SerializeField] private UnityEvent onWinDialogueEnd;
    [SerializeField] private Dialogue loseDialogue;
    [SerializeField] private UnityEvent onLoseDialogueEnd;
    private DialogueBox _dialogueBox;

    [Header("Rewards Settings")]
    [SerializeField] private int coinForEachPlayer;
    [SerializeField] private int coinForFirstPlayer;
    [SerializeField] private int keyForEachPlayer;
    [SerializeField] private int keyForFirstPlayer;
    [SerializeField] private WinScreenHandler winScreenHandler;

    [Header("End Screen Settings")]
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private float victoryLoseScreenTime = 5f;

    [Header("Audio Settings")]
    [SerializeField] AudioSource mainThemeSource;
    [SerializeField] private float attenuationVolume = 0.5f;
    [SerializeField] private AudioClip victoryClip;
    [SerializeField] private AudioClip loseClip;


    private void Awake()
    {
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

    }

    private void Start()
    {
        SetPlayers(playerSpawnPoints);
    }
    private void SetPlayers(List<GameObject> positions)
    {
        foreach (TrashPressPlayer player in players)
        {
            int randomIndex = Random.Range(0, positions.Count);
            Vector3 spawnPosition = positions[randomIndex].transform.position;
            positions.RemoveAt(randomIndex);
            player.transform.position = spawnPosition;
            player.gameObject.SetActive(true);
        }
    }
}