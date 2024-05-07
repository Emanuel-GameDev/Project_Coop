using UnityEngine;
using UnityEngine.EventSystems;

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
    public MenuInfo ActualMenu => actualMenu;

    [SerializeField]
    private GameObject emptyRoot;
    [SerializeField]
    private GameObject emptyRootSelected;

    [SerializeField]
    private HPHandler hpHandler;


    private PlayerInputHandler actualMenuOwner;

    private GameObject lastSelectedObject;
    private IVisualizationChanger actualVisualizatorChanger;

    private delegate void GoBackHandler();
    private static event GoBackHandler OnGoBack;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            Inizialize();
        }
    }

    private void Inizialize()
    {
        if (pauseMenu != null)
        {
            pauseMenu.Inizialize();
            pauseMenu.gameObject.SetActive(false);
        }

        if (optionMenu != null)
        {
            optionMenu.Inizialize();
            optionMenu.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (EventSystem.current != null)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(lastSelectedObject);
            }
            else if (EventSystem.current.currentSelectedGameObject != lastSelectedObject)
            {
                lastSelectedObject = EventSystem.current.currentSelectedGameObject;
            }
        }
    }

    public void GoBack(PlayerInputHandler playerInputHandler)
    {
        if (playerInputHandler != actualMenuOwner) return;

        if (OnGoBack != null)
            OnGoBack?.Invoke();
        else if(actualMenu.GoBackButtonCanCloseMenu)
            CloseMenu();
    }


    #region Pause/Option Menu
    public void OpenPauseMenu(PlayerInputHandler player)
    {
        OpenMenu(pauseMenu, player, true);
        if (hpHandler != null)
            hpHandler.gameObject.SetActive(false);
    }

    public void OpenOptionMenu(PlayerInputHandler player)
    {
        if (optionMenu != null)
        {
            OpenMenu(optionMenu, player, true);
            if (hpHandler != null)
                hpHandler.gameObject.SetActive(false);
        }
        else
            OpenPauseMenu(player);
    }

    public void ClosePauseMenu(PlayerInputHandler player)
    {
        if (actualMenu == pauseMenu && player == actualMenuOwner)
            CloseAllMenu();
        else if (actualMenu.PauseButtonCanCloseMenu)
            CloseMenu();
    }

    public void CloseOptionMenu(PlayerInputHandler player)
    {
        if (actualMenu == optionMenu && player == actualMenuOwner)
            CloseAllMenu();
    }

    #endregion

    #region OpenMenu

    public void OpenMenu(MenuInfo menu)
    {
        OpenMenu(menu, null, null, false);
    }

    public void OpenMenu(MenuInfo menu, bool pauseGame)
    {
        OpenMenu(menu, null, null, pauseGame);
    }

    public void OpenMenu(MenuInfo menu, TabInfo tabToOpen)
    {
        OpenMenu(menu, tabToOpen, false);
    }

    public void OpenMenu(MenuInfo menu, TabInfo tabToOpen, bool pauseGame)
    {
        OpenMenu(menu, null, tabToOpen, pauseGame);
    }

    public void OpenMenu(MenuInfo menu, PlayerInputHandler player)
    {
        OpenMenu(menu, player, false);
    }

    public void OpenMenu(MenuInfo menu, PlayerInputHandler player, bool pauseGame)
    {
        OpenMenu(menu, player, null, pauseGame);
    }

    public void OpenMenu(MenuInfo menu, PlayerInputHandler player, TabInfo tabToOpen, bool pauseGame)
    {
        if (actualMenuOwner != null && player != null && actualMenuOwner != player)
            return;

        if (menu == null)
        {
            Debug.LogError("Menu is null!");
            return;
        }


        if (actualMenu != null)
        {
            if (actualMenu == menu)
                return;

            menu.PreviousMenu = actualMenu;
        }

        if (pauseGame)
            GameManager.Instance.PauseGame();

        if (player != null)
        {
            DisableOtherPlayerInteraction(player);
            actualMenuOwner = player;
        }


        if (menu.HaveTabs)
        {
            if (tabToOpen != null)
                menu.GoToTab(tabToOpen);
            else
                menu.GoDefaultTab();
        }

        if (menu.InteractableSetter != null)
            menu.InteractableSetter.EnableInteract();

        if (actualMenuOwner != null)
            actualMenuOwner.SetPlayerActiveMenu(menu.MenuRoot, menu.FirstObjectSelected);

        menu.gameObject.SetActive(true);

        actualMenu = menu;


    }

    private void DisableOtherPlayerInteraction(PlayerInputHandler player)
    {
        foreach (PlayerInputHandler handler in CoopManager.Instance.GetActiveHandlers())
        {
            if (player != null && handler != player)
            {
                handler.SetPlayerActiveMenu(emptyRoot, emptyRootSelected);
            }
        }

    }

    public void FirstPlayerOpenMenu(MenuInfo menu)
    {
        OpenMenu(menu, CoopManager.Instance.GetFirstPlayer());
    }

    #endregion

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
            if (actualMenu.InteractableSetter != null)
                actualMenu.InteractableSetter.EnableInteract();
        }
        else
        {
            ClearMenuEntries();
            GameManager.Instance.ResumeGame();
            if (hpHandler != null)
                hpHandler.gameObject.SetActive(true);
        }

    }

    private void ClearMenuEntries()
    {
        actualMenu = null;
        if (actualMenuOwner != null)
            actualMenuOwner.SetPlayerActiveMenu(null, null);
        actualMenuOwner = null;
    }

    private void CloseAllMenu()
    {
        CloseAllMenuRecursive(actualMenu);
        ClearMenuEntries();
        GameManager.Instance.ResumeGame();
        if (hpHandler != null)
            hpHandler.gameObject.SetActive(true);
    }

    private void CloseAllMenuRecursive(MenuInfo menu)
    {
        if (menu.PreviousMenu != null)
        {
            CloseAllMenuRecursive(menu.PreviousMenu);
            menu.PreviousMenu = null;
        }
        menu.gameObject.SetActive(false);
        if (menu.HaveTabs)
            menu.CloseAllTab();
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

    public void GoNextSubTab(PlayerInputHandler playerInputHandler)
    {
        if (playerInputHandler == actualMenuOwner && actualMenu.HaveSubTabs)
        {
            actualMenu.GoNextSubTab();
            actualMenuOwner.SetPlayerActiveMenu(actualMenu.MenuRoot, actualMenu.FirstObjectSelected);
        }
    }

    public void GoPreviousSubTab(PlayerInputHandler playerInputHandler)
    {
        if (playerInputHandler == actualMenuOwner && actualMenu.HaveSubTabs)
        {
            actualMenu.GoPreviousSubTab();
            actualMenuOwner.SetPlayerActiveMenu(actualMenu.MenuRoot, actualMenu.FirstObjectSelected);
        }
    }

    public void ChangeVisualization(PlayerInputHandler playerInputHandler)
    {
        if (playerInputHandler == actualMenuOwner && actualVisualizatorChanger != null)
            if (actualVisualizatorChanger is MonoBehaviour changer)
                if (changer.gameObject.activeInHierarchy)
                    actualVisualizatorChanger.ChangeVisualization();
    }

    public void SetActiveVisualizationChanger(IVisualizationChanger visualizatorChanger)
    {
        actualVisualizatorChanger = visualizatorChanger;
    }

}
