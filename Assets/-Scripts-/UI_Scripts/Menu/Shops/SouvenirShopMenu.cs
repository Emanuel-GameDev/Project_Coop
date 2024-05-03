using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SouvenirShopMenu : Menu
{
    public PlayerCharacter currentPlayerInShop;

    [SerializeField] public SouvenirShopTable[] shopTables = new SouvenirShopTable[4];

    [SerializeField] AudioClip openingAudioClip;

    //[Serializable]
    //public class SouvenirShopEntry
    //{
    //    [SerializeField] public SouvenirShopTable table;
    //    [SerializeField] public PowerUp[] souvenirs = new PowerUp[2];
    //    [HideInInspector] public int souvenirID;
    //}





    public override void OpenMenu()
    {
        if (shopGroup.activeSelf) return;

        base.OpenMenu();

        //if (interacter.GetInteracterObject().TryGetComponent<PlayerCharacter>(out PlayerCharacter playerInShop))
        //{
        //shopGroup.SetActive(true);
        //shopGroup.GetComponent<Animation>().Play("SouvenirEntrance");
        //canClose = true;
        //currentPlayerInShop = interacter.GetInteracterObject().GetComponent<PlayerCharacter>();

        //PlayerInputHandler inputHandler = currentPlayerInShop.GetInputHandler();
        //inputHandler.MultiplayerEventSystem.SetSelectedGameObject(firstSelected.GetComponentInChildren<Selectable>().gameObject);


        //InputActionAsset actions = inputHandler.GetComponent<PlayerInput>().actions;
        //actions.FindActionMap("UI").Enable();
        //actions.FindAction("Cancel").performed += Menu_performed;

        //foreach (PlayerInputHandler ih in CoopManager.Instance.GetComponentsInChildren<PlayerInputHandler>())
        //{
        //    InputActionAsset actionAsset = ih.GetComponent<PlayerInput>().actions;

        //    actionAsset.FindActionMap("UI").FindAction("Next").performed += SouvenirTableNext;

        //}

        foreach (PlayerCharacter pc in PlayerCharacterPoolManager.Instance.ActivePlayerCharacters)
        {
            gameObject.GetComponentInParent<PressInteractable>().CancelInteraction(pc);
        }

        shopGroup.GetComponent<Animation>().Play("SouvenirEntrance");

        foreach (SouvenirShopTable table in shopTables)
        {
            table.SetTableCurrentCharacter();
            table.StartIdleAnimationIn(Random.value);
            table.SetUpInput();
        }

        //CheckForMoney();
        AudioManager.Instance.PlayAudioClip(openingAudioClip, transform);
        //}
    }


    protected override void Menu_performed(InputAction.CallbackContext obj)
    {
        if (canClose)
        {
            CloseMenu();
        }
    }

    public void CheckForMoney()
    {
        foreach (SouvenirShopTable table in shopTables)
        {
            table.MoneyCheck();
        }
    }

    public override void CloseMenu()
    {
        //InputActionAsset actions = currentPlayerInShop.GetInputHandler().GetComponent<PlayerInput>().actions;
        //actions.FindAction("Cancel").performed -= Menu_performed;

        //foreach (PlayerInputHandler ih in CoopManager.Instance.GetComponentsInChildren<PlayerInputHandler>())
        //{
        //    ih.MultiplayerEventSystem.SetSelectedGameObject(null);
        //    InputActionAsset actionAsset = ih.GetComponent<PlayerInput>().actions;
        //    actionAsset.FindActionMap("Player").Enable();
        //    actionAsset.FindActionMap("UI").FindAction("Menu").Enable();
        //    actionAsset.FindActionMap("UI").Disable();
        //}
        //AudioManager.Instance.PlayAudioClip(openingAudioClip, transform);

        //currentPlayerInShop = null;

        foreach (PlayerInputHandler ih in CoopManager.Instance.GetComponentsInChildren<PlayerInputHandler>())
        {
            InputActionAsset actions = ih.GetComponent<PlayerInput>().actions;

            actions.FindActionMap("Player").Enable();

            actions.FindActionMap("UI").Disable();
            actions.FindActionMap("UI").FindAction("Menu").Enable();

            actions.FindAction("Cancel").performed -= Menu_performed;
        }

        foreach (SouvenirShopTable table in shopTables)
        {
            table.DesetInput();
        }

        shopGroup.GetComponent<Animation>().Play("SouvenirExit");
        StartCoroutine(CloseMenuWithDelay(shopGroup.GetComponent<Animation>().clip.length));
    }

    IEnumerator CloseMenuWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        shopGroup.SetActive(false);
    }
}
