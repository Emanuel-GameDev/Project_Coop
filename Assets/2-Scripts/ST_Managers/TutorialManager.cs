using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.Video;

public enum TutorialFaseType
{
    movement,
    attack,
    dodge,
    guard,
    heal,
    brutusAbility,
    kainaAbility,
    cassiusAbility,
    judeAbility
}


public class TutorialManager : MonoBehaviour
{
    public StateMachine<TutorialFase> stateMachine { get; } = new();

    [SerializeField] GameObject exit;

    [Header("Notification")]
    [SerializeField] public GameObject currentFaseObjective;
    [SerializeField] public TextMeshProUGUI objectiveText;
    [SerializeField] public GameObject objectiveNumbersGroup;
    [SerializeField] public TextMeshProUGUI objectiveNumberReached;
    [SerializeField] public TextMeshProUGUI objectiveNumberToReach;

    [Header("Intro")]
    [SerializeField] GameObject introScreen;
    [SerializeField] Slider skipSlider;
    public bool playIntro = false;
    VideoPlayer videoPlayer;

    [Header("Dialogues")]
    [SerializeField] public DialogueBox dialogueBox;
    [SerializeField] Dialogue postIntroDialogue;
    [SerializeField] Dialogue endingDialogueOne;
    [SerializeField] Dialogue endingDialogueTwo;

    [Header("Respawns")]
    [SerializeField] Transform DPSRespawn;
    [SerializeField] Transform healerRespawn;
    [SerializeField] Transform tankRespawn;
    [SerializeField] Transform rangedRespawn;

    [SerializeField] Transform postIntroEnemyRespawn;
    [SerializeField] Transform enemyRespawn;
    [SerializeField] Transform postIntroLilithRespawn;
    [SerializeField] Transform lilithRespawn;

    [Header("NPCs")]
    public TutorialEnemy tutorialEnemy;
    [SerializeField] GameObject lilith;
    [SerializeField] GameObject lilithBaloon;

    [SerializeField] MenuInfo startTutorialMenu;
    [SerializeField] MenuInfo continueTutorialMenu;

    public Image currentTutorialFaseImage;

    [Serializable]
    public class Fase
    {
        [SerializeField] public TutorialFaseData faseData;
    }

    [HideInInspector] public PlayerCharacter dps;
    [HideInInspector] public PlayerCharacter healer;
    [HideInInspector] public PlayerCharacter tank;
    [HideInInspector] public PlayerCharacter ranged;

    [HideInInspector] public List<PlayerCharacter> characters = new List<PlayerCharacter>();


    [HideInInspector] public bool blockFaseChange = false;
    bool finale = false;

    [HideInInspector] public Dictionary<PlayerCharacter, PlayerCharacterController> inputBindings;


    PlayableDirector playableDirector;

    [SerializeField] public Fase[] standardFases = new Fase[1];
    [HideInInspector] public int standardFaseCount = 0;

    [SerializeField] public Fase[] abilityFases = new Fase[1];
    [HideInInspector] public int abilityFaseCount = 0;

    [HideInInspector] public List<PlayerInputHandler> inputHandlers;
    private Dictionary<PlayerInputHandler, PlayerCharacter> startingCharacters;

    int _inputHandlerId = 0;

    int inputHandlersID
    {
        get { return _inputHandlerId; }

        set
        {
            if (value < inputHandlers.Count)
                _inputHandlerId = value;
            else
                _inputHandlerId = 0;
        }
    }

    private void Awake()
    {
        inputBindings = new Dictionary<PlayerCharacter, PlayerCharacterController>();
        playableDirector = gameObject.GetComponent<PlayableDirector>();
        videoPlayer = introScreen.GetComponent<VideoPlayer>();
    }



