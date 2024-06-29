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

        faseData = (MovementTutorialFaseData) tutorialManager.standardFases[tutorialManager.standardFaseCount].faseData;

        tutorialManager.objectiveText.enabled = true;
        tutorialManager.currentFaseObjective.SetActive(true);
        tutorialManager.objectiveText.text = faseData.faseObjective.GetLocalizedString();
        tutorialManager.DeactivateAllPlayerInputs();

        tutorialManager.objectiveNumberGroup.SetActive(false);
        tutorialManager.currentFaseObjective.gameObject.SetActive(false);

        tutorialManager.ResetStartingCharacterAssosiacion();
        tutorialManager.ResetPlayerReminders(new PlayerCharacter[4] {  tutorialManager.dps, tutorialManager.tank, tutorialManager.ranged, tutorialManager.healer } );

        tutorialManager.DeactivateEnemyAI();

        tutorialManager.dialogueBox.OnDialogueEnded += StartFaseTimer;
        tutorialManager.PlayDialogue(tutorialManager.standardFases[tutorialManager.standardFaseCount].faseData.faseStartDialogue);


       
    }

    

    private void StartFaseTimer()
    {
        tutorialManager.StartCoroutine(tutorialManager.Timer(faseData.faseLenght));

        tutorialManager.DeactivateAllPlayerInputs();

        foreach(PlayerInputHandler ih in tutorialManager.inputHandlers)
        {
            ih.GetComponent<PlayerInput>().actions.Enable();
        }

        tutorialManager.dialogueBox.OnDialogueEnded -= StartFaseTimer;

        tutorialManager.ChangeAndActivateCurrentCharacterImage(null, null, null);
    }


    public override void Update()
    {
        base.Update();

        
        if (!moveCheck)
        {
            foreach (PlayerCharacter p in tutorialManager.characters)
                if(p.MoveDirection != Vector2.zero)
                    moveCheck=true;
        }

        if (tutorialManager.timerEnded)
        {
            tutorialManager.timerEnded = false;
            stateMachine.SetState(new IntermediateTutorialFase(tutorialManager));
        }
    }


    public override void Exit()
    {
        base.Exit();

        tutorialManager.currentFaseObjective.gameObject.SetActive(false);

        tutorialManager.dialogueBox.OnDialogueEnded += tutorialManager.EndCurrentFase;

        if (!moveCheck)
            tutorialManager.PlayDialogue(faseData.specialFaseEndDialogue);
        else
            tutorialManager.PlayDialogue(tutorialManager.standardFases[tutorialManager.standardFaseCount].faseData.faseEndDialogue);

    }
}
