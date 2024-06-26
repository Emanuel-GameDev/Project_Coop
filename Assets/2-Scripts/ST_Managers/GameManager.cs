using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject loadScreen;

    [SerializeField]
    private float fakeLoadSceenTime = 3f;

    [SerializeField]
    private List<PlayerCharacterData> playerCharacterDatas;

    private CoopManager coopManager;
    private AsyncOperation sceneLoadOperation;

    public CoopManager CoopManager => coopManager;
    public bool IsLoading => loadScreen.activeInHierarchy;

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
        coopManager = CoopManager.Instance;

        if (playerCharacterDatas == null || playerCharacterDatas.Count <= 0)
        {
            Debug.LogError("No player character datas found");
        }

        if (loadScreen != null)
        {
            loadScreen = Instantiate(loadScreen);
            DontDestroyOnLoad(loadScreen);
        }

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
        LoadSceneInbackground(sceneName);
        StartLoadScreen();
    }

    public void LoadSceneInbackground(string sceneName)
    {
        if (sceneLoadOperation == null)
        {
            sceneLoadOperation = SceneManager.LoadSceneAsync(sceneName);
            sceneLoadOperation.allowSceneActivation = false;
        }
    }

    public void ActivateLoadedScene()
    {
        StartLoadScreen();
    }

    private void ActivateScene()
    {
        sceneLoadOperation.allowSceneActivation = true;
        sceneLoadOperation = null;
    }

    public bool IsSceneLoaded()
    {
        if (sceneLoadOperation == null)
            return false;

        return sceneLoadOperation.progress >= 0.9f;
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
        PubSub.Instance.Notify(EMessageType.sceneLoading, null);
        SaveManager.Instance.SaveData();
        StartCoroutine(LoadScreen());
    }

    IEnumerator LoadScreen()
    {
        float loadTime = Time.time;
        Utility.DebugTrace($"Start Load Time: {Time.time}");
        CoopManager.Instance.DisableAllInput();
        loadScreen.SetActive(true);
        yield return new WaitUntil(() => IsSceneLoaded());
        ActivateScene();
        Utility.DebugTrace($"End Load Time: {Time.time}");
        if (Time.time - loadTime < fakeLoadSceenTime)
            yield return new WaitForSeconds(fakeLoadSceenTime - (Time.time - loadTime));
        loadScreen.SetActive(false);
        CoopManager.Instance.EnableAllInput();
        Utility.DebugTrace($"Total Load Time: {Time.time - loadTime}");
    }

    #endregion

    public void ExitGame()
    {
        Debug.Log("Exit Game");
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

    #region Localization

    public void ChangeLanguage(string language)
    {
        Locale selectedLocale = LocalizationSettings.AvailableLocales.Locales.Find(x => x.LocaleName == language);

        if (selectedLocale != null)
        {
            LocalizationSettings.SelectedLocale = selectedLocale;
        }
        else
        {
            Debug.LogWarning("Lingua non trovata: " + language);
        }

    }

    public void NextLanguage()
    {
        int currentIndex = LocalizationSettings.AvailableLocales.Locales.FindIndex(x => x.LocaleName == LocalizationSettings.SelectedLocale.LocaleName);

        if (currentIndex == -1 || currentIndex == LocalizationSettings.AvailableLocales.Locales.Count - 1)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        }
        else
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[currentIndex + 1];
        }


    }
    public void PreviousLanguage()
    {
        int currentIndex = LocalizationSettings.AvailableLocales.Locales.FindIndex(x => x.LocaleName == LocalizationSettings.SelectedLocale.LocaleName);

        if (currentIndex == -1 || currentIndex == 0)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[LocalizationSettings.AvailableLocales.Locales.Count - 1];
        }
        else
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[currentIndex - 1];
        }
    }
   

    #endregion
}