    private void Start()
    {
        exit.SetActive(true);
        objectiveText.enabled = false;
        objectiveNumbersGroup.SetActive(false);
        currentFaseObjective.SetActive(false);

        tutorialEnemy.gameObject.SetActive(false);
        lilith.gameObject.SetActive(true);


        skipSlider.gameObject.SetActive(false);

        foreach (PlayerInputHandler inputHandler in GameManager.Instance.CoopManager.GetComponentsInChildren<PlayerInputHandler>())
        {
            inputHandler.GetComponent<PlayerInput>().actions.Disable();
            inputHandler.GetComponent<PlayerInput>().actions.FindAction("SkipCutscene").Enable();
            inputHandler.GetComponent<PlayerInput>().actions.FindAction("SkipCutscene").performed += SkipIntro;
            inputHandler.GetComponent<PlayerInput>().actions.FindAction("SkipCutscene").started += EnableSkipSlider;
            inputHandler.GetComponent<PlayerInput>().actions.FindAction("SkipCutscene").canceled += DisableSkipSlider;
        }

        if (playIntro)
        {
            introScreen.SetActive(true);
            videoPlayer.Play();
            videoPlayer.loopPointReached += IntroEnded;
        }
        else
        {
            introScreen.SetActive(false);
            PostIntro();
        }


    }

    public void ChangeAndActivateCurrentCharacterImage(PlayerCharacter character)
    {
        currentTutorialFaseImage.sprite = GameManager.Instance.GetCharacterData(character.Character).DialogueSprite;
        currentTutorialFaseImage.gameObject.SetActive(true);
    }

    private void DisableSkipSlider(InputAction.CallbackContext context)
    {
        skipSlider.gameObject.SetActive(false);
        updateSlider = false;
    }

    private void EnableSkipSlider(InputAction.CallbackContext context)
    {
        skipSlider.gameObject.SetActive(true);
        updateSlider = true;
    }

    bool updateSlider = false;
    private void Update()
    {
        if (!videoPlayer.isPlaying)
            stateMachine.StateUpdate();

        if (updateSlider)
        {
            skipSlider.value += Time.deltaTime/3;
        }
        else
        {
            skipSlider.value = 0;
        }
    }

    private void IntroEnded(VideoPlayer source)
    {
        introScreen.SetActive(false);
        PostIntro();
        videoPlayer.loopPointReached -= IntroEnded;
    }

    private void SkipIntro(InputAction.CallbackContext obj)
    {
        introScreen.SetActive(false);
        PostIntro();

        foreach (PlayerInputHandler inputHandler in GameManager.Instance.CoopManager.GetComponentsInChildren<PlayerInputHandler>())
        {
            inputHandler.GetComponent<PlayerInput>().actions.FindAction("SkipCutscene").Disable();
            inputHandler.GetComponent<PlayerInput>().actions.FindAction("SkipCutscene").performed -= SkipIntro;
            inputHandler.GetComponent<PlayerInput>().actions.FindAction("SkipCutscene").started -= EnableSkipSlider;
            inputHandler.GetComponent<PlayerInput>().actions.FindAction("SkipCutscene").canceled -= DisableSkipSlider;
        }

    }

    [SerializeField] AudioSource musicaTutorial;
    private void PostIntro()
    {
        foreach (PlayerInputHandler inputHandler in GameManager.Instance.CoopManager.GetComponentsInChildren<PlayerInputHandler>())
        {
            inputHandler.GetComponent<PlayerInput>().actions.FindAction("SkipCutscene").Disable();
            inputHandler.GetComponent<PlayerInput>().actions.FindAction("SkipCutscene").performed -= SkipIntro;
            inputHandler.GetComponent<PlayerInput>().actions.FindAction("SkipCutscene").started -= EnableSkipSlider;
            inputHandler.GetComponent<PlayerInput>().actions.FindAction("SkipCutscene").canceled -= DisableSkipSlider;
        }

        musicaTutorial.Play();

        SetUpCharacters();
        //DeactivateAllPlayerInputs();

        ResetScene();
        DeactivateEnemyAI();

        foreach (PlayerCharacter character in characters)
        {
            //DA RIVEDERE #MODIFICATO
            character.SetCurrentHP(character.MaxHp - (character.MaxHp/2));
            HPHandler.Instance.UpdateContainer(character);
        }

        PlayDialogue(postIntroDialogue);
        dialogueBox.OnDialogueEnded += PostIntroDialogueEnd;

        tutorialEnemy.gameObject.transform.SetPositionAndRotation(postIntroEnemyRespawn.transform.position, Quaternion.identity);
        lilith.gameObject.transform.SetPositionAndRotation(postIntroLilithRespawn.transform.position, Quaternion.identity);

    }

    private void PostIntroDialogueEnd()
    {
        dialogueBox.OnDialogueEnded -= PostIntroDialogueEnd;

        tutorialEnemy.gameObject.SetActive(true);
        DeactivateEnemyAI();
    }

