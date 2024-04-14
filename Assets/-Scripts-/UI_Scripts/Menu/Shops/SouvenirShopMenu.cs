using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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



    public void OpenMenu(IInteracter interacter)
    {
        if (interacter.GetInteracterObject().TryGetComponent<PlayerCharacter>(out PlayerCharacter playerInShop))
        {
            shopGroup.SetActive(true);
            canClose = true;
            currentPlayerInShop = interacter.GetInteracterObject().GetComponent<PlayerCharacter>();

            PlayerInputHandler inputHandler = currentPlayerInShop.GetInputHandler();
            inputHandler.MultiplayerEventSystem.SetSelectedGameObject(firstSelected.GetComponentInChildren<Selectable>().gameObject);

            foreach (PlayerInputHandler ih in CoopManager.Instance.GetComponentsInChildren<PlayerInputHandler>())
            {
                InputActionAsset actionAsset = ih.GetComponent<PlayerInput>().actions;
                actionAsset.FindActionMap("Player").Disable();
                actionAsset.FindActionMap("UI").Disable();
                actionAsset.FindActionMap("UI").FindAction("Menu").Disable();
            }

            InputActionAsset actions = inputHandler.GetComponent<PlayerInput>().actions;
            actions.FindActionMap("UI").Enable();
            actions.FindAction("Cancel").performed += Menu_performed;


            foreach (SouvenirShopTable table in shopTables)
            {
                table.SetTableCurrentCharacter(playerInShop);
            }

            AudioManager.Instance.PlayAudioClip(openingAudioClip,transform);
        }
    }

    public override void CloseMenu()
    {
        InputActionAsset actions = currentPlayerInShop.GetInputHandler().GetComponent<PlayerInput>().actions;
        actions.FindAction("Cancel").performed -= Menu_performed;

        foreach (PlayerInputHandler ih in CoopManager.Instance.GetComponentsInChildren<PlayerInputHandler>())
        {
            ih.MultiplayerEventSystem.SetSelectedGameObject(null);
            InputActionAsset actionAsset = ih.GetComponent<PlayerInput>().actions;
            actionAsset.FindActionMap("Player").Enable();
            actionAsset.FindActionMap("UI").FindAction("Menu").Enable();
            actionAsset.FindActionMap("UI").Disable();
        }
        AudioManager.Instance.PlayAudioClip(openingAudioClip, transform);

        currentPlayerInShop = null;
        shopGroup.SetActive(false);
    }

}
