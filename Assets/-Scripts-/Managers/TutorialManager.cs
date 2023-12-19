using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TutorialFase
{
    Movement,
    Attack,
    Dodge,
    Guard,
    Heal
}

public class TutorialManager : MonoBehaviour
{
    public StateMachine<TutorialFase> stateMachine { get; } = new();

    


    [SerializeField] public UnityEvent[] OnFaseStart = new UnityEvent[Enum.GetValues(typeof(TutorialFase)).Length];
    [SerializeField] public UnityEvent[] OnFaseEnd = new UnityEvent[Enum.GetValues(typeof(TutorialFase)).Length];

    

    private void Start()
    {
        stateMachine.RegisterState(TutorialFase.Movement, new MovementTutorialState(this));
        stateMachine.RegisterState(TutorialFase.Attack, new AttackTutorialState(this));
        stateMachine.RegisterState(TutorialFase.Dodge, new DodgeTutorialState(this));
        stateMachine.RegisterState(TutorialFase.Guard, new GuardTutorialState(this));
        stateMachine.RegisterState(TutorialFase.Heal, new HealTutorialState(this));

        

        stateMachine.SetState(TutorialFase.Movement);
    }

    private void Update()
    {
        stateMachine.StateUpdate();
    }

    
}
