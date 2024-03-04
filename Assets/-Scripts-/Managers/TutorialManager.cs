using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

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

    [SerializeField] public DialogueBox dialogueBox;

    [SerializeField] Dialogue endingDialogueOne;
    [SerializeField] Dialogue endingDialogueTwo;

    [SerializeField] Transform DPSRespawn;
    [SerializeField] Transform healerRespawn;
    [SerializeField] Transform tankRespawn;
    [SerializeField] Transform rangedRespawn;
    [SerializeField] Transform enemyRespawn;


    public PlayerCharacter dps;
    public PlayerCharacter healer;
    public PlayerCharacter tank;
    public PlayerCharacter ranged;

    public List<PlayerCharacter> characters = new List<PlayerCharacter>();

    public TutorialEnemy tutorialEnemy;
    [SerializeField] GameObject lilith;

    [HideInInspector] public bool blockFaseChange = false;
    bool finale = false;

    [HideInInspector] public Dictionary<PlayerCharacter, PlayerInputHandler> inputBindings;

    [Serializable]
    public class Fase
    {
        [SerializeField] public TutorialFaseData faseData;
    }

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
                dps=ih.CurrentReceiver.GetReceiverObject().GetComponent<PlayerCharacter>();
                inputBindings.Add(dps, ih);
                startingCharacters.Add(ih, dps);
            }

            if (ih.currentCharacter == ePlayerCharacter.Cassius)
            {
                healerPresent = true;
                healer = ih.CurrentReceiver.GetReceiverObject().GetComponent<PlayerCharacter>();
                inputBindings.Add(healer, ih);
                startingCharacters.Add(ih, healer);
            }

            if (ih.currentCharacter == ePlayerCharacter.Jude)
            {
                rangedPresent = true;
                ranged = ih.CurrentReceiver.GetReceiverObject().GetComponent<PlayerCharacter>();
                inputBindings.Add(ranged, ih);
                startingCharacters.Add(ih, ranged);
            }

            if (ih.currentCharacter == ePlayerCharacter.Caina)
            {
                tankPresent = true;
                tank = ih.CurrentReceiver.GetReceiverObject().GetComponent<PlayerCharacter>();
                inputBindings.Add(tank, ih);
                startingCharacters.Add(ih, tank);
            }
        }

        if (!dpsPresent)
        {
            InputReceiver receiver = SceneInputReceiverManager.Instance.GetSceneInputReceiver(null);
            receiver.SetCharacter(ePlayerCharacter.Brutus);
            dps=receiver.GetReceiverObject().GetComponent<PlayerCharacter>();

            inputBindings.Add(dps, inputHandlers[inputHandlersID]);
            inputHandlersID++;
        }

        if (!healerPresent)
        {
            InputReceiver receiver = SceneInputReceiverManager.Instance.GetSceneInputReceiver(null);
            receiver.SetCharacter(ePlayerCharacter.Cassius);
            healer = receiver.GetReceiverObject().GetComponent<PlayerCharacter>();

            inputBindings.Add(healer, inputHandlers[inputHandlersID]);
            inputHandlersID++;
        }

        if (!rangedPresent)
        {
            InputReceiver receiver = SceneInputReceiverManager.Instance.GetSceneInputReceiver(null);
            receiver.SetCharacter(ePlayerCharacter.Jude);
            ranged = receiver.GetReceiverObject().GetComponent<PlayerCharacter>();

            inputBindings.Add(ranged, inputHandlers[inputHandlersID]);
            inputHandlersID++;
        }

        if (!tankPresent)
        {
            InputReceiver receiver = SceneInputReceiverManager.Instance.GetSceneInputReceiver(null);
            receiver.SetCharacter(ePlayerCharacter.Caina);
            tank = receiver.GetReceiverObject().GetComponent<PlayerCharacter>();

            inputBindings.Add(tank, inputHandlers[inputHandlersID]);
            inputHandlersID++;
        }

       

        characters.Add(dps);
        characters.Add(healer);
        characters.Add(ranged);
        characters.Add(tank);

        //Debug.Log(inputBindings[dps].CurrentReceiver.GetCharacter());
        //Debug.Log(inputBindings[healer].CurrentReceiver.GetCharacter());
        //Debug.Log(inputBindings[ranged].CurrentReceiver.GetCharacter());
        //Debug.Log(inputBindings[tank].CurrentReceiver.GetCharacter());

    }

    

    public CharacterClass current;
    [SerializeField] private GameObject playerInputPrefab;

    private void Start()
    {
        inputBindings = new Dictionary<PlayerCharacter, PlayerInputHandler>();

        DeactivateEnemyAI();


        SetUpCharacters();


        playableDirector = gameObject.GetComponent<PlayableDirector>();

        stateMachine.SetState(new IntermediateTutorialFase(this));

        playableDirector.Play();


        //dps.GetComponent<PlayerInput>().actions.FindAction("Move").Disable();
        //OnMovementFaseStart.Invoke();

        //DeactivatePlayerInput(dps);
        //DeactivatePlayerInput(healer);
        //DeactivatePlayerInput(ranged);
        //DeactivatePlayerInput(tank);

        //DeactivateEnemyAI();

    }

    private void Update()
    {
        stateMachine.StateUpdate();
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

        PlayerCharacter player = inputHandler.CurrentReceiver.GetReceiverObject().GetComponent<PlayerCharacter>();
        inputHandler.GetComponent<PlayerInput>().actions.Disable();
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
        PlayDialogue(endingDialogueOne);
    }

    private void PlayFinalePartTwo()
    {
        dialogueBox.OnDialogueEnded -= Fade;

        foreach (PlayerInputHandler ih in inputHandlers)
        {
            ih.SetReceiver(startingCharacters[ih]);
        }

        dialogueBox.OnDialogueEnded += TutorialEnd;
        PlayDialogue(endingDialogueTwo);
    }



    private void TutorialEnd()
    {
        dialogueBox.OnDialogueEnded -= TutorialEnd;

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

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

        dps.GetRigidBody().MovePosition(DPSRespawn.position);
        healer.GetRigidBody().MovePosition(healerRespawn.position);
        ranged.GetRigidBody().MovePosition(rangedRespawn.position);
        tank.GetRigidBody().MovePosition(tankRespawn.position);

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
