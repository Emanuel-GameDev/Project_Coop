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
        tutorialManager.ResetStartingCharacterAssosiacion();
        PubSub.Instance.RegisterFunction(EMessageType.uniqueAbilityActivated, StartEndFaseCountdown);

        faseData = (TutorialFaseData)tutorialManager.abilityFases[tutorialManager.abilityFaseCount].faseData;

        tutorialManager.objectiveText.enabled = true;
        tutorialManager.objectiveText.text = faseData.faseObjective.GetLocalizedString();
        tutorialManager.objectiveNumbersGroup.SetActive(true);
        tutorialManager.objectiveNumberToReach.text = "1";
        tutorialManager.objectiveNumberReached.text = "0";

        tutorialManager.ChangeAndActivateCurrentCharacterImage(tutorialManager.tank);

        tutorialManager.DeactivateAllPlayerInputs();

        tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
        tutorialManager.PlayDialogue(faseData.faseStartDialogue);

        tutorialManager.tutorialEnemy.focus = false;
        tutorialManager.tutorialEnemy.SetTarget(tutorialManager.healer.transform);

        tutorialManager.DeactivateEnemyAI();

        tutorialManager.tank.AddPowerUp(tutorialManager.powerUpDebug);
    }


    private void WaitAfterDialogue()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= WaitAfterDialogue;

        tutorialManager.inputBindings[tutorialManager.tank].SetPlayerCharacter(tutorialManager.tank);

        tutorialManager.DeactivateAllPlayerInputs();


        foreach (PlayerInputHandler ih in CoopManager.Instance.GetActiveHandlers())
        {
            ih.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        }
        tutorialManager.inputBindings[tutorialManager.tank].GetInputHandler().GetComponent<PlayerInput>().actions.Enable();

        //tutorialManager.inputBindings[tutorialManager.tank].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("UniqueAbility").Enable();
        tutorialManager.ActivateEnemyAI();
    }

    private void StartEndFaseCountdown(object obj)
    {
        if(obj is Tank)
        {
            tutorialManager.objectiveNumberReached.text = "1";
            tutorialManager.StartCoroutine(WaitSeconds());
        }
    }


    public override void Update()
    {
        base.Update();

    }


    public override void Exit()
    {
        base.Exit();

        tutorialManager.tank.RemovePowerUp(tutorialManager.powerUpDebug);

        tutorialManager.dialogueBox.OnDialogueEnded += tutorialManager.EndCurrentFase;
        tutorialManager.DeactivateAllPlayerInputs();
        tutorialManager.PlayDialogue(faseData.faseEndDialogue);

        PubSub.Instance.UnregisterFunction(EMessageType.uniqueAbilityActivated, StartEndFaseCountdown);
    }
    IEnumerator WaitSeconds()
    {
        yield return new WaitForSecondsRealtime(5);

        stateMachine.SetState(new IntermediateTutorialFase(tutorialManager));
        tutorialManager.DeactivateEnemyAI();
    }
  

}
