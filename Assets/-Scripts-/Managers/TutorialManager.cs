using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Playables;


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

    [SerializeField] Transform DPSRespawn;
    [SerializeField] Transform healerRespawn;
    [SerializeField] Transform tankRespawn;
    [SerializeField] Transform rangedRespawn;
    [SerializeField] Transform enemyRespawn;


    public PlayerCharacter dps;
    public PlayerCharacter healer;
    public PlayerCharacter tank;
    public PlayerCharacter ranged;

    public TutorialEnemy tutorialEnemy;
    

    [Serializable]
    public class Fase
    {
        [SerializeField] public TutorialFaseData faseData;
    }

    PlayableDirector playableDirector;

    [SerializeField] public Fase[] fases = new Fase[1];


    public int faseCount = 0;

    private void Awake()
    {
        SetUpCharacters();

        playableDirector = gameObject.GetComponent<PlayableDirector>();


    }

    private void SetUpCharacters()
    {
        PlayerCharacter searched = GameManager.Instance.coopManager.activePlayers.Find(c => c.CharacterClass is DPS);

        if (searched != null)
        {
            dps = searched;
        }

        searched = GameManager.Instance.coopManager.activePlayers.Find(c => c.CharacterClass is Healer);

        if (searched != null)
        {
            healer = searched;
        }

        searched = GameManager.Instance.coopManager.activePlayers.Find(c => c.CharacterClass is Ranged);

        if (searched != null)
        {
            ranged = searched;
        }

        searched = GameManager.Instance.coopManager.activePlayers.Find(c => c.CharacterClass is Tank);

        if (searched != null)
        {
            tank = searched;
        }
    }

    public CharacterClass current;

    private void Start()
    {

        stateMachine.SetState(new IntermediateTutorialFase(this));

        playableDirector.Play();

        current = healer.CharacterClass;
        //dps.GetComponent<PlayerInput>().actions.FindAction("Move").Disable();
        //OnMovementFaseStart.Invoke();

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
        ResetPlayersPosition();
        ResetEnemyPosition();
    }


    public void DeactivatePlayerInput(PlayerCharacter character)
    {
        character.GetComponent<PlayerInput>().actions.Disable();
    }

    public bool blockFaseChange = false;

    public void StartNextFase()
    {
        if (blockFaseChange)
            return;
        
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

    private void ResetPlayersPosition()
    {
        dps.gameObject.SetActive(false);
        healer.gameObject.SetActive(false);
        ranged.gameObject.SetActive(false);
        tank.gameObject.SetActive(false);

        dps.gameObject.transform.SetPositionAndRotation(DPSRespawn.position, dps.gameObject.transform.rotation);
        healer.gameObject.transform.SetPositionAndRotation(healerRespawn.position, healer.gameObject.transform.rotation);
        ranged.gameObject.transform.SetPositionAndRotation(rangedRespawn.position, ranged.gameObject.transform.rotation);
        tank.gameObject.transform.SetPositionAndRotation(tankRespawn.position, tank.gameObject.transform.rotation);


        dps.gameObject.SetActive(true);
        healer.gameObject.SetActive(true);
        ranged.gameObject.SetActive(true);
        tank.gameObject.SetActive(true);

        DeactivatePlayerInput(dps);
        DeactivatePlayerInput(healer);
        DeactivatePlayerInput(ranged);
        DeactivatePlayerInput(tank);
    }

    private void ResetEnemyPosition()
    {
        tutorialEnemy.gameObject.SetActive(false);
        tutorialEnemy.gameObject.transform.SetPositionAndRotation(enemyRespawn.position, tutorialEnemy.gameObject.transform.rotation);

        DeactivateEnemyAI();
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
