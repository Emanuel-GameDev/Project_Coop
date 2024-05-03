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
        PubSub.Instance.RegisterFunction(EMessageType.characterDamaged, CheckAndCount);

        faseData = (TutorialFaseData)tutorialManager.fases[tutorialManager.faseCount].faseData;

        tutorialManager.objectiveText.enabled = true;
        tutorialManager.objectiveText.text = faseData.faseObjective.GetLocalizedString();


        tutorialManager.DeactivateAllPlayerInputs();

        tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
        tutorialManager.PlayDialogue(faseData.faseStartDialogue);


        tutorialManager.DeactivateEnemyAI();
        
    }

    private void CheckAndCount(object obj)
    {
        if(obj is Projectile) 
        {
            Projectile projectile = (Projectile)obj;    
            if (projectile.projectileType == EProjectileType.empoweredProjectile)
                stateMachine.SetState(new IntermediateTutorialFase(tutorialManager));
        }
    }

    //private void HealAreaExpired(object obj)
    //{
    //    numberOfHealAreaExpired++;

    //    if (numberOfHealAreaExpired >= 1)
    //    {
    //        stateMachine.SetState(new IntermediateTutorialFase(tutorialManager));
    //    }
    //}

    private void WaitAfterDialogue()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= WaitAfterDialogue;

        tutorialManager.ResetStartingCharacterAssosiacion();

        tutorialManager.inputBindings[tutorialManager.ranged].SetPlayerCharacter(tutorialManager.ranged);

        tutorialManager.DeactivateAllPlayerInputs();


        foreach (PlayerInputHandler ih in CoopManager.Instance.GetActiveHandlers())
        {
            ih.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        }

        tutorialManager.inputBindings[tutorialManager.ranged].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("UniqueAbility").Enable();
        tutorialManager.inputBindings[tutorialManager.ranged].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("Look").Enable();
        tutorialManager.inputBindings[tutorialManager.ranged].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("LookMouse").Enable();
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

        PubSub.Instance.UnregisterFunction(EMessageType.characterDamaged, CheckAndCount);
    }
}