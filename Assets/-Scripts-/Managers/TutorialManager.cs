using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

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

    [Serializable]
    public class Fase
    {
        [SerializeField] public TutorialFaseData faseData;
    }

    PlayableDirector playableDirector;

    [SerializeField] public Fase[] fases = new Fase[1];


    public int faseCount = 0;

    List<PlayerInputHandler> inputHandlers;
    List<CharacterClass> nonPlayingCharacter;
    private void SetUpCharacters()
    {
        inputHandlers = new List<PlayerInputHandler>();

        foreach(PlayerInputHandler ih in GameManager.Instance.coopManager.GetComponentsInChildren<PlayerInputHandler>())
        {
            inputHandlers.Add(ih);
        }

        


        //dps.SetInputHandler(GameManager.Instance.coopManager.GetComponentsInChildren<PlayerInputHandler>()[0]);
        
        //nonPlayingCharacter = CharacterPoolManager.Instance.GetComponentsInChildren<CharacterClass>().ToList();
        //GameManager.Instance.coopManager.GetComponentsInChildren<PlayerInputHandler>();


        PlayerCharacter searched = GameManager.Instance.coopManager.ActivePlayers.Find(c => c.CharacterClass is DPS);

        ////dps = SceneInputReceiverManager.Instance.
        ////CharacterPoolManager.Instance.SwitchCharacter(GameManager.Instance.coopManager.ActivePlayers[0], ePlayerCharacter.Brutus);
        if (searched != null)
        {
            dps = searched;
        }

        searched = GameManager.Instance.coopManager.ActivePlayers.Find(c => c.CharacterClass is Healer);

        if (searched != null)
        {
            healer = searched;
        }

        searched = GameManager.Instance.coopManager.ActivePlayers.Find(c => c.CharacterClass is Ranged);

        if (searched != null)
        {
            ranged = searched;
        }



        searched = GameManager.Instance.coopManager.ActivePlayers.Find(c => c.CharacterClass is Tank);

        if (searched != null)
        {
            tank = searched;
        }


        characters.Add(dps);
        characters.Add(healer);
        characters.Add(ranged);
        characters.Add(tank);


        dps.gameObject.SetActive(true);
        healer.gameObject.SetActive(true);
        ranged.gameObject.SetActive(true);
        tank.gameObject.SetActive(true);


    }

    private void SetInputs()
    {
        //Debug.Log("only 1");
        //da rivedere a input system finito

        int idInputs = 0;
        List<PlayerCharacter> players = GameManager.Instance.coopManager.ActivePlayers;



        InputUser dpsUser = dps.GetComponent<PlayerInput>().user;
        InputUser healerUser = healer.GetComponent<PlayerInput>().user;
        InputUser rangedUser = ranged.GetComponent<PlayerInput>().user;
        InputUser tankUser = tank.GetComponent<PlayerInput>().user;

        string dpsControl = "";
        string healerControl = "";
        string rangedControl = "";
        string tankControl = "";

        InputDevice[] dpsDevices = new InputDevice[0];
        InputDevice[] healerDevices = new InputDevice[0];
        InputDevice[] rangedDevices = new InputDevice[0];
        InputDevice[] tankDevices = new InputDevice[0];


        if (dps.GetComponent<PlayerInput>().devices.Count > 0)
        {
            dpsControl = dps.GetComponent<PlayerInput>().currentControlScheme;
            dpsDevices = dpsUser.pairedDevices.ToArray();
        }

        if (healer.GetComponent<PlayerInput>().devices.Count > 0)
        {
            healerControl = healer.GetComponent<PlayerInput>().currentControlScheme;
            healerDevices = healerUser.pairedDevices.ToArray();
        }

        if (ranged.GetComponent<PlayerInput>().devices.Count > 0)
        {
            rangedControl = ranged.GetComponent<PlayerInput>().currentControlScheme;
            rangedDevices = rangedUser.pairedDevices.ToArray();
        }

        if (tank.GetComponent<PlayerInput>().devices.Count > 0)
        {
            tankControl = tank.GetComponent<PlayerInput>().currentControlScheme;
            tankDevices = tankUser.pairedDevices.ToArray();
        }


        


        if (dpsDevices.Length <= 0)
        {
            dps.GetComponent<PlayerInput>().SwitchCurrentControlScheme(players[idInputs].GetComponent<PlayerInput>().currentControlScheme);

            foreach (InputDevice device in players[idInputs].GetComponent<PlayerInput>().devices)
                InputUser.PerformPairingWithDevice(device,
                    dps.GetComponent<PlayerInput>().user);

            if(idInputs < players.Count-1)
                idInputs++;
        }


        if (healerDevices.Length <= 0)
        {
            healer.GetComponent<PlayerInput>().SwitchCurrentControlScheme(players[idInputs].GetComponent<PlayerInput>().currentControlScheme);

            foreach (InputDevice device in players[idInputs].GetComponent<PlayerInput>().devices)
                InputUser.PerformPairingWithDevice(device,
                    healer.GetComponent<PlayerInput>().user);

            if (idInputs < players.Count - 1)
                idInputs++;
        }

        if (rangedDevices.Length <= 0)
        {
            ranged.GetComponent<PlayerInput>().SwitchCurrentControlScheme(players[idInputs].GetComponent<PlayerInput>().currentControlScheme);

            foreach (InputDevice device in players[idInputs].GetComponent<PlayerInput>().devices)
                InputUser.PerformPairingWithDevice(device,
                    ranged.GetComponent<PlayerInput>().user);
            if (idInputs < players.Count - 1)
                idInputs++;
        }

        if (tankDevices.Length <= 0)
        {
            tank.GetComponent<PlayerInput>().SwitchCurrentControlScheme(players[idInputs].GetComponent<PlayerInput>().currentControlScheme);

            foreach (InputDevice device in players[idInputs].GetComponent<PlayerInput>().devices)
                InputUser.PerformPairingWithDevice(device,
                    tank.GetComponent<PlayerInput>().user);
            if (idInputs < players.Count - 1)
                idInputs++;
        }


        //if (healerDevices.Length <= 0)
        //{
        //    healer.GetComponent<PlayerInput>().SwitchCurrentControlScheme(dpsControl);

        //    foreach (InputDevice device in dpsDevices)
        //        InputUser.PerformPairingWithDevice(device,
        //            healer.GetComponent<PlayerInput>().user);


        //    if (tankDevices.Length <= 0)
        //    {
        //        tank.GetComponent<PlayerInput>().SwitchCurrentControlScheme(dpsControl);


        //        foreach (InputDevice device in dpsDevices)
        //            InputUser.PerformPairingWithDevice(device,
        //                tank.GetComponent<PlayerInput>().user);
        //    }

        //}
        //else
        //{
        //    if (tankDevices.Length <= 0)
        //    {
        //        tank.GetComponent<PlayerInput>().SwitchCurrentControlScheme(healerControl);


        //        foreach (InputDevice device in healerDevices)
        //            InputUser.PerformPairingWithDevice(device,
        //                tank.GetComponent<PlayerInput>().user);
        //    }
        //}

        //if (rangedDevices.Length <= 0)
        //{
        //    ranged.GetComponent<PlayerInput>().SwitchCurrentControlScheme(dpsControl);

        //    foreach (InputDevice device in dpsDevices)
        //        InputUser.PerformPairingWithDevice(device,
        //            ranged.GetComponent<PlayerInput>().user);
        //}



        //tank.GetComponent<PlayerInput>().SwitchCurrentControlScheme(dpsControl);

        //foreach (InputDevice device in dpsDevices)
        //    InputUser.PerformPairingWithDevice(device,
        //        tank.GetComponent<PlayerInput>().user);

        //dpsUser.UnpairDevices();
    }

    public CharacterClass current;

    private void Start()
    {



        //SetUpCharacters();


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
    public void ActivatePlayerInput(PlayerCharacter character)
    {
        character.GetComponent<PlayerInput>().actions.Enable();
    }

    public void DeactivatePlayerInput(PlayerCharacter character)
    {
        character.GetComponent<PlayerInput>().actions.Disable();


        character.GetComponent<PlayerInput>().actions.FindAction("Dialogue").Enable();

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

        dialogueBox.OnDialogueEnded += TutorialEnd;
        PlayDialogue(endingDialogueTwo);
    }



    private void TutorialEnd()
    {
        dialogueBox.OnDialogueEnded -= TutorialEnd;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        foreach (PlayerCharacter character in characters)
        {
            ActivatePlayerInput(character);
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
    }

    public void EndCurrentFase()
    {
        faseCount++;
        Fade();

        dialogueBox.OnDialogueEnded -= EndCurrentFase;
    }

    bool setted = false;

    private void ResetPlayersPosition()
    {

        dps.GetRigidBody().MovePosition(DPSRespawn.position);
        healer.GetRigidBody().MovePosition(healerRespawn.position);
        ranged.GetRigidBody().MovePosition(rangedRespawn.position);
        tank.GetRigidBody().MovePosition(tankRespawn.position);


        //dps.gameObject.transform.SetPositionAndRotation(DPSRespawn.position, dps.gameObject.transform.rotation);
        //healer.gameObject.transform.SetPositionAndRotation(healerRespawn.position, healer.gameObject.transform.rotation);
        //ranged.gameObject.transform.SetPositionAndRotation(rangedRespawn.position, ranged.gameObject.transform.rotation);
        //tank.gameObject.transform.SetPositionAndRotation(tankRespawn.position, tank.gameObject.transform.rotation);


        //dps.gameObject.SetActive(true);
        //healer.gameObject.SetActive(true);
        //ranged.gameObject.SetActive(true);
        //tank.gameObject.SetActive(true);

        //DeactivatePlayerInput(dps);
        //DeactivatePlayerInput(healer);
        //DeactivatePlayerInput(ranged);
        //DeactivatePlayerInput(tank);

        if (faseCount < fases.Length)
        {
            if (!setted && fases[faseCount].faseData.faseType != TutorialFaseType.movement)
            {
                Debug.Log("SET");
                SetInputs();
                setted = true;
            }
        }
    }

    private void ResetEnemyPosition()
    {
        tutorialEnemy.gameObject.SetActive(false);
        tutorialEnemy.gameObject.transform.SetPositionAndRotation(enemyRespawn.position, tutorialEnemy.gameObject.transform.rotation);

        tutorialEnemy.viewTrigger.ClearList();
        tutorialEnemy.closeRangeTrigger.ClearList();

        tutorialEnemy.stateMachine.SetState(tutorialEnemy.idleState);
        tutorialEnemy.GetRigidBody().velocity = Vector2.zero;
        

        tutorialEnemy.gameObject.SetActive(true);
    }

    public void DeactivateEnemyAI()
    {
        //tutorialEnemy.enabled = false;
        tutorialEnemy.AIActive = false;

    }

    public void ActivateEnemyAI()
    {
        //tutorialEnemy.enabled = true;
        tutorialEnemy.AIActive = true;
    }
}
