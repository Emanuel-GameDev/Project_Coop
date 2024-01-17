using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] PowerUp powerUpToGive; //Debug
    [SerializeField] PlayerCharacter player; //Debug
    public PlayerInputManager playerInputManager { get; private set; }
    public CoopManager coopManager { get; private set; }
    public CameraManager cameraManager { get; private set; }

    public bool canJoin = false;
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
        playerInputManager = PlayerInputManager.instance;
        coopManager = CoopManager.Instance;
        cameraManager = CameraManager.Instance;

        if (playerInputManager != null)
            playerInputManager.JoinPlayer(0, -1, null);
        if (cameraManager != null && coopManager != null)
            cameraManager.AddAllPlayers();

    }

    private void Update()
    {
        //Debug start
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
        //Debug End

        if (canJoin)
        {
            playerInputManager.joinAction.action.Enable();
        }
        else
        {
            playerInputManager.joinAction.action.Disable();
        }
    }

    public void PlayerIsJoined(PlayerInput playerInput)
    {
        if(cameraManager != null)
            cameraManager.AddTarget(playerInput.transform);
    }

    public void PlayerAsLeft(PlayerInput playerInput)
    {
        if (cameraManager != null)
            cameraManager.RemoveTarget(playerInput.transform);
    }

    public void SetCanJoin(bool canJoin)
    {
        this.canJoin = canJoin;
    }

}