    public void StartTutorial()
    {
        stateMachine.SetState(new IntermediateTutorialFase(this));
        exit.SetActive(false);
        lilith.gameObject.GetComponent<CircleCollider2D>().enabled = false;
        
        lilithBaloon.SetActive(false);
        playableDirector.Play();


    }

    private void SetUpCharacters()
    {
        inputHandlersID = 0;
        inputHandlers = new List<PlayerInputHandler>();
        startingCharacters = new Dictionary<PlayerInputHandler, PlayerCharacter>();

        foreach (PlayerInputHandler ih in GameManager.Instance.CoopManager.GetComponentsInChildren<PlayerInputHandler>())
        {
            inputHandlers.Add(ih);
        }

        bool dpsPresent = false;
        bool healerPresent = false;
        bool tankPresent = false;
        bool rangedPresent = false;

        foreach(PlayerInputHandler ih in inputHandlers)
        {
            if(ih.currentCharacter == ePlayerCharacter.Brutus)
            {
                dpsPresent = true;
                PlayerCharacterController receiver = (PlayerCharacterController)ih.CurrentReceiver;
                dps = receiver.ActualPlayerCharacter;
                inputBindings.Add(dps, receiver);
                startingCharacters.Add(ih, dps);
            }

            if (ih.currentCharacter == ePlayerCharacter.Cassius)
            {
                healerPresent = true;
                PlayerCharacterController receiver = (PlayerCharacterController)ih.CurrentReceiver;
                healer = receiver.ActualPlayerCharacter;
                inputBindings.Add(healer, receiver);
                startingCharacters.Add(ih, healer);
            }

            if (ih.currentCharacter == ePlayerCharacter.Jude)
            {
                rangedPresent = true;
                PlayerCharacterController receiver = (PlayerCharacterController)ih.CurrentReceiver;
                ranged = receiver.ActualPlayerCharacter;
                inputBindings.Add(ranged, receiver);
                startingCharacters.Add(ih, ranged);
            }

            if (ih.currentCharacter == ePlayerCharacter.Kaina)
            {
                tankPresent = true;
                PlayerCharacterController receiver = (PlayerCharacterController)ih.CurrentReceiver;
                tank = receiver.ActualPlayerCharacter;
                inputBindings.Add(tank, receiver);
                startingCharacters.Add(ih, tank);
            }
        }

        if (!dpsPresent)
        {
            dps = PlayerCharacterPoolManager.Instance.GetCharacter(ePlayerCharacter.Brutus, transform);
            PlayerCharacterController receiver =(PlayerCharacterController) inputHandlers[inputHandlersID].CurrentReceiver;
            inputBindings.Add(dps, receiver);
            //HPHandler.Instance.AddContainer(dps);
            inputHandlersID++;
        }

        if (!healerPresent)
        {
            healer = PlayerCharacterPoolManager.Instance.GetCharacter(ePlayerCharacter.Cassius, transform);
            PlayerCharacterController receiver = (PlayerCharacterController)inputHandlers[inputHandlersID].CurrentReceiver;
            inputBindings.Add(healer, receiver);
            //HPHandler.Instance.AddContainer(healer);
            inputHandlersID++;
        }

        if (!rangedPresent)
        {
            ranged = PlayerCharacterPoolManager.Instance.GetCharacter(ePlayerCharacter.Jude, transform);
            PlayerCharacterController receiver = (PlayerCharacterController)inputHandlers[inputHandlersID].CurrentReceiver;
            inputBindings.Add(ranged, receiver);
            //HPHandler.Instance.AddContainer(ranged);
            inputHandlersID++;
        }

        if (!tankPresent)
        {
            tank = PlayerCharacterPoolManager.Instance.GetCharacter(ePlayerCharacter.Kaina, transform);
            PlayerCharacterController receiver = (PlayerCharacterController)inputHandlers[inputHandlersID].CurrentReceiver;
            inputBindings.Add(tank, receiver);
            //HPHandler.Instance.AddContainer(tank);
            inputHandlersID++;
        }

       

        characters.Add(dps);
        characters.Add(healer);
        characters.Add(ranged);
        characters.Add(tank);

    }


    [HideInInspector] public bool timerEnded = false;

    public IEnumerator Timer(float time)
    {
        yield return new WaitForSeconds(time);

        timerEnded = true;
        StopCoroutine(Timer(time));
    }



