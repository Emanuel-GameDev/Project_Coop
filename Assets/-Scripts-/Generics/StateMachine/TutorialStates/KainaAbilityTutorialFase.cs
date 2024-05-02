using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KainaAbilityTutorialFase : TutorialFase
{
    TutorialManager tutorialManager;
    TutorialFaseData faseData;

    public KainaAbilityTutorialFase(TutorialManager tutorialManager)
    {
        this.tutorialManager = tutorialManager;
    }

    public override void Enter()
    {
        base.Enter();

        PubSub.Instance.RegisterFunction(EMessageType.uniqueAbilityActivated, StartEndFaseCountdown);

        faseData = (TutorialFaseData)tutorialManager.fases[tutorialManager.faseCount].faseData;

        tutorialManager.objectiveText.enabled = true;
        tutorialManager.objectiveText.text = faseData.faseObjective.GetLocalizedString();


        tutorialManager.DeactivateAllPlayerInputs();

        tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
        tutorialManager.PlayDialogue(faseData.faseStartDialogue);

        tutorialManager.tutorialEnemy.SetTarget(tutorialManager.healer.transform);
        tutorialManager.DeactivateEnemyAI();

    }

    private void StartEndFaseCountdown(object obj)
    {
        if(obj is Tank)
        {
            tutorialManager.StartCoroutine(WaitSeconds());
        }
    }

    IEnumerator WaitSeconds()
    {
        yield return new WaitForSecondsRealtime(5);
        stateMachine.SetState(new IntermediateTutorialFase(tutorialManager));
        tutorialManager.DeactivateEnemyAI();
    }
  

    private void WaitAfterDialogue()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= WaitAfterDialogue;

        tutorialManager.ResetStartingCharacterAssosiacion();

        tutorialManager.inputBindings[tutorialManager.tank].SetPlayerCharacter(tutorialManager.tank);

        tutorialManager.DeactivateAllPlayerInputs();


        foreach (PlayerInputHandler ih in CoopManager.Instance.GetActiveHandlers())
        {
            ih.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        }

        tutorialManager.inputBindings[tutorialManager.tank].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("UniqueAbility").Enable();
        tutorialManager.ActivateEnemyAI();
    }



    public override void Update()
    {
        base.Update();

    }


    public override void Exit()
    {
        base.Exit();

        tutorialManager.dialogueBox.OnDialogueEnded += tutorialManager.EndCurrentFase;
        tutorialManager.DeactivateAllPlayerInputs();
        tutorialManager.PlayDialogue(faseData.faseEndDialogue);
    }
}
