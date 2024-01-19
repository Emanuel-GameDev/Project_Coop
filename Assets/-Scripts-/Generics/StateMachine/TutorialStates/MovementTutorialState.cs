using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MovementTutorialState : TutorialFase
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

        tutorialManager.DeactivatePlayerInput(tutorialManager.dps);
        tutorialManager.DeactivatePlayerInput(tutorialManager.healer);
        tutorialManager.DeactivatePlayerInput(tutorialManager.ranged);
        tutorialManager.DeactivatePlayerInput(tutorialManager.tank);

        tutorialManager.OnMovementFaseStart.Invoke();

        tutorialManager.dps.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        tutorialManager.healer.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        tutorialManager.ranged.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        tutorialManager.tank.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();

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
    //    Debug.Log("waiting");
    //    yield return new WaitForSeconds(10);
    //    tutorialManager.stateMachine.SetState(new AttackTutorialState(tutorialManager));
    //}

}
