using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BrutusAbilityTutorialFase : TutorialFase
{
    TutorialManager tutorialManager;
    TutorialFaseData faseData;

    int hitCounter = 0;
    bool canUpdateCounter = false;

    public BrutusAbilityTutorialFase(TutorialManager tutorialManager)
    {
        this.tutorialManager = tutorialManager;
    }

    public override void Enter()
    {
        base.Enter();

        PubSub.Instance.RegisterFunction(EMessageType.uniqueAbilityActivated, AllowUpdate);
        PubSub.Instance.RegisterFunction(EMessageType.characterHitted, UpdateCounter);
        PubSub.Instance.RegisterFunction(EMessageType.uniqueAbilityExpired, UnallowUpdate);

        faseData = (TutorialFaseData)tutorialManager.abilityFases[tutorialManager.abilityFaseCount].faseData;

        tutorialManager.objectiveText.enabled = true;
        tutorialManager.objectiveText.text = faseData.faseObjective.GetLocalizedString();
        tutorialManager.objectiveNumbersGroup.SetActive(true);

        tutorialManager.ChangeAndActivateCurrentCharacterImage(tutorialManager.dps, null, null);

        tutorialManager.ResetStartingCharacterAssosiacion();


        tutorialManager.DeactivateAllPlayerInputs();

        tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
        tutorialManager.PlayDialogue(faseData.faseStartDialogue);

        tutorialManager.dps.AddPowerUp(tutorialManager.powerUpDebug);

        tutorialManager.DeactivateEnemyAI();
        hitCounter = 0;
        tutorialManager.objectiveNumberToReach.text = "3";
        tutorialManager.objectiveNumberReached.text = hitCounter.ToString();
    }

    private void UnallowUpdate(object obj)
    {
        if (obj is PlayerCharacter)
        {
            PlayerCharacter character = (PlayerCharacter)obj;
            if (character.gameObject.GetComponent<DPS>() != null)
            {
                canUpdateCounter = false;
            }
        }
    }

    private void AllowUpdate(object obj)
    {
        if (obj is DPS)
        {
            PlayerCharacter character = (PlayerCharacter)obj;
            if (character.gameObject.GetComponent<DPS>() != null)
            {
                canUpdateCounter = true;
            }
        }
    }

    private void UpdateCounter(object obj)
    {
        if (!canUpdateCounter) return;

        if(obj is DPS)
        {
            PlayerCharacter character = (PlayerCharacter)obj;
            if (character.gameObject.GetComponent<DPS>() != null)
            {
                hitCounter++;
                tutorialManager.objectiveNumberReached.text = hitCounter.ToString();

                if (hitCounter >= 3)
                {
                    stateMachine.SetState(new IntermediateTutorialFase(tutorialManager));
                    tutorialManager.DeactivateEnemyAI();
                }
            }

        }
    }

    
    private void WaitAfterDialogue()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= WaitAfterDialogue;

        tutorialManager.inputBindings[tutorialManager.dps].SetPlayerCharacter(tutorialManager.dps);
        tutorialManager.DeactivateAllPlayerInputs();
        tutorialManager.tutorialEnemy.SetTarget(tutorialManager.dps.transform);

        foreach (PlayerInputHandler ih in CoopManager.Instance.GetActiveHandlers())
        {
            ih.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        }
        tutorialManager.inputBindings[tutorialManager.dps].GetInputHandler().GetComponent<PlayerInput>().actions.Enable();
        //tutorialManager.inputBindings[tutorialManager.dps].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("UniqueAbility").Enable();

        tutorialManager.tutorialEnemy.focus = false;
        tutorialManager.tutorialEnemy.SetTarget(tutorialManager.dps.transform);
        tutorialManager.tutorialEnemy.focus = true;

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
        tutorialManager.DeactivateEnemyAI();

        tutorialManager.dps.RemovePowerUp(tutorialManager.powerUpDebug);

        PubSub.Instance.UnregisterFunction(EMessageType.uniqueAbilityActivated, AllowUpdate);
        PubSub.Instance.UnregisterFunction(EMessageType.characterHitted, UpdateCounter);
        PubSub.Instance.UnregisterFunction(EMessageType.uniqueAbilityExpired, UnallowUpdate);
    }
}
