using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CassiusAbilityTutorialFase : TutorialFase
{
    TutorialManager tutorialManager;
    TutorialFaseData faseData;

    int numberOfHealAreaExpired = 0;

    public CassiusAbilityTutorialFase(TutorialManager tutorialManager)
    {
        this.tutorialManager = tutorialManager;
    }

    public override void Enter()
    {
        base.Enter();
        tutorialManager.ResetStartingCharacterAssosiacion();
        PubSub.Instance.RegisterFunction(EMessageType.uniqueAbilityActivated, HealAreaActivated);
        PubSub.Instance.RegisterFunction(EMessageType.uniqueAbilityExpired, HealAreaExpired);

        faseData = (TutorialFaseData)tutorialManager.abilityFases[tutorialManager.abilityFaseCount].faseData;

        tutorialManager.objectiveText.enabled = true;
        tutorialManager.objectiveText.text = faseData.faseObjective.GetLocalizedString();
        tutorialManager.objectiveNumbersGroup.SetActive(true);
        tutorialManager.objectiveNumberToReach.text = "1";
        tutorialManager.objectiveNumberReached.text = "0";

        tutorialManager.DeactivateAllPlayerInputs();

        tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
        tutorialManager.PlayDialogue(faseData.faseStartDialogue);

        numberOfHealAreaExpired = 0;
    }

    private void HealAreaActivated(object obj)
    {
        if (obj is Healer)
        {
            tutorialManager.objectiveNumberReached.text = "1";
        }
    }

    private void HealAreaExpired(object obj)
    {
        if(obj is Healer)
        {
            numberOfHealAreaExpired++;

            if(numberOfHealAreaExpired >= 1)
            {
                stateMachine.SetState(new IntermediateTutorialFase(tutorialManager));
            }
        }

    }

    private void WaitAfterDialogue()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= WaitAfterDialogue;

        tutorialManager.ResetStartingCharacterAssosiacion();

        tutorialManager.inputBindings[tutorialManager.healer].SetPlayerCharacter(tutorialManager.healer);

        tutorialManager.DeactivateAllPlayerInputs();


        foreach (PlayerInputHandler ih in CoopManager.Instance.GetActiveHandlers())
        {
            ih.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        }

        tutorialManager.inputBindings[tutorialManager.healer].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("UniqueAbility").Enable();

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

        PubSub.Instance.UnregisterFunction(EMessageType.uniqueAbilityActivated, HealAreaActivated);
        PubSub.Instance.UnregisterFunction(EMessageType.uniqueAbilityExpired, HealAreaExpired);
    }
}
