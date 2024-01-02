using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    [SerializeField] List<Fase> faseList;

    [Header("Movement")]
    [SerializeField] float faseLenght = 10;
    [SerializeField] UnityEvent OnMovementFaseStart;
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

    [Serializable]
    public class Fase
    {
        [SerializeField] public UnityEvent OnFaseStart;
        [SerializeField] public UnityEvent OnFaseEnd;
    }

    //[SerializeField] Fase[] fases = new Fase[Enum.GetValues(typeof(TutorialFase)).Length];

    //[SerializeField] public UnityEvent[] OnFaseStart = new UnityEvent[Enum.GetValues(typeof(TutorialFase)).Length];
    //[SerializeField] public UnityEvent[] OnFaseEnd = new UnityEvent[Enum.GetValues(typeof(TutorialFase)).Length];



    private void Start()
    {
        //stateMachine.RegisterState(TutorialFase.Intermediate, new IntermediateTutorialFase(this));
        //stateMachine.RegisterState(TutorialFase.Movement, new MovementTutorialState(this));
        //stateMachine.RegisterState(TutorialFase.Attack, new AttackTutorialState(this));
        //stateMachine.RegisterState(TutorialFase.Dodge, new DodgeTutorialState(this));
        //stateMachine.RegisterState(TutorialFase.Guard, new GuardTutorialState(this));
        //stateMachine.RegisterState(TutorialFase.Heal, new HealTutorialState(this));

        GameManager.Instance.coopManager.AddPlayer();

        //stateMachine.SetState(TutorialFase.Movement);


    }

    private void Update()
    {
        //stateMachine.StateUpdate();
    }

    
}
