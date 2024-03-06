using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private List<PlayerCharacterData> playerCharacterDatas;

    public PlayerInputManager playerInputManager { get; private set; }
    public CoopManager coopManager { get; private set; }
    public CameraManager cameraManager { get; private set; }
    public GameObject BossGameobject; //Debug;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("GameManager");
                    _instance = singletonObject.AddComponent<GameManager>();
                }
            }

            return _instance;
        }
    }

    private Dictionary<string, AsyncOperation> sceneLoadOperations = new Dictionary<string, AsyncOperation>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        playerInputManager = PlayerInputManager.instance;
        coopManager = CoopManager.Instance;
        cameraManager = CameraManager.Instance;

        if (cameraManager != null && coopManager != null)
            cameraManager.AddAllPlayers();

        if (playerCharacterDatas == null || playerCharacterDatas.Count <= 0)
        {
            Debug.LogError("No player character datas found");
        }

    }

    private void Update()
    {
        //Debug start
        
        if (Input.GetKeyDown(KeyCode.B))
            {
            BossGameobject.SetActive(true);
        }
        //Debug End
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    #region Scene Management

    public void ChangeScene(string sceneName)
    {
        if (sceneLoadOperations.ContainsKey(sceneName))
        {
            if (!sceneLoadOperations[sceneName].isDone)
                StartLoadScreen();

            ActivateLoadedScene(sceneName);
        }
    }

    public void LoadSceneInbackground(string sceneName)
    {
        if (!sceneLoadOperations.ContainsKey(sceneName))
        {
            AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
            asyncOp.allowSceneActivation = false;
            sceneLoadOperations.Add(sceneName, asyncOp);
        }
    }

    public void ActivateLoadedScene(string sceneName)
    {
        if (sceneLoadOperations.ContainsKey(sceneName))
        {
            foreach (var kvp in sceneLoadOperations)
            {
                if (kvp.Key != sceneName)
                {
                    SceneManager.UnloadSceneAsync(kvp.Key);
                }
            }

            sceneLoadOperations[sceneName].allowSceneActivation = true;
        }
    }

    public void CancelSceneLoad(string sceneName)
    {
        if (sceneLoadOperations.ContainsKey(sceneName))
        {
            sceneLoadOperations[sceneName].allowSceneActivation = false;
            SceneManager.UnloadSceneAsync(sceneName);
            sceneLoadOperations.Remove(sceneName);
        }
    }

    public bool IsSceneLoaded(string sceneName)
    {
        if (sceneLoadOperations.ContainsKey(sceneName))
        {
            return sceneLoadOperations[sceneName].isDone;
        }
        else
            return false;
    }


    public void LoadScene(int id)
    {
        SceneManager.LoadScene(id);
    }
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartLoadScreen()
    {
        //Do Something
    }

    #endregion

    public void ExitGame()
    {
        Application.Quit();
    }

    public List<PlayerCharacterData> GetCharacterDataList() => playerCharacterDatas;

    public PlayerCharacterData GetCharacterData(ePlayerCharacter character)
    {
        foreach (PlayerCharacterData characterData in playerCharacterDatas)
        {
            if (characterData.Character == character)
            {
                return characterData;
            }
        }

        Debug.LogError($"Character {character} not found");
        return null;
    }

}
