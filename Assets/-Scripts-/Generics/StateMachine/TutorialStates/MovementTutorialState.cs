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

        tutorialManager.objectiveText.enabled = true;
        tutorialManager.objectiveNumbersGroup.SetActive(false);
        tutorialManager.objectiveText.text = faseData.faseObjective.GetLocalizedString();

        tutorialManager.DeactivateAllPlayerInputs();

        tutorialManager.dialogueBox.OnDialogueEnded += StartFaseTimer;
        tutorialManager.DeactivateEnemyAI();

        tutorialManager.PlayDialogue(tutorialManager.fases[tutorialManager.faseCount].faseData.faseStartDialogue);
    }

    

    private void StartFaseTimer()
    {
        tutorialManager.StartCoroutine(tutorialManager.Timer(faseData.faseLenght));

        tutorialManager.DeactivateAllPlayerInputs();

        foreach(PlayerInputHandler ih in tutorialManager.inputHandlers)
        {
            ih.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        }

        tutorialManager.dialogueBox.OnDialogueEnded -= StartFaseTimer;
    }


    public override void Update()
    {
        base.Update();

        
        if (!moveCheck)
        {
            foreach (PlayerCharacter p in tutorialManager.characters)
            {
                if(p.MoveDirection != Vector2.zero)
                {
                    moveCheck=true;
                    Debug.Log(moveCheck);
                }
            }

        }

        if (tutorialManager.timerEnded)
        {
            tutorialManager.timerEnded = false;

            //tutorialManager.DeactivateAllPlayerInputs();
            stateMachine.SetState(new IntermediateTutorialFase(tutorialManager));
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
