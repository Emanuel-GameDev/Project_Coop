using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] PowerUp powerUpToGive;
    [SerializeField] PlayerCharacter player;

    [SerializeField] GameObject playerManager;

    public PlayerInputManager manager { get; private set; }
    public CoopManager coopManager { get; private set; }

    public bool canJoin = false;
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        manager = playerManager.GetComponent<PlayerInputManager>();
        coopManager = playerManager.GetComponent<CoopManager>();
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.AddPowerUp(powerUpToGive);
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            player.UnlockUpgrade(AbilityUpgrade.Ability1);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            player.UnlockUpgrade(AbilityUpgrade.Ability2);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            player.UnlockUpgrade(AbilityUpgrade.Ability3);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            player.UnlockUpgrade(AbilityUpgrade.Ability4);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            player.UnlockUpgrade(AbilityUpgrade.Ability5);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            player.CharacterClass.SetIsInBossfight(true);
        }


        if (canJoin)
        {
           manager.joinAction.action.Enable();
        }
        else
        {
            manager.joinAction.action.Disable();
        }
    }
}
