using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private static MenuManager _instance;
    public static MenuManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MenuManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("MenuManager");
                    _instance = singletonObject.AddComponent<MenuManager>();
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


    public void SetPlayerActiveMenu(PlayerInputHandler player, GameObject menuRoot, GameObject firstSelection)
    {
        player.SetPlayerActiveMenu(menuRoot, firstSelection);
    }   


}
