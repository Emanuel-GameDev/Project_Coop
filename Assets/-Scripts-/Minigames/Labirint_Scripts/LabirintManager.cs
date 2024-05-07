using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class LabirintManager : MonoBehaviour
{
    private static LabirintManager _instance;
    public static LabirintManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LabirintManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("LabirintManager");
                    _instance = singletonObject.AddComponent<LabirintManager>();
                }
            }

            return _instance;
        }
    }
    [Header("Labirint Settings")]
    [SerializeField]
    int enemyCount = 4;
    [SerializeField]
    int keyCount = 10;
    [SerializeField]
    GameObject playerPrefab;
    [SerializeField]
    GameObject enemyPrefab;
    [SerializeField]
    GameObject keyPrefab;
    [SerializeField]
    Grid grid;
    [SerializeField]
    List<Labirint> Labirints;

    List<GameObject> objectsForTheGame;
    List<LabirintPlayer> players = new();

    int PlayerCount => players.Count;
    int deadPlayerCount = 0;
    Labirint currentLabirint;
    int pickedKey;
    public Grid Grid => grid;


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


    [Header("Rewards Settings")]
    [SerializeField]
    private int coinForEachPlayer;
    [SerializeField]
    private int coinForFirstPlayer;
    [SerializeField]
    private int keyForEachPlayer;
    [SerializeField]
    private int keyForFirstPlayer;
    [SerializeField]
    private WinScreenHandler winScreenHandler;

    [Header("End Screen Settings")]
    [SerializeField]
    private GameObject victoryScreen;
    [SerializeField]
    private GameObject loseScreen;
    [SerializeField]
    private float victoryLoseScreenTime = 5f;

    [Header("Audio Settings")]
    [SerializeField]
    AudioSource mainThemeSource;
    [SerializeField]
    private float attenuationVolume = 0.5f;
    [SerializeField]
    private AudioClip victoryClip;
    [SerializeField]
    private AudioClip loseClip;


    [Header("VFX Settings")]
    [SerializeField]
    private GameObject gameVolume;

    [SerializeField]
    private GameObject endVolume;

    private LabirintUI labirintUI;
    private SceneChanger sceneChanger;

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

        ChangeGameVolume(true);

    }

    private void Start()
    {
        StartCoroutine(WaitForPlayers());
        sceneChanger = GetComponent<SceneChanger>();
    }

    public void StartPlay()
    {
        SetupLabirint();
        StartGame();
    }

    #region GameManagement
    public void PickedKey(ePlayerCharacter character, int pickedKeys)
    {
        pickedKey++;
        labirintUI.UpdatePickedKey(character, pickedKeys);
        labirintUI.UpdateRemainingKeyCount(keyCount - pickedKey);
        if (pickedKey >= keyCount)
            EndGame(true);
    }

    public void PlayerDead()
    {
        deadPlayerCount++;
        if (deadPlayerCount >= PlayerCount)
            EndGame(false);
    }

    public void StartGame()
    {
        labirintUI.gameObject.SetActive(true);
        labirintUI.UpdateRemainingKeyCount(keyCount);
        labirintUI.SetAllPlayer(CoopManager.Instance.GetActiveHandlers());

        foreach (GameObject obj in objectsForTheGame)
        {
            obj.SetActive(true);
        }
    }

    private void EndGame(bool playerWin)
    {
        labirintUI.gameObject.SetActive(false);
        StartCoroutine(EndgameTimer(playerWin));

        if (playerWin)
        {
            AudioManager.Instance.PlayAudioClip(victoryClip);
            StartCoroutine(AttenuateManinTheme(victoryClip.length));
        }
        else
        {
            AudioManager.Instance.PlayAudioClip(loseClip);
            StartCoroutine(AttenuateManinTheme(loseClip.length));
        }

        foreach (GameObject obj in objectsForTheGame)
        {
            obj.SetActive(false);
        }

        MakeRankList();
    }

    IEnumerator EndgameTimer(bool playerWin)
    {
        _dialogueBox.RemoveAllDialogueEnd();

        if (playerWin)
        {
            victoryScreen.SetActive(true);
            yield return new WaitForSeconds(victoryLoseScreenTime);
            victoryScreen.SetActive(false);

            _dialogueBox.SetDialogue(winDialogue);
            _dialogueBox.AddDialogueEnd(onWinDialogueEnd);
        }
        else
        {
            loseScreen.SetActive(true);
            yield return new WaitForSeconds(victoryLoseScreenTime);
            loseScreen.SetActive(false);

            _dialogueBox.SetDialogue(loseDialogue);
            _dialogueBox.AddDialogueEnd(onLoseDialogueEnd);
        }

        dialogueBox.SetActive(true);
        _dialogueBox.StartDialogue();
    }

    public void ChangeGameVolume(bool game)
    {
        if (game)
        {
            gameVolume.SetActive(true);
            endVolume.SetActive(false);
        }
        else
        {
            gameVolume.SetActive(false);
            endVolume.SetActive(true);
        }
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

    IEnumerator AttenuateManinTheme(float duration)
    {
        mainThemeSource.volume = attenuationVolume;
        yield return new WaitForSeconds(duration);
        mainThemeSource.volume = 1f;
    }

    #endregion

    #region Labirint Setup
    public void SetupLabirint()
    {
        ResetLabirint();
        currentLabirint = Labirints[Random.Range(0, Labirints.Count)];
        foreach (Labirint labirint in Labirints)
        {
            labirint.gameObject.SetActive(false);
        }
        currentLabirint.gameObject.SetActive(true);
        SetElements(currentLabirint.GetEnemySpawnPoints(), enemyCount, enemyPrefab);
        SetElements(currentLabirint.GetKeySpawnPoints(), keyCount, keyPrefab);
        SetPlayers(currentLabirint.GetPlayerSpawnPoints());
        pickedKey = 0;
        deadPlayerCount = 0;
        currentLabirint.DisableObjectMap();
    }

    private void ResetLabirint()
    {
        if (currentLabirint != null)
        {
            currentLabirint.EnableObjectMap();
            currentLabirint.gameObject.SetActive(false);
            currentLabirint = null;
        }

        if (objectsForTheGame != null)
        {
            foreach (GameObject obj in objectsForTheGame)
            {
                Destroy(obj);
            }
        }
        objectsForTheGame = new();
    }

    private void SetPlayers(List<Vector3Int> positions)
    {
        foreach (LabirintPlayer player in players)
        {
            int randomIndex = Random.Range(0, positions.Count);
            Vector3Int position = positions[randomIndex];
            positions.RemoveAt(randomIndex);
            player.transform.position = grid.GetCellCenterWorld(position);
            player.transform.SetParent(Grid.transform);
            player.transform.localScale = Vector3.one;
            player.Inizialize();
            player.gameObject.SetActive(true);
        }
    }

    private void SetElements(List<Vector3Int> positions, int quantity, GameObject element)
    {
        for (int i = 0; i < quantity; i++)
        {
            if (positions.Count == 0)
            {
                keyCount = i;
                return;
            }
            else
            {
                int randomIndex = Random.Range(0, positions.Count);
                Vector3Int position = positions[randomIndex];
                positions.RemoveAt(randomIndex);
                GameObject obj = GameObject.Instantiate(element, grid.GetCellCenterWorld(position), Quaternion.identity, Grid.transform);
                obj.SetActive(false);
                objectsForTheGame.Add(obj);
            }

        }
    }

    public void SetLabirintUI(LabirintUI UI)
    {
        labirintUI = UI;
    }

    #endregion

    #region Misc
    public Tilemap GetWallMap()
    {
        return currentLabirint.WallTilemap;
    }

    public void AddPlayer(LabirintPlayer labirintPlayer)
    {
        players.Add(labirintPlayer);
    }

    private void MakeRankList()
    {
        List<LabirintPlayer> winOrder = new();

        foreach (LabirintPlayer player in players)
        {
            winOrder.Add(player);
        }

        winOrder.Sort((x, y) => y.pickedKeys.CompareTo(x.pickedKeys));

        List<ePlayerCharacter> ranking = new();

        foreach (LabirintPlayer player in winOrder)
        {
            ranking.Add(player.GetCharacter());
        }

        foreach (ePlayerCharacter c in Enum.GetValues(typeof(ePlayerCharacter)))
        {
            if (c != ePlayerCharacter.EmptyCharacter)
            {
                if (!ranking.Contains(c))
                {
                    ranking.Add(c);
                }
            }
        }

        bool yetCompleted = CheckAndSaveYetCompleted();
         
        for (int i = 0; i < ranking.Count; i++)
        {
            int totalCoin = 0;
            int totalKey = 0;
            int gainedCoin = 0;
            int gainedKey = 0;

            CharacterSaveData saveData = SaveManager.Instance.GetPlayerSaveData(ranking[i]);

            if (saveData != null)
            {
                totalCoin = saveData.extraData.coin;
                totalKey = saveData.extraData.key;
            }

            Debug.Log(yetCompleted);

            if (!yetCompleted)
            {
                if (i == 0)
                {
                    totalCoin += coinForFirstPlayer;
                    totalKey += keyForFirstPlayer;

                    gainedCoin += coinForFirstPlayer;
                    gainedKey += keyForFirstPlayer;
                }
                else
                {
                    totalCoin += coinForEachPlayer;
                    totalKey += keyForEachPlayer;

                    gainedCoin += coinForEachPlayer;
                    gainedKey += keyForEachPlayer;

                }

                if (saveData != null)
                {
                    saveData.extraData.coin = totalCoin;
                    saveData.extraData.key = totalKey;
                }
            }

            Enum.TryParse<Rank>(i.ToString(), out Rank rank);
            winScreenHandler.SetCharacterValues(ranking[i], rank, gainedCoin, totalCoin, gainedKey, totalKey);

        }

        SaveManager.Instance.SaveData();
    }

    private bool CheckAndSaveYetCompleted()
    {
        SceneSetting sceneSetting = SaveManager.Instance.GetSceneSetting(SceneSaveSettings.Passepartout);
        if (sceneSetting == null)
            sceneSetting = new SceneSetting(SceneSaveSettings.Passepartout);
        if (!sceneSetting.GetBoolValue("Completed"))
        {
            sceneSetting.AddBoolValue("Completed", true);
            SaveManager.Instance.SaveSceneData(sceneSetting);
            return false;
        }
        else 
            return true;  
    }

    #endregion

}
