using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;


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
    [SerializeField] Press pressPrefab;
    [SerializeField] List<GameObject> pressSpawnPoints;
    [SerializeField] float pressTimerBetweenAttacks;
    [SerializeField] float pressTimerAttackPreview;
    [SerializeField] float pressSpeed;
    [SerializeField] float pressSpeedIncreaseMultyOnPlayerDeath;
    
    [Header("TrashRain Settings")]
    [SerializeField] Trash trashPrefab;
    [SerializeField] List<GameObject> trashSpawnPoints;   
    [SerializeField] float minTrashFallSpeed;
    [SerializeField] float maxTrashFallSpeed;
    [SerializeField] float trashTimerBetweenAttacks;
    
    [Header("Player Settings")]
    [SerializeField] GameObject playerPrefab;
    [SerializeField] List<GameObject> playerSpawnPoints;
   
    
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
    public int deadPlayerCounter = 0;

  
    [HideInInspector] public bool canSpawnPress = true;
    [HideInInspector] public bool canSpawnTrash = true;
    [HideInInspector] public bool canChangePhase = true;
    private int idTrashSpawner;
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
        canSpawnTrash = false;

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
    public void PlayerDead()
    {
        deadPlayerCounter++;
        if (deadPlayerCounter >= players.Count)
        {
            //EndGame(false);
        }

    }
    //private void EndGame(bool playerWin)
    //{
    //    labirintUI.gameObject.SetActive(false);
    //    StartCoroutine(EndgameTimer(playerWin));

    //    if (playerWin)
    //    {
    //        AudioManager.Instance.PlayAudioClip(victoryClip);
    //        StartCoroutine(AttenuateManinTheme(victoryClip.length));
    //    }
    //    else
    //    {
    //        AudioManager.Instance.PlayAudioClip(loseClip);
    //        StartCoroutine(AttenuateManinTheme(loseClip.length));
    //    }

    //    foreach (GameObject obj in objectsForTheGame)
    //    {
    //        obj.SetActive(false);
    //    }

    //    MakeRankList();
    //}
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
        canSpawnTrash = true;
        canSpawnPress = true;
        StartCoroutine(TrashGameplay());

    }
    IEnumerator TrashGameplay()
    {
        StopCoroutine(PressGameplay());

        float tempTimer = 0f;
        idTrashSpawner = 0;
        trashSpawnPointsCopy = Utility.Shuffle(trashSpawnPointsCopy);

        while (tempTimer < trashPhaseDuration)
        {
           
            SpawnTrash();       
            yield return new WaitForSeconds(trashTimerBetweenAttacks);          
            tempTimer += trashTimerBetweenAttacks;
        }

        StartCoroutine(PressTrashGameplay());

    }
    IEnumerator PressTrashGameplay()
    {
        StopCoroutine(TrashGameplay());
        float tempTimer = 0f;
        idTrashSpawner = 0;
        trashSpawnPointsCopy = Utility.Shuffle(trashSpawnPointsCopy);

        while (tempTimer < pressTrashPhaseDuration)
        {
            SpawnPress();
            SpawnTrash();
            yield return new WaitForSeconds(trashTimerBetweenAttacks);
            tempTimer += trashTimerBetweenAttacks;
        }
        yield return new WaitForSeconds(pressTrashPhaseDuration);
        StopCoroutine(PressTrashGameplay());

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
    public IEnumerator SetCanSpawnPress()
    {
        yield return new WaitForSeconds(pressTimerBetweenAttacks);
        canSpawnPress = true;
    }

    private void SpawnTrash()
    {
       
        if (idTrashSpawner <= trashSpawnPointsCopy.Count-1)
        {          
            Trash spawnedTrash = Instantiate(trashPrefab, trashSpawnPointsCopy[idTrashSpawner].transform);
            spawnedTrash.transform.localPosition = Vector3.zero;
            spawnedTrash.Drop(Random.Range(minTrashFallSpeed, maxTrashFallSpeed));
            idTrashSpawner++;
        }
        else
        {
            trashSpawnPointsCopy = Utility.Shuffle(trashSpawnPointsCopy);
            idTrashSpawner = 0;
            Trash spawnedTrash = Instantiate(trashPrefab, trashSpawnPointsCopy[idTrashSpawner].transform);
            spawnedTrash.transform.localPosition = Vector3.zero;
            spawnedTrash.Drop(Random.Range(minTrashFallSpeed, maxTrashFallSpeed));
            idTrashSpawner++;
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