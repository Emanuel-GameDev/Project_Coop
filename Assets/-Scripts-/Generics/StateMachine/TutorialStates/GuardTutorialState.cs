using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class GuardTutorialState : TutorialFase
{
    TutorialManager tutorialManager;

    GuardTutorialFaseData faseData;

    int guardExecuted = 0;
    int perfectGuardExecuted = 0;

    public GuardTutorialState(TutorialManager tutorialManager)
    {
        this.tutorialManager = tutorialManager;
    }

    public override void Enter()
    {
        base.Enter();

        guardExecuted = 0;
        perfectGuardExecuted = 0;

        faseData = (GuardTutorialFaseData) tutorialManager.fases[tutorialManager.faseCount].faseData;
        PubSub.Instance.RegisterFunction(EMessageType.guardExecuted, UpdateCounter);

        tutorialManager.DeactivatePlayerInput(tutorialManager.dps);
        tutorialManager.DeactivatePlayerInput(tutorialManager.healer);
        tutorialManager.DeactivatePlayerInput(tutorialManager.ranged);
        tutorialManager.DeactivatePlayerInput(tutorialManager.tank);

        tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
        tutorialManager.PlayDialogue(faseData.faseStartDialogue);

       
    }


    private void WaitAfterDialogue()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= WaitAfterDialogue;

        tutorialManager.tank.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        tutorialManager.tank.GetComponent<PlayerInput>().actions.FindAction("Defense").Enable();

        tutorialManager.ActivateEnemyAI();
    }

    private void UpdateCounter(object obj)
    {
        guardExecuted++;

        Debug.Log("conta");

        if(guardExecuted == 3)
        {
            //PubSub.Instance.UnregisterFunction(EMessageType.guardExecuted, UpdateCounter);
            tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
            tutorialManager.PlayDialogue(faseData.tankPerfectGuardDialogue);

            tutorialManager.tank.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
            tutorialManager.tank.GetComponent<PlayerInput>().actions.FindAction("Defense").Enable();

            tutorialManager.DeactivateEnemyAI();

            PubSub.Instance.RegisterFunction(EMessageType.perfectGuardExecuted, UpdatePerfectCounter);
            //PubSub.Instance.UnregisterFunction(EMessageType.guardExecuted, UpdateCounter);

        }
    }

    private void UpdatePerfectCounter(object obj)
    {
        perfectGuardExecuted++;

        if( perfectGuardExecuted >= 3)
        {
            tutorialManager.tank.GetComponent<PlayerInput>().actions.FindAction("Move").Disable();
            tutorialManager.tank.GetComponent<PlayerInput>().actions.FindAction("Defense").Disable();


            stateMachine.SetState(new IntermediateTutorialFase(tutorialManager));
        }
    }

    public override void Update()
    {
        base.Update();

    }


    public override void Exit()
    {
        base.Exit();
        //PubSub.Instance.UnregisterFunction(EMessageType.perfectGuardExecuted, UpdateCounter);
        tutorialManager.EndCurrentFase();
    }
}
