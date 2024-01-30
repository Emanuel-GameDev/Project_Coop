using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;



public class MovementTutorialState : TutorialFase
{
    TutorialManager tutorialManager;

    MovementTutorialFaseData faseData;

    bool moveCheck = false;

    public MovementTutorialState(TutorialManager tutorialManager)
    {
        this.tutorialManager = tutorialManager;
    }
    
    public override void Enter()
    {
         base.Enter();

         faseData = (MovementTutorialFaseData) tutorialManager.fases[tutorialManager.faseCount].faseData;

        tutorialManager.DeactivatePlayerInput(tutorialManager.dps);
        tutorialManager.DeactivatePlayerInput(tutorialManager.healer);
        tutorialManager.DeactivatePlayerInput(tutorialManager.ranged);
        tutorialManager.DeactivatePlayerInput(tutorialManager.tank);

        tutorialManager.dialogueBox.OnDialogueEnded += StartFaseTimer;

        tutorialManager.PlayDialogue(tutorialManager.fases[tutorialManager.faseCount].faseData.faseStartDialogue);
    }

    

    private void StartFaseTimer()
    {
        tutorialManager.StartCoroutine(tutorialManager.Timer(faseData.faseLenght));

        tutorialManager.dps.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        tutorialManager.healer.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        tutorialManager.ranged.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        tutorialManager.tank.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();

        tutorialManager.dialogueBox.OnDialogueEnded -= StartFaseTimer;
    }


    public override void Update()
    {
        base.Update();

        

        if (!moveCheck)
        {
            foreach (PlayerCharacter p in GameManager.Instance.coopManager.activePlayers)
            {
                if(p.MoveDirection!=Vector2.zero)
                {
                    moveCheck=true;
                    Debug.Log(moveCheck);
                }
            }

        }

        if (tutorialManager.timerEnded)
        {
            tutorialManager.timerEnded = false;
            stateMachine.SetState(new IntermediateTutorialFase(tutorialManager));

            tutorialManager.DeactivatePlayerInput(tutorialManager.dps);
            tutorialManager.DeactivatePlayerInput(tutorialManager.healer);
            tutorialManager.DeactivatePlayerInput(tutorialManager.ranged);
            tutorialManager.DeactivatePlayerInput(tutorialManager.tank);
        }
    }


    public override void Exit()
    {
        base.Exit();

        tutorialManager.dialogueBox.OnDialogueEnded += tutorialManager.EndCurrentFase;

        if (!moveCheck)
            tutorialManager.PlayDialogue(faseData.specialFaseEndDialogue);
        else
            tutorialManager.PlayDialogue(tutorialManager.fases[tutorialManager.faseCount].faseData.faseEndDialogue);

    }
}
