using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JudeAbilityTutorialFase : TutorialFase
{
    TutorialManager tutorialManager;
    TutorialFaseData faseData;

    public JudeAbilityTutorialFase(TutorialManager tutorialManager)
    {
        this.tutorialManager = tutorialManager;
    }

    public override void Enter()
    {
        base.Enter();
        tutorialManager.ResetStartingCharacterAssosiacion();
       

        faseData = (TutorialFaseData)tutorialManager.abilityFases[tutorialManager.abilityFaseCount].faseData;

        tutorialManager.objectiveText.enabled = true;
        tutorialManager.objectiveText.text = faseData.faseObjective.GetLocalizedString();
        tutorialManager.objectiveNumbersGroup.SetActive(true);
        tutorialManager.objectiveNumberToReach.text = "1";
        tutorialManager.objectiveNumberReached.text = "0";

        tutorialManager.ChangeAndActivateCurrentCharacterImage(tutorialManager.ranged);

        tutorialManager.DeactivateAllPlayerInputs();

        tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
        tutorialManager.PlayDialogue(faseData.faseStartDialogue);
        tutorialManager.ranged.AddPowerUp(tutorialManager.powerUpDebug);

        tutorialManager.DeactivateEnemyAI();
        
    }

    private void CheckAndCount(object obj)
    {
        if (obj is Projectile) 
        {
            Projectile projectile = (Projectile)obj;
            if (projectile.projectileType == EProjectileType.empoweredProjectile) 
            {
                tutorialManager.objectiveNumberReached.text = "1";
                stateMachine.SetState(new IntermediateTutorialFase(tutorialManager));
            }
        }
    }


    private void WaitAfterDialogue()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= WaitAfterDialogue;

        tutorialManager.ResetStartingCharacterAssosiacion();

        tutorialManager.inputBindings[tutorialManager.ranged].SetPlayerCharacter(tutorialManager.ranged);

        tutorialManager.DeactivateAllPlayerInputs();

        PubSub.Instance.RegisterFunction(EMessageType.characterDamaged, CheckAndCount);

        foreach (PlayerInputHandler ih in CoopManager.Instance.GetActiveHandlers())
        {
            ih.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        }
        tutorialManager.inputBindings[tutorialManager.ranged].GetInputHandler().GetComponent<PlayerInput>().actions.Enable();

        //tutorialManager.inputBindings[tutorialManager.ranged].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("UniqueAbility").Enable();
        //tutorialManager.inputBindings[tutorialManager.ranged].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("Look").Enable();
        //tutorialManager.inputBindings[tutorialManager.ranged].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("LookMouse").Enable();
    }



    public override void Update()
    {
        base.Update();

    }


    public override void Exit()
    {
        base.Exit();
        tutorialManager.ranged.RemovePowerUp(tutorialManager.powerUpDebug);

        tutorialManager.dialogueBox.OnDialogueEnded += tutorialManager.EndCurrentFase;
        tutorialManager.DeactivateAllPlayerInputs();
        tutorialManager.PlayDialogue(faseData.faseEndDialogue);

        PubSub.Instance.UnregisterFunction(EMessageType.characterDamaged, CheckAndCount);
    }
}
