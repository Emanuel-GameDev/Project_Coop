using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTutorialState : State
{
    TutorialManager tutorialManager;

    public MovementTutorialState(TutorialManager tutorialManager)
    {
        this.tutorialManager = tutorialManager;
    }
    
    public override void Enter()
    {
        base.Enter();
        tutorialManager.OnFaseStart[0].Invoke();

    }


    public override void Update()
    {
        base.Enter();

        tutorialManager.StartCoroutine(Count());
    }


    public override void Exit()
    {
        base.Enter();
        tutorialManager.OnFaseEnd[0].Invoke();
    }

    IEnumerator Count()
    {
        Debug.Log("waiting");
        yield return new WaitForSeconds(10);
        tutorialManager.stateMachine.SetState(TutorialFase.Attack);
    }
}
