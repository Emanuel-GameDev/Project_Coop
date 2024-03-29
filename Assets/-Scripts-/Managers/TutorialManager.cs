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
using UnityEngine.Video;

public enum TutorialFaseType
{
    movement,
    attack,
    dodge,
    guard,
    heal
}


public class TutorialManager : MonoBehaviour
{
    public StateMachine<TutorialFase> stateMachine { get; } = new();

    [SerializeField] GameObject exit;

    [Header("Notification")]
    [SerializeField] public GameObject currentFaseObjective;
    [SerializeField] public TextMeshProUGUI objectiveText;
    [SerializeField] public GameObject objectiveNumbersGroup;
    [SerializeField] public TextMeshProUGUI objectiveNumberToReach;
    [SerializeField] public TextMeshProUGUI objectiveNumberReached;

    [Header("Intro")]
    [SerializeField] GameObject introScreen;
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
    [SerializeField] Transform enemyRespawn;

    [Header("NPCs")]
    public TutorialEnemy tutorialEnemy;
    [SerializeField] GameObject lilith;

   
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

    [SerializeField] public Fase[] fases = new Fase[1];


    public int faseCount = 0;

    [HideInInspector] public List<PlayerInputHandler> inputHandlers;
    private Dictionary<PlayerInputHandler, PlayerCharacter> startingCharacters;

    int _inputHandlerId = 0;

