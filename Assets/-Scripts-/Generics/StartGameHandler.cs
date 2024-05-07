using System.Collections;
using UnityEngine;

public class StartGameHandler : MonoBehaviour
{
    [SerializeField]
    MenuInfo mainMenu;
    [SerializeField]
    MenuInfo languageStartMenu;
    [SerializeField]
    GameObject pressAnyKeyObject;
    [SerializeField]
    Animation startAnimation;

    bool firstStart = true;

    // Start is called before the first frame update
    void Start()
    {
        StartingChecks();
        StartCoroutine(Utility.WaitForPlayers(StartMainMenu));
    }

    private void StartingChecks()
    {
        if (mainMenu == null)
            Debug.LogError("mainMenu not found on StartGameHandler");
        if(languageStartMenu == null)
            Debug.LogError("VolumeMenu not found on StartGameHandler");
        if(pressAnyKeyObject == null)
            Debug.LogError("pressAnyKeyObject not found on StartGameHandler");

        PlayerPrefsSettigsStartingCheck();

    }


    private void PlayerPrefsSettigsStartingCheck()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsSettings.FirstStart.ToString()))
        {
            firstStart = false;
            
            if (PlayerPrefs.HasKey(PlayerPrefsSettings.Languge.ToString()))
                GameManager.Instance.ChangeLanguage(PlayerPrefs.GetString(PlayerPrefsSettings.Languge.ToString()));
            else
                GameManager.Instance.ChangeLanguage("en");

            if (PlayerPrefs.HasKey(PlayerPrefsSettings.MasterVolume.ToString()))
                AudioManager.Instance.SetMasterVolume(PlayerPrefs.GetFloat(PlayerPrefsSettings.MasterVolume.ToString()));

            if (PlayerPrefs.HasKey(PlayerPrefsSettings.MusicVolume.ToString()))
                AudioManager.Instance.SetMusicVolume(PlayerPrefs.GetFloat(PlayerPrefsSettings.MusicVolume.ToString()));

            if (PlayerPrefs.HasKey(PlayerPrefsSettings.SFXVolume.ToString()))
                AudioManager.Instance.SetSoundFXVolume(PlayerPrefs.GetFloat(PlayerPrefsSettings.SFXVolume.ToString()));

        }
        else
            SaveManager.Instance.SavePlayerPref(PlayerPrefsSettings.FirstStart, "Game has been started for the first time");
    }


    private void StartMainMenu()
    {
        if (firstStart)
            OpenVolumeMenu();
        else
            OpenMainMenu();

        pressAnyKeyObject.SetActive(false);
    }

    private void OpenVolumeMenu()
    {
        MenuManager.Instance.OpenMenu(languageStartMenu, CoopManager.Instance.GetFirstPlayer());
        languageStartMenu.gameObject.SetActive(true);
    }

    public void OpenMainMenu()
    {
        StartCoroutine(StartMenuAnimation());
    }

    IEnumerator StartMenuAnimation()
    {
        startAnimation.Play();
        yield return new WaitForSeconds(startAnimation.clip.length);
        MenuManager.Instance.OpenMenu(mainMenu, CoopManager.Instance.GetFirstPlayer());
    }


}
