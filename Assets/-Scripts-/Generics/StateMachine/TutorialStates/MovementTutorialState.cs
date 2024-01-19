using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovementTutorialState : State
{
    TutorialManager tutorialManager;
    [SerializeField] Dialogue dialogo;
    [SerializeField] public UnityEvent OnFaseStart;
     public UnityEvent OnFaseEnd;

    public MovementTutorialState(TutorialManager tutorialManager)
    {
        this.tutorialManager = tutorialManager;
    }
    
    public override void Enter()
    {
        base.Enter();
        OnFaseStart.Invoke();

    }


    public override void Update()
    {
        base.Enter();

        tutorialManager.StartCoroutine(Count());
    }


    public override void Exit()
    {
        base.Enter();
        OnFaseEnd.Invoke();
    }

    IEnumerator Count()
    {
        Debug.Log("waiting");
        yield return new WaitForSeconds(10);
        tutorialManager.stateMachine.SetState(TutorialFase.Attack);
    }
}
