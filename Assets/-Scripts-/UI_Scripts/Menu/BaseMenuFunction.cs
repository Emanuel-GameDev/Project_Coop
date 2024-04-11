using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMenuFunction : MonoBehaviour
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

}
