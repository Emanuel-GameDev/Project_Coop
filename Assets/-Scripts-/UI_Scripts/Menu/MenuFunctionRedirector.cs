using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuFunctionRedirector : MonoBehaviour
{
    public void Resume()
    {
        MenuManager.Instance.CloseMenu();
    }

    public void ExitGame()
    {
        GameManager.Instance.ExitGame();
    }

    public void CloseMenu()
    {
        MenuManager.Instance.CloseMenu();
    }

    public void NextLanguage()
    {
        GameManager.Instance.NextLanguage();
    }

    public void PreviousLanguage()
    {
        GameManager.Instance.PreviousLanguage();
    }

    public void ChangeLanguage(string language)
    {
        GameManager.Instance.ChangeLanguage(language);
    }

}
