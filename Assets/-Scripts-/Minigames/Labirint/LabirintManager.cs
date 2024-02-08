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
    Grid grid;
    [SerializeField]
    List<Labirint> Labirints;
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
        currentLabirint = Labirints[Random.Range(0, Labirints.Count)];
        foreach (Labirint labirint in Labirints)
        {
            labirint.gameObject.SetActive(false);
        }
        currentLabirint.gameObject.SetActive(true);
        SetElements(currentLabirint.GetEnemySpawnPoints(), enemyCount);
        SetElements(currentLabirint.GetKeySpawnPoints(), keyCount);
        SetElements(currentLabirint.GetPlayerSpawnPoints(), playerCount);
        pickedKey = 0;
        deadPlayerCount = 0;
        SetEnemyes(currentLabirint.GetEnemySpawnPoints());
        SetPlayers(currentLabirint.GetPlayerSpawnPoints());
    }

    private void SetPlayers(List<GameObject> gameObjects)
    {
        foreach (GameObject obj in gameObjects)
        {
            if (obj.activeInHierarchy)
            {
                GameObject.Instantiate(playerPrefab, obj.transform.position, Quaternion.identity);
                obj.SetActive(false);
            }
        }
    }

    private void SetEnemyes(List<GameObject> gameObjects)
    {
        foreach (GameObject obj in gameObjects)
        {
            if (obj.activeInHierarchy)
            {
                GameObject.Instantiate(enemyPrefab, obj.transform.position, Quaternion.identity);
                obj.SetActive(false);
            }

        }
    }

    private void SetElements(List<GameObject> objects, int quantity)
    {
        if (quantity < objects.Count)
        {
            for (int i = 0; i < objects.Count - quantity; i++)
            {
                GameObject obj = objects[Random.Range(0, objects.Count)];
                while (!obj.activeInHierarchy)
                {
                    obj = objects[Random.Range(0, objects.Count)];
                }
                obj.SetActive(false);
            }
        }
    }
    #endregion
    
}