    int inputHandlersID
    {
        get { return _inputHandlerId; }

        set 
        { 
            if(value<inputHandlers.Count)
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
        exit.SetActive(false);
        objectiveText.enabled = false;
        objectiveNumbersGroup.SetActive(false);
        currentFaseObjective.SetActive(false);
        tutorialEnemy.gameObject.SetActive(false);

      

        foreach (PlayerInputHandler inputHandler in GameManager.Instance.coopManager.GetComponentsInChildren<PlayerInputHandler>())
        {
            inputHandler.GetComponent<PlayerInput>().actions.Disable();
            inputHandler.GetComponent<PlayerInput>().actions.FindAction("SkipCutscene").Enable();
            inputHandler.GetComponent<PlayerInput>().actions.FindAction("SkipCutscene").performed += SkipIntro;
        }

        if (playIntro)
        {
            introScreen.SetActive(true);
            videoPlayer.Play();
            videoPlayer.loopPointReached += IntroEnded;
        }
        else
        {
            introScreen.SetActive(false) ;
            PostIntro();
        }
        

    }

    private void Update()
    {
        if(!videoPlayer.isPlaying)
            stateMachine.StateUpdate();
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

        foreach (PlayerInputHandler inputHandler in GameManager.Instance.coopManager.GetComponentsInChildren<PlayerInputHandler>())
        {
            inputHandler.GetComponent<PlayerInput>().actions.FindAction("SkipCutscene").Disable();
            inputHandler.GetComponent<PlayerInput>().actions.FindAction("SkipCutscene").performed -= SkipIntro;
        }

    }

     
    private void PostIntro()
    {
        foreach (PlayerInputHandler inputHandler in GameManager.Instance.coopManager.GetComponentsInChildren<PlayerInputHandler>())
        {
            inputHandler.GetComponent<PlayerInput>().actions.FindAction("SkipCutscene").Disable();
            inputHandler.GetComponent<PlayerInput>().actions.FindAction("SkipCutscene").performed -= SkipIntro;
        }

        SetUpCharacters();
        //DeactivateAllPlayerInputs();

        ResetScene();
        DeactivateEnemyAI();

        foreach (PlayerCharacter character in characters)
        {
            //DA RIVEDERE #MODIFICATO
            character.SetCurrentHP(character.MaxHp - 5);
            //HPHandler.Instance.UpdateContainer(character);
        }

        PlayDialogue(postIntroDialogue);
        dialogueBox.OnDialogueEnded += StartTutorial;
        

    }

    private void StartTutorial()
    {
        dialogueBox.OnDialogueEnded -= StartTutorial;
        stateMachine.SetState(new IntermediateTutorialFase(this));

        tutorialEnemy.gameObject.SetActive(true);
        DeactivateEnemyAI();

        playableDirector.Play();
       
    }

    private void SetUpCharacters()
    {
        inputHandlersID = 0;
        inputHandlers = new List<PlayerInputHandler>();
        startingCharacters = new Dictionary<PlayerInputHandler, PlayerCharacter>();

        foreach (PlayerInputHandler ih in GameManager.Instance.coopManager.GetComponentsInChildren<PlayerInputHandler>())
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

            if (ih.currentCharacter == ePlayerCharacter.Caina)
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
            //PlayerCharacterController receiver =(PlayerCharacterController) SceneInputReceiverManager.Instance.GetSceneInputReceiver(null);
            //PlayerCharacterPoolManager.Instance.GetCharacter(ePlayerCharacter.Brutus, transform);
            //PlayerCharacterController receiver = (PlayerCharacterController)inputHandlers[inputHandlersID].CurrentReceiver;
            //Debug.Log(receiver.gameObject);
            //receiver.SetCharacter(ePlayerCharacter.Brutus);
            dps = PlayerCharacterPoolManager.Instance.GetCharacter(ePlayerCharacter.Brutus, transform);
            PlayerCharacterController receiver =(PlayerCharacterController) inputHandlers[inputHandlersID].CurrentReceiver;
            inputBindings.Add(dps, receiver);
            HPHandler.Instance.AddContainer(dps);
            inputHandlersID++;

        }

        if (!healerPresent)
        {
            //PlayerCharacterController receiver = (PlayerCharacterController)SceneInputReceiverManager.Instance.GetSceneInputReceiver(null);
            //PlayerCharacterController receiver = (PlayerCharacterController)inputHandlers[inputHandlersID].CurrentReceiver;
            //PlayerCharacterPoolManager.Instance.GetCharacter(ePlayerCharacter.Cassius, transform);
            //receiver.SetCharacter(ePlayerCharacter.Cassius);
            healer = PlayerCharacterPoolManager.Instance.GetCharacter(ePlayerCharacter.Cassius, transform);
            PlayerCharacterController receiver = (PlayerCharacterController)inputHandlers[inputHandlersID].CurrentReceiver;
            inputBindings.Add(healer, receiver);
            HPHandler.Instance.AddContainer(healer);
            inputHandlersID++;
        }

        if (!rangedPresent)
        {
            //PlayerCharacterController receiver = (PlayerCharacterController)SceneInputReceiverManager.Instance.GetSceneInputReceiver(null);
            //PlayerCharacterController receiver = (PlayerCharacterController)inputHandlers[inputHandlersID].CurrentReceiver;
            //receiver.SetCharacter(ePlayerCharacter.Jude);
            //
            //receiver � null
            //
            ranged = PlayerCharacterPoolManager.Instance.GetCharacter(ePlayerCharacter.Jude, transform);
            PlayerCharacterController receiver = (PlayerCharacterController)inputHandlers[inputHandlersID].CurrentReceiver;
            inputBindings.Add(ranged, receiver);
            HPHandler.Instance.AddContainer(ranged);
            inputHandlersID++;
        }

        if (!tankPresent)
        {
            //PlayerCharacterController receiver = (PlayerCharacterController)SceneInputReceiverManager.Instance.GetSceneInputReceiver(null);
            //PlayerCharacterController receiver = (PlayerCharacterController)inputHandlers[inputHandlersID].CurrentReceiver;
            //receiver.SetCharacter(ePlayerCharacter.Caina);
            tank = PlayerCharacterPoolManager.Instance.GetCharacter(ePlayerCharacter.Caina, transform);
            PlayerCharacterController receiver = (PlayerCharacterController)inputHandlers[inputHandlersID].CurrentReceiver;
            inputBindings.Add(tank, receiver);
            HPHandler.Instance.AddContainer(tank);
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


    private void PlayFinalePartOne()
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

        foreach (PlayerInputHandler ih in inputHandlers)
        {
            // DA RIVEDERE #MODIFICATO
            PlayerCharacterController receiver = (PlayerCharacterController)ih.CurrentReceiver;
            receiver.SetPlayerCharacter(startingCharacters[ih]);
        }

        dialogueBox.OnDialogueEnded += TutorialEnd;
        PlayDialogue(endingDialogueTwo);
    }



    private void TutorialEnd()
    {
        dialogueBox.OnDialogueEnded -= TutorialEnd;

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        exit.SetActive(true);
        foreach (PlayerCharacter character in characters)
        {
            ActivatePlayerInput(character.GetInputHandler());
        }
    }

    public void StartNextFase()
    {
        if (blockFaseChange)
            return;

        if (faseCount >= fases.Length)
        {
            //cambio scena o altro
            Debug.Log("Tutorial finito");

            PlayFinalePartOne();

            return;
        }

        switch (fases[faseCount].faseData.faseType)
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

    public void Fade()
    {
        playableDirector.Play();
        DeactivateAllPlayerInputs();
    }

    public void EndCurrentFase()
    {
        faseCount++;
        Fade();

        dialogueBox.OnDialogueEnded -= EndCurrentFase;
    }

    private void ResetPlayersPosition()
    {

        dps.transform.position = DPSRespawn.position;
        healer.transform.position = healerRespawn.position;
        ranged.transform.position = rangedRespawn.position;
        tank.transform.position = tankRespawn.position;

        //dps.GetRigidBody().MovePosition(DPSRespawn.position);
        //healer.GetRigidBody().MovePosition(healerRespawn.position);
        //ranged.GetRigidBody().MovePosition(rangedRespawn.position);
        //tank.GetRigidBody().MovePosition(tankRespawn.position);

        dps.GetRigidBody().velocity = Vector2.zero;
        healer.GetRigidBody().velocity = Vector2.zero;
        ranged.GetRigidBody().velocity = Vector2.zero;
        tank.GetRigidBody().velocity = Vector2.zero;

       
    }

    private void ResetEnemyPosition()
    {
       
        tutorialEnemy.gameObject.SetActive(false);
        tutorialEnemy.gameObject.transform.SetPositionAndRotation(enemyRespawn.position, tutorialEnemy.gameObject.transform.rotation);

        tutorialEnemy.viewTrigger.ClearList();
        tutorialEnemy.AttackRangeTrigger.ClearList();

        tutorialEnemy.stateMachine.SetState(tutorialEnemy.idleState);
        tutorialEnemy.GetRigidBody().velocity = Vector2.zero;
        

        tutorialEnemy.gameObject.SetActive(true);
    }

    public void DeactivateEnemyAI()
    {
        tutorialEnemy.viewTrigger.ClearList();
        //tutorialEnemy.closeRangeTrigger.ClearList();

        //tutorialEnemy.stateMachine.SetState(tutorialEnemy.idleState);
        tutorialEnemy.GetRigidBody().velocity = Vector2.zero;

        tutorialEnemy.AIActive = false;

    }

    public void ActivateEnemyAI()
    {
        tutorialEnemy.AIActive = true;
    }
}
