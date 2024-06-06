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
    [SerializeField] Press pressPrefab;
    [SerializeField] float pressTimerAttackPreview;
    [SerializeField] float pressTimerBetweenAttacks;
    [SerializeField] float pressSpeed;
    [SerializeField] float pressSpeedIncreaseMultyOnPlayerDeath;
    
    [Header("TrashRain Settings")]
    [SerializeField] List<GameObject> trashSpawnPoints;
    [SerializeField] Trash trashPrefab;
    [SerializeField] float minTrashFallSpeed;
    [SerializeField] float maxTrashFallSpeed;
    [SerializeField] float trashTimerBetweenAttacks;
    
    [Header("Player Settings")]
    [SerializeField] List<GameObject> playerSpawnPoints;
    [SerializeField] GameObject playerPrefab;
    
    [Header("Dialogue Settings")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private Dialogue winDialogue;
    [SerializeField] private UnityEvent onWinDialogueEnd;
    [SerializeField] private Dialogue loseDialogue;
    [SerializeField] private UnityEvent onLoseDialogueEnd;
   
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

  
    public bool canSpawnPress = true;
    public bool canSpawnTrash = true;
    public bool canChangePhase = true;
    private DialogueBox _dialogueBox;
    private List<GameObject> trashSpawnPointsCopy = new();
    List<TrashPressPlayer> players = new();







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
        
        foreach (GameObject spawn in trashSpawnPoints)
        {
            trashSpawnPointsCopy.Add(spawn);
        }

        canSpawnPress = true;
        canSpawnTrash = true;

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
        Debug.Log("InizioMinigame");
        StartCoroutine(PressGameplay());
    }
    #endregion

    #region GameFlow

    IEnumerator PressGameplay()
    {
        float tempTimer = 0f;

        while (tempTimer < pressPhaseDuration)
        {
            // Chiama la funzione
            SpawnPress();

            // Aspetta un intervallo di tempo (per esempio, 1 secondo)
            yield return new WaitForSeconds(1f);

            // Incrementa il timer
            tempTimer += 1f;

            
        }
        // Attendi fino a quando canChangePhase è true
        while (!canChangePhase)
        {
            yield return null; // Aspetta il prossimo frame
        }

        StartCoroutine(TrashGameplay());

    }
    IEnumerator TrashGameplay()
    {
        StopCoroutine(PressGameplay());

        float tempTimer = 0f;

        while (tempTimer < pressPhaseDuration)
        {
            // Chiama la funzione
            Invoke(nameof(SpawnTrash), trashTimerBetweenAttacks);

            // Aspetta un intervallo di tempo (per esempio, 1 secondo)
            yield return new WaitForSeconds(1f);

            // Incrementa il timer
            tempTimer += 1f;
        }

        StartCoroutine(PressTrashGameplay());

    }
    IEnumerator PressTrashGameplay()
    {
        StopCoroutine(TrashGameplay());
        yield return new WaitForSeconds(pressTrashPhaseDuration);

    }


    private void SpawnPress()
    {
        if (canSpawnPress)
        {
            canSpawnPress = false;
            canChangePhase = false;

            int randomInt = Random.Range(0, 2);
            int randomInt2;
            Press press1 = null;
            Press press2 = null;

            //spawn 1 press

            randomInt = Random.Range(0, 3);
            press1 = Instantiate(pressPrefab, pressSpawnPoints[randomInt].transform);
            press1.transform.localPosition = Vector3.zero;



            //spawn 2 Press
            if (Random.Range(0, 2) == 1)
            {
                do
                {
                    randomInt2 = Random.Range(0, 3);
                }
                while (randomInt2 == randomInt);

                press2 = Instantiate(pressPrefab, pressSpawnPoints[randomInt2].transform);
                press2.transform.localPosition = Vector3.zero;

            }

            //Inizio Movimento Piattaforme
            StartCoroutine(press1.Activate(pressSpeed * (deadPlayerCounter > 0 ? deadPlayerCounter + 1 : 1) * pressSpeedIncreaseMultyOnPlayerDeath, pressTimerAttackPreview));

            if (press2 != null)
                StartCoroutine(press2.Activate(pressSpeed * (deadPlayerCounter > 0 ? deadPlayerCounter + 1 : 1) * pressSpeedIncreaseMultyOnPlayerDeath, pressTimerAttackPreview));

        }

    }

    private void SpawnTrash()
    {
        if (trashSpawnPointsCopy.Count > 0)
        {
            int randomInt = Random.Range(0, trashSpawnPoints.Count);
            Trash spawnedTrash = Instantiate(trashPrefab, trashSpawnPointsCopy[randomInt].transform);
            spawnedTrash.Drop(Random.Range(minTrashFallSpeed, maxTrashFallSpeed));
            trashSpawnPointsCopy.RemoveAt(randomInt);
        }
        else
        {
            trashSpawnPointsCopy.Clear();
            foreach (GameObject spawn in trashSpawnPoints)
            {
                trashSpawnPointsCopy.Add(spawn);
            }

            int randomInt = Random.Range(0, trashSpawnPointsCopy.Count);
            Trash spawnedTrash = Instantiate(trashPrefab, trashSpawnPointsCopy[randomInt].transform);
            spawnedTrash.Drop(Random.Range(minTrashFallSpeed, maxTrashFallSpeed));
            trashSpawnPointsCopy.RemoveAt(randomInt);


        }


    }
    #endregion



    #region misc
    public void AddPlayer(TrashPressPlayer trashPressPlayer)
    {
        players.Add(trashPressPlayer);
    }


    #endregion
}