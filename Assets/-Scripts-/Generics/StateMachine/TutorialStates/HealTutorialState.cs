using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HealTutorialState : TutorialFase
{
    TutorialManager tutorialManager;
    List<PlayerCharacter> playerHealed;

    HealTutorialFaseData faseData;

    public HealTutorialState(TutorialManager tutorialManager)
    {
        this.tutorialManager = tutorialManager;
    }

    public override void Enter()
    {
        base.Enter();

        faseData = (HealTutorialFaseData)tutorialManager.fases[tutorialManager.faseCount].faseData;

        playerHealed = new List<PlayerCharacter> { tutorialManager.dps, tutorialManager.ranged, tutorialManager.tank };

        DamageData damageData = new DamageData(1, null);

        //tutorialManager.dps.CharacterClass.TakeDamage(damageData);
        //tutorialManager.ranged.CharacterClass.TakeDamage(damageData);
        //tutorialManager.tank.CharacterClass.TakeDamage(damageData);
        //tutorialManager.tutorialEnemy.TakeDamage(damageData);

        PubSub.Instance.RegisterFunction(EMessageType.characterHealed, CharacterHealed);

        tutorialManager.DeactivateAllPlayerInputs();

        tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
        tutorialManager.PlayDialogue(faseData.faseStartDialogue);

        foreach(PlayerCharacter character in playerHealed)
        {
            character.CharacterClass.currentHp = character.MaxHp - 5;
        }

    }

    private void WaitAfterDialogue()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= WaitAfterDialogue;

        tutorialManager.healer.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        tutorialManager.healer.GetComponent<PlayerInput>().actions.FindAction("Defense").Enable();
    }

    private void CharacterHealed(object obj)
    {
        if (obj is PlayerCharacter)
        {
            PlayerCharacter character = (PlayerCharacter)obj;

            switch (character.CharacterClass)
            {
                case DPS:
                    tutorialManager.DeactivatePlayerInput(tutorialManager.healer.GetInputHandler());

                    tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
                    tutorialManager.PlayDialogue(faseData.DPSDialogue);
                    break;

                case Ranged:
                    tutorialManager.DeactivatePlayerInput(tutorialManager.healer.GetInputHandler());

                    tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
                    tutorialManager.PlayDialogue(faseData.rangedDialogue);
                    break;

                case Tank:
                    tutorialManager.DeactivatePlayerInput(tutorialManager.healer.GetInputHandler());

                    tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
                    tutorialManager.PlayDialogue(faseData.tankDialogue);
                    break;
            }
        }
    }


    public override void Update()
    {
        base.Update();

        if (playerHealed.TrueForAll(p => p.CharacterClass.currentHp >= p.CharacterClass.MaxHp))
        {
            Debug.Log("fatto");
            stateMachine.SetState(new IntermediateTutorialFase(tutorialManager));
        }
        //else if(tutorialManager.tutorialEnemy.currentHp >= tutorialManager.tutorialEnemy.MaxHp)
        //{
        //    Debug.Log("enemy");
        //}

    }


    public override void Exit()
    {
        base.Exit();

        tutorialManager.dialogueBox.OnDialogueEnded += tutorialManager.EndCurrentFase;

        tutorialManager.PlayDialogue(faseData.faseEndDialogue);
    }
}
