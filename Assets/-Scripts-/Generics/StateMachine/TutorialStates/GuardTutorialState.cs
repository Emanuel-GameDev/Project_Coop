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

        tutorialManager.objectiveText.enabled = true;
        tutorialManager.objectiveText.text = faseData.faseObjective.GetLocalizedString();
        tutorialManager.objectiveNumbersGroup.SetActive(true);

        PubSub.Instance.RegisterFunction(EMessageType.guardExecuted, UpdateCounter);

        tutorialManager.DeactivateAllPlayerInputs();

        tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
        tutorialManager.PlayDialogue(faseData.faseStartDialogue);

        tutorialManager.tutorialEnemy.SetTarget(tutorialManager.tank.transform);
        tutorialManager.objectiveNumberToReach.text = guardExecuted.ToString();

    }


    private void WaitAfterDialogue()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= WaitAfterDialogue;
        // DA RIVEDERE #MODIFICATO
        tutorialManager.inputBindings[tutorialManager.tank].SetPlayerCharacter(tutorialManager.tank);
        tutorialManager.DeactivateAllPlayerInputs();

        foreach (PlayerInputHandler ih in CoopManager.Instance.GetActiveHandlers())
        {
            ih.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        }

        tutorialManager.inputBindings[tutorialManager.tank].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("Defense").Enable();

        tutorialManager.ActivateEnemyAI();

        tutorialManager.tutorialEnemy.focus = false;
        tutorialManager.tutorialEnemy.SetTarget(tutorialManager.tank.transform);
        tutorialManager.tutorialEnemy.focus = true;
    }

    private void UpdateCounter(object obj)
    {
            guardExecuted++;
        if (guardExecuted < 3)
        {
            tutorialManager.objectiveNumberToReach.text = guardExecuted.ToString();
        }

        if(guardExecuted == 3)
        {
            perfectGuardExecuted = 0;
            tutorialManager.objectiveNumberToReach.text = perfectGuardExecuted.ToString();

            tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
            tutorialManager.DeactivateAllPlayerInputs();
            tutorialManager.PlayDialogue(faseData.tankPerfectGuardDialogue);

            tutorialManager.objectiveText.text = faseData.faseObjectivePerfect.GetLocalizedString();

            foreach (PlayerInputHandler ih in CoopManager.Instance.GetActiveHandlers())
            {
                ih.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
            }

            tutorialManager.inputBindings[tutorialManager.tank].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("Defense").Disable();

            tutorialManager.DeactivateEnemyAI();

            PubSub.Instance.RegisterFunction(EMessageType.perfectGuardExecuted, UpdatePerfectCounter);


        }
    }

    private void UpdatePerfectCounter(object obj)
    {
        perfectGuardExecuted++;
        tutorialManager.objectiveNumberToReach.text = perfectGuardExecuted.ToString();

        if ( perfectGuardExecuted >= 3)
        {
            tutorialManager.DeactivateAllPlayerInputs();

            foreach (PlayerInputHandler ih in CoopManager.Instance.GetActiveHandlers())
            {
                ih.GetComponent<PlayerInput>().actions.FindAction("Move").Disable();
            }

            tutorialManager.inputBindings[tutorialManager.tank].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("Defense").Disable();
            tutorialManager.DeactivateEnemyAI();

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