    public void PlayDialogue(Dialogue dialogueToPlay)
    {
        dialogueBox.SetDialogue(dialogueToPlay);
        dialogueBox.StartDialogue();
    }

    public void ResetScene()
    {
        if (!finale)
        {
            ResetPlayersPosition();
            ResetEnemyPosition();
        }
        else
        {
            tutorialEnemy.gameObject.SetActive(false);
            lilith.SetActive(false);
            PlayFinalePartTwo();
        }
    }
    public void ActivatePlayerInput(PlayerInputHandler inputHandler)
    {
        if (inputHandler == null)
            return;

        inputHandler.GetComponent<PlayerInput>().actions.Enable();
    }

    public void ActivateAllPlayerInput()
    {
        foreach (PlayerInputHandler ih in inputHandlers)
        {
            ActivatePlayerInput(ih);
        }
    }

    public void DeactivatePlayerInput(PlayerInputHandler inputHandler)
    {
        if (inputHandler == null)
            return;
        PlayerCharacterController receiver = (PlayerCharacterController)inputHandler.CurrentReceiver;
        PlayerCharacter player = receiver.ActualPlayerCharacter;
        inputHandler.GetComponent<PlayerInput>().actions.Disable();
        //inputHandler.GetComponent<PlayerInput>().actions.Disable();
        player.GetRigidBody().velocity = Vector3.zero;

    }

    public void DeactivateAllPlayerInputs()
    {
        foreach (PlayerInputHandler ih in inputHandlers)
        {
            DeactivatePlayerInput(ih);
        }

        dps.GetRigidBody().velocity = Vector2.zero;
        healer.GetRigidBody().velocity = Vector2.zero;
        ranged.GetRigidBody().velocity = Vector2.zero;
        tank.GetRigidBody().velocity = Vector2.zero;
    }


    public void PlayFinalePartOne()
    {
        blockFaseChange = true;
        finale = true;
        dialogueBox.OnDialogueEnded += Fade;

        currentFaseObjective.SetActive(false);
        objectiveNumbersGroup.SetActive(false);
        objectiveText.enabled = false;

        PlayDialogue(endingDialogueOne);
    }

    private void PlayFinalePartTwo()
    {
        dialogueBox.OnDialogueEnded -= Fade;

        ResetStartingCharacterAssosiacion();

        foreach (PlayerCharacter pc in PlayerCharacterPoolManager.Instance.AllPlayerCharacters)
        {
            if (!startingCharacters.ContainsValue(pc))
            {
                PlayerCharacterPoolManager.Instance.ReturnCharacter(pc);
                HPHandler.Instance.RemoveLastContainer(pc);
            }
        }


        dialogueBox.OnDialogueEnded += TutorialEnd;
        PlayDialogue(endingDialogueTwo);
    }

    public void ResetStartingCharacterAssosiacion()
    {
        foreach (PlayerInputHandler ih in inputHandlers)
        {
            // DA RIVEDERE #MODIFICATO
            PlayerCharacterController receiver = (PlayerCharacterController)ih.CurrentReceiver;
            receiver.SetPlayerCharacter(startingCharacters[ih]);
        }
    }
    [SerializeField] public PowerUp powerUpDebug;

    private void TutorialEnd()
    {
        dialogueBox.OnDialogueEnded -= TutorialEnd;

        //dps.RemovePowerUp(powerUpDebug);
        //healer.RemovePowerUp(powerUpDebug);
        //ranged.RemovePowerUp(powerUpDebug);
        //tank.RemovePowerUp(powerUpDebug);



        foreach (PlayerCharacter character in characters)
        {
            ActivatePlayerInput(character.GetInputHandler());
        }

        exit.SetActive(true);
    }
    public bool specialFase = false;
    public void SetSpecilaFases(bool set)
    {
        specialFase = set;
    }

    public void StartTutorialMenu()
    {
        MenuManager.Instance.FirstPlayerOpenMenu(startTutorialMenu);
    }


    public void StartNextFase()
    {
        if (blockFaseChange)
            return;

        if (!specialFase)
        {
            if (standardFaseCount >= standardFases.Length)
            {
                MenuManager.Instance.FirstPlayerOpenMenu(continueTutorialMenu);
                //standardFaseCount = 0;
                return;

            }

        }
        else
        {
            if (abilityFaseCount >= abilityFases.Length)
            {
                PlayFinalePartOne();
                return;
            }
        }

        if (!specialFase)
        {
            SwitchStandardFase();
        }
        else
        {
            SwitchSpecialFase();
        }
    }

