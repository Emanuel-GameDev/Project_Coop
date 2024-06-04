using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


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
    [SerializeField] int pressPhaseDuration = 30;
    [SerializeField] int trashPhaseDuration = 30;
    [SerializeField] int pressTrashPhaseDuration = 30;

    [Header("Press Settings")]
    [SerializeField] List<GameObject> pressSpawnPoints;
    [SerializeField] List<Press> pressPrefabs;
    [SerializeField] float pressTimerAttackPreview;
    [SerializeField] float pressTimerBetweenAttacks;
    [SerializeField] float pressSpeed;
    [SerializeField] float pressSpeedIncreaseMultyOnPlayerDeath;

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

    private SceneChanger sceneChanger;
    private int deadPlayerCounter = 0;

    private enum GamePhase
    {
        PressPhase,
        TrashPhase,
        PressTrashPhase
    }
    private GamePhase currentGamePhase;



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
        StartCoroutine(WaitForPlayers());
        sceneChanger = GetComponent<SceneChanger>();
        currentGamePhase = GamePhase.PressPhase;

    }


    #region GameManagement
    public void StartPlay()
    {
        SetPlayers(playerSpawnPoints);
    }
    public void ExitMinigame()
    {
        if (sceneChanger != null)
            sceneChanger.ChangeScene();
    }
    IEnumerator WaitForPlayers()
    {
        yield return new WaitUntil(() => CoopManager.Instance.GetActiveHandlers() != null && CoopManager.Instance.GetActiveHandlers().Count > 0);
        dialogueBox.SetActive(true);
        _dialogueBox.StartDialogue();
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

    public void StartMinigame()
    {
        currentGamePhase = GamePhase.PressPhase;
    }
    #endregion

    #region GameFlow

    IEnumerator PressGameplay()
    {
        InvokeRepeating(nameof(SpawnPress), pressTimerBetweenAttacks, pressPhaseDuration - 1);
        yield return new WaitForSeconds(pressPhaseDuration);
        StartCoroutine(TrashGameplay());

    }
    IEnumerator TrashGameplay()
    {
        yield return new WaitForSeconds(trashPhaseDuration);
        StartCoroutine(PressTrashGameplay());

    }
    IEnumerator PressTrashGameplay()
    {
        yield return new WaitForSeconds(pressTrashPhaseDuration);

    }


    private void SpawnPress()
    {

        int randomInt = Random.Range(0, 2);
        int randomInt2;

        //spawn 1 press
        if (randomInt == 0)
        {
            randomInt = Random.Range(0, 2);
            pressPrefabs[randomInt].Activate(pressSpeed * (deadPlayerCounter > 0 ? deadPlayerCounter + 1 : 1) * pressSpeedIncreaseMultyOnPlayerDeath,pressTimerAttackPreview);

        }
        //spawn 2 Press
        else
        {
            randomInt = Random.Range(0, 2);
            pressPrefabs[randomInt].Activate(pressSpeed * (deadPlayerCounter > 0 ? deadPlayerCounter + 1 : 1) * pressSpeedIncreaseMultyOnPlayerDeath, pressTimerAttackPreview);
            do
            {
                randomInt2 = Random.Range(0, 2);
            }
            while (randomInt2 == randomInt);

            pressPrefabs[randomInt2].Activate(pressSpeed * (deadPlayerCounter > 0 ? deadPlayerCounter + 1 : 1) * pressSpeedIncreaseMultyOnPlayerDeath, pressTimerAttackPreview);
        }

    }
    private void SpawnTrash()
    {

    }
    #endregion



    #region misc
    public void AddPlayer(TrashPressPlayer trashPressPlayer)
    {
        players.Add(trashPressPlayer);
    }
    #endregion
}