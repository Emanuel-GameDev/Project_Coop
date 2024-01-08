using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public enum TutorialFase
{
    Intermediate,
    Movement,
    Attack,
    Dodge,
    Guard,
    Heal
}

public class TutorialManager : MonoBehaviour
{
    public StateMachine<TutorialFase> stateMachine { get; } = new();

    //[SerializeField] List<Fase> faseList;

    [SerializeField] Transform DPSRespawn;
    [SerializeField] Transform healerRespawn;
    [SerializeField] Transform tankRespawn;
    [SerializeField] Transform rangedRespawn;


    public PlayerCharacter dps;
    public PlayerCharacter healer;
    public PlayerCharacter tank;
    public PlayerCharacter ranged;

    [Header("Movement")]
    [SerializeField] float faseLenght = 10;
    public UnityEvent OnMovementFaseStart;
    [SerializeField] UnityEvent OnMovementFaseEnd;
    [SerializeField] UnityEvent OnMovementFaseEndSpecial;

    [Header("Attack")]
    [SerializeField] UnityEvent OnAttackFaseStart;
    [SerializeField] UnityEvent OnAttackFaseEnd;

    [Header("Dodge")]
    [SerializeField] UnityEvent OnDodgeFaseStart;
    [SerializeField] UnityEvent OnDodgeFaseEnd;

    [Header("Guard")]
    [SerializeField] UnityEvent OnGuardFaseStart;
    [SerializeField] UnityEvent OnGuardFaseEnd;

    [Header("Heal")]
    [SerializeField] UnityEvent OnHealFaseStart;
    [SerializeField] UnityEvent OnHealFaseEnd;

    //[Serializable]
    //public class Fase
    //{
    //    [SerializeField] public UnityEvent OnFaseStart;
    //    [SerializeField] public UnityEvent OnFaseEnd;
    //}

    PlayableDirector playableDirector;

    //[SerializeField] Fase[] fases = new Fase[Enum.GetValues(typeof(TutorialFase)).Length];

    //[SerializeField] public UnityEvent[] OnFaseStart = new UnityEvent[Enum.GetValues(typeof(TutorialFase)).Length];
    //[SerializeField] public UnityEvent[] OnFaseEnd = new UnityEvent[Enum.GetValues(typeof(TutorialFase)).Length];

    private void Awake()
    {
        playableDirector = gameObject.GetComponent<PlayableDirector>();
    }

    public CharacterClass current;

    private void Start()
    {
        stateMachine.RegisterState(TutorialFase.Intermediate, new IntermediateTutorialFase(this));

        stateMachine.RegisterState(TutorialFase.Movement, new MovementTutorialState(this));

        stateMachine.RegisterState(TutorialFase.Attack, new AttackTutorialState(this));

        stateMachine.RegisterState(TutorialFase.Dodge, new DodgeTutorialState(this));

        stateMachine.RegisterState(TutorialFase.Guard, new GuardTutorialState(this));

        stateMachine.RegisterState(TutorialFase.Heal, new HealTutorialState(this));

        ResetScene(null);

        current = healer.CharacterClass;
        //dps.GetComponent<PlayerInput>().actions.FindAction("Move").Disable();
        //OnMovementFaseStart.Invoke();

        stateMachine.SetState(TutorialFase.Movement);
        
    }

    private void Update()
    {
        stateMachine.StateUpdate();
    }

    public void StartTimerFase()
    {
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(faseLenght);
        OnMovementFaseEnd.Invoke();
    }

    public void ResetScene(object obj)
    {
        ResetPosition();
    }

    public void DeactivatePlayerInput(PlayerCharacter character)
    {
        character.GetComponent<PlayerInput>().actions.Disable();
    }

    private void ResetPosition()
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
    }
}
