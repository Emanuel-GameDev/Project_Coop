using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackTutorialState : TutorialFase
{
    TutorialManager tutorialManager;

    AttackTutorialFaseData faseData;


    PlayerCharacter[] characters;
    Dialogue[] charactersPreTutorialDialogue;
    int currentCharacterIndex;

    int hitCount = 0;

    public AttackTutorialState(TutorialManager tutorialManager)
    {
        this.tutorialManager = tutorialManager;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("attack");

        currentCharacterIndex = -1;

        characters = new PlayerCharacter[4] {tutorialManager.dps, tutorialManager.tank , tutorialManager.ranged , tutorialManager.healer };
        charactersPreTutorialDialogue = new Dialogue[4] { faseData.dpsDialogue, faseData.tankDialogue, faseData.rangedDialogue, faseData.healerDialogue };
            
        faseData = (AttackTutorialFaseData) tutorialManager.fases[tutorialManager.faseCount].faseData;

        for (int i = 0; i < 4; i++)
        {
            tutorialManager.DeactivatePlayerInput(characters[i]);
        }

        //PubSub.Instance.RegisterFunction(EMessageType.comboPerformed, AttackCount);

        //if (tutorialManager.current is DPS) 
        //{
        //    PubSub.Instance.RegisterFunction(EMessageType.dpsCombo, AttackCount);
        //}

        //if(tutorialManager.current is Healer)
        //{
        //    PubSub.Instance.RegisterFunction(EMessageType.healerCombo, AttackCount);
        //}

        //if (tutorialManager.current is Healer)
        //{
        //    PubSub.Instance.RegisterFunction(EMessageType.healerCombo, AttackCount);
        //}

        //if (tutorialManager.current is Healer)
        //{
        //    PubSub.Instance.RegisterFunction(EMessageType.healerCombo, AttackCount);
        //}

        tutorialManager.dialogueBox.OnDialogueEnded += StartNextCharacter;
        tutorialManager.PlayDialogue(faseData.faseStartDialogue);
    }

    private void StartNextCharacter()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= StartNextCharacter;
        currentCharacterIndex++;

        tutorialManager.dialogueBox.OnDialogueEnded += StartSubFase;
        tutorialManager.PlayDialogue(charactersPreTutorialDialogue[currentCharacterIndex]);


    }

    private void StartSubFase()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= StartSubFase;

        hitCount = 0;

        characters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        characters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Attack").Enable();


    }

    public override void Update()
    {
        base.Update();

        




    }


    public override void Exit()
    {
        base.Exit();
    }

    int i = 0;

    private void AttackCount(object obj)
    {
        i++;
        Debug.Log(i);
    }
}
