using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    private StateMachine<LabirintState> stateMachine = new StateMachine<LabirintState>();


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
    }

    private void Start()
    {
        //SetupLabirint();
        stateMachine.SetState(new MenuLabirint());
        //Debug
        //StartGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SetupLabirint();
            StartGame();
        }
    }

    #region GameManagement
    public void PickedKey()
    {
        pickedKey++;
        if (pickedKey >= keyCount)
            EndGame(true);
    }

    public void PlayerDead()
    {
        deadPlayerCount++;
        if (deadPlayerCount >= PlayerCount)
            EndGame(false);
    }

    private void StartGame()
    {
        currentLabirint.DisableObjectMap();
        foreach (GameObject obj in objectsForTheGame)
        {
            obj.SetActive(true);
        }
    }

    private void EndGame(bool playerWin)
    {
        if (playerWin)
            Debug.Log("End Game: You Win");
        else
            Debug.Log("End Game: You Lose");
    }

    #endregion

    #region Labirint Setup
    private void SetupLabirint()
    {
        objectsForTheGame = new();
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
            //player.transform.SetParent(null);
            player.Inizialize();
        }
    }

    private void SetElements(List<Vector3Int> positions, int quantity, GameObject element)
    {
        for (int i = 0; i < quantity; i++)
        {
            if (i >= positions.Count)
                return;
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
    #endregion

}
