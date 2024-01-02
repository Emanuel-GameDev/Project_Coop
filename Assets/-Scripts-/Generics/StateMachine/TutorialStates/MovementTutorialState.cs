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
        //tutorialManager.OnMovementFaseStart.Invoke();

    }
    bool check = false;

    public override void Update()
    {
        base.Update();
        //tutorialManager.StartCoroutine(Count());

        if (!check)
        {
            foreach (PlayerCharacter p in GameManager.Instance.coopManager.activePlayers)
            {
                if(p.MoveDirection!=Vector2.zero)
                {
                    Debug.Log(check);
                    check=true;
                }
            }

        }
    }


    public override void Exit()
    {
        base.Exit();
        //tutorialManager.OnMovementFaseEnd.Invoke();
    }

    //IEnumerator Count()
    //{
    //    //yield return new WaitForSeconds(tutorialManager.faseLenght);
    //    tutorialManager.stateMachine.SetState(TutorialFase.Attack);
    //}
}
