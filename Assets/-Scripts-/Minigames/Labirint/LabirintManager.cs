using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    List<GameObject> objectsForTheMatch;

    int playerCount = 1;
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
        stateMachine.SetState(new StartLabirint());
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
        if (deadPlayerCount >= playerCount)
            EndGame(false);
    }

    private void EndGame(bool playerWin)
    {
        if(playerWin)
            Debug.Log("End Game: You Win");
        else
            Debug.Log("End Game: You Lose");
    }

    #endregion

    #region Labirint Setup
    private void SetupLabirint()
    {
        objectsForTheMatch = new();
        currentLabirint = Labirints[Random.Range(0, Labirints.Count)];
        foreach (Labirint labirint in Labirints)
        {
            labirint.gameObject.SetActive(false);
        }
        currentLabirint.gameObject.SetActive(true);
        SetElements(currentLabirint.GetEnemySpawnPoints(), enemyCount, enemyPrefab);
        SetElements(currentLabirint.GetKeySpawnPoints(), keyCount, keyPrefab);
        SetElements(currentLabirint.GetPlayerSpawnPoints(), playerCount, playerPrefab);
        pickedKey = 0;
        deadPlayerCount = 0;
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
                GameObject obj = GameObject.Instantiate(element, grid.CellToWorld(position), Quaternion.identity);
                obj.SetActive(false);
                objectsForTheMatch.Add(obj);
            }

        }
    }
    #endregion
    
}