    private void SwitchStandardFase()
    {
        switch (standardFases[standardFaseCount].faseData.faseType)
        {

            case TutorialFaseType.movement:
                stateMachine.SetState(new MovementTutorialState(this));
                break;

            case TutorialFaseType.attack:
                stateMachine.SetState(new AttackTutorialState(this));
                break;

            case TutorialFaseType.dodge:
                stateMachine.SetState(new DodgeTutorialState(this));
                break;

            case TutorialFaseType.guard:
                stateMachine.SetState(new GuardTutorialState(this));
                break;

            case TutorialFaseType.heal:
                stateMachine.SetState(new HealTutorialState(this));
                break;
        }
    }
    private void SwitchSpecialFase()
    {
        switch (abilityFases[abilityFaseCount].faseData.faseType)
        {
            case TutorialFaseType.brutusAbility:
                stateMachine.SetState(new BrutusAbilityTutorialFase(this));
                break;

            case TutorialFaseType.kainaAbility:
                stateMachine.SetState(new KainaAbilityTutorialFase(this));
                break;

            case TutorialFaseType.cassiusAbility:
                stateMachine.SetState(new CassiusAbilityTutorialFase(this));
                break;

            case TutorialFaseType.judeAbility:
                stateMachine.SetState(new JudeAbilityTutorialFase(this));
                break;
        }
    }

    public void Fade()
    {
        playableDirector.Play();
        DeactivateAllPlayerInputs();
    }

    public void EndCurrentFase()
    {
        if (!specialFase)
            standardFaseCount++;
        else
            abilityFaseCount++;

        Fade();

        dialogueBox.OnDialogueEnded -= EndCurrentFase;
    }

    private void ResetPlayersPosition()
    {

        dps.transform.position = DPSRespawn.position;
        healer.transform.position = healerRespawn.position;
        ranged.transform.position = rangedRespawn.position;
        tank.transform.position = tankRespawn.position;


        dps.SetSpriteDirection(new Vector2(1,-1));
        dps.ResetSpriteDirection();

        healer.SetSpriteDirection(new Vector2(1, -1));
        healer.ResetSpriteDirection();

        tank.SetSpriteDirection(new Vector2(1, -1));
        tank.ResetSpriteDirection();

        ranged.SetSpriteDirection(new Vector2(1, -1));
        ranged.ResetSpriteDirection();

        dps.GetRigidBody().velocity = Vector2.zero;
        healer.GetRigidBody().velocity = Vector2.zero;
        ranged.GetRigidBody().velocity = Vector2.zero;
        tank.GetRigidBody().velocity = Vector2.zero;

        

       
    }

    private void ResetEnemyPosition()
    {
        lilith.gameObject.transform.SetPositionAndRotation(lilithRespawn.transform.position, Quaternion.identity);

        tutorialEnemy.gameObject.SetActive(false);
        tutorialEnemy.gameObject.transform.SetPositionAndRotation(enemyRespawn.position, tutorialEnemy.gameObject.transform.rotation);
        
        tutorialEnemy.SetSpriteDirection(new Vector2(-1,-1));
        tutorialEnemy.ResetSpriteDirection();

        tutorialEnemy.viewTrigger.ClearList();
        tutorialEnemy.AttackRangeTrigger.ClearList();

        tutorialEnemy.stateMachine.SetState(tutorialEnemy.idleState);
        tutorialEnemy.GetRigidBody().velocity = Vector2.zero;
        

        tutorialEnemy.gameObject.SetActive(true);


        lilith.gameObject.transform.SetPositionAndRotation(lilithRespawn.position, lilith.gameObject.transform.rotation);
        lilith.gameObject.GetComponentInChildren<SpriteRenderer>().gameObject.SetActive(true);
    }

    public void DeactivateEnemyAI()
    {
        tutorialEnemy.viewTrigger.ClearList();
        tutorialEnemy.AttackRangeTrigger.ClearList();

        //tutorialEnemy.stateMachine.SetState(tutorialEnemy.idleState);
        tutorialEnemy.GetRigidBody().velocity = Vector2.zero;

        tutorialEnemy.AIActive = false;

    }

    public void ActivateEnemyAI()
    {
        tutorialEnemy.AIActive = true;
    }
}
