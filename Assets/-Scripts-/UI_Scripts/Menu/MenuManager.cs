using UnityEditor;
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

    [SerializeField]
    private MenuInfo pauseMenu;

    [SerializeField]
    private MenuInfo optionMenu;

    private MenuInfo actualMenu;

    private PlayerInputHandler actualMenuOwner;

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

    #region Pause/Option Menu
    public void OpenPauseMenu(PlayerInputHandler player)
    {
        if (actualMenuOwner != null) return;
        actualMenuOwner = player;
        OpenMenu(pauseMenu);
    }

    public void OpenOptionMenu(PlayerInputHandler player)
    {
        if (actualMenuOwner != null) return;
        actualMenuOwner = player;
        OpenMenu(optionMenu);
    }

    public void ClosePauseMenu()
    {
        if (actualMenu == pauseMenu)
        {
            CloseAllMenu(actualMenu);
            ClearMenuEntries();
        }
    }

    public void CloseOptionMenu()
    {
        if (actualMenu == optionMenu)
        {
            CloseAllMenu(actualMenu);
            ClearMenuEntries();
        }
    }
    #endregion

    public void OpenMenu(MenuInfo menu, TabInfo tabToOpen)
    {
        if(menu.HaveTabs)
            menu.GoToTab(tabToOpen);
        OpenMenu(menu);
    }

    public void OpenMenu(MenuInfo menu)
    {
        actualMenuOwner.SetPlayerActiveMenu(menu.MenuRoot, menu.FirstObjectSelected);
        menu.gameObject.SetActive(true);
        if (actualMenu != null)
        {
            menu.PreviousMenu = actualMenu;
        }
        actualMenu = menu;
    }

    public void CloseMenu()
    {
        actualMenu.gameObject.SetActive(false);
        if (actualMenu.HaveTabs)
            actualMenu.CloseAllTab();

        if (actualMenu.PreviousMenu != null)
        {
            actualMenu = actualMenu.PreviousMenu;
            actualMenuOwner.SetPlayerActiveMenu(actualMenu.MenuRoot, actualMenu.FirstObjectSelected);
            actualMenu.gameObject.SetActive(true);
        }
        else
        {
            ClearMenuEntries();
        }

    }

    private void ClearMenuEntries()
    {
        actualMenu = null;
        actualMenuOwner.SetPlayerActiveMenu(null, null);
        actualMenuOwner = null;
    }

    private void CloseAllMenu(MenuInfo menu)
    {
        if (menu.PreviousMenu != null)
        {
            CloseAllMenu(menu.PreviousMenu);
            menu.PreviousMenu = null;
        }
        menu.gameObject.SetActive(false);
    }

    public void GoNextTab(PlayerInputHandler playerInputHandler)
    {
        if (playerInputHandler == actualMenuOwner && actualMenu.HaveTabs)
        {
            actualMenu.GoNextTab();
            actualMenuOwner.SetPlayerActiveMenu(actualMenu.MenuRoot, actualMenu.FirstObjectSelected);
        }
            
    }
    public void GoPreviousTab(PlayerInputHandler playerInputHandler)
    {
        if (playerInputHandler == actualMenuOwner && actualMenu.HaveTabs)
        {
            actualMenu.GoPreviousTab();
            actualMenuOwner.SetPlayerActiveMenu(actualMenu.MenuRoot, actualMenu.FirstObjectSelected);
        }  
    }
}
