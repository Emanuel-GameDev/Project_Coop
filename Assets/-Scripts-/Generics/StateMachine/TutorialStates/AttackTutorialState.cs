using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

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

        faseData = (AttackTutorialFaseData) tutorialManager.fases[tutorialManager.faseCount].faseData;

        tutorialManager.blockFaseChange = true;

        currentCharacterIndex = -1;

        characters = new PlayerCharacter[4] {tutorialManager.dps, tutorialManager.tank , tutorialManager.ranged , tutorialManager.healer };
        charactersPreTutorialDialogue = new Dialogue[4] { faseData.dpsDialogue, faseData.tankDialogue, faseData.rangedDialogue, faseData.healerDialogue };


        tutorialManager.DeactivateAllPlayerInputs();

        tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
        tutorialManager.PlayDialogue(faseData.faseStartDialogue);
    }

    public void WaitAfterDialogue()
    {
        tutorialManager.StartCoroutine(Wait(0.5f));
    }

    private void SetupNextCharacter()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= WaitAfterDialogue;
        currentCharacterIndex++;
        

        tutorialManager.dialogueBox.OnDialogueEnded += StartSubFase;
        tutorialManager.PlayDialogue(charactersPreTutorialDialogue[currentCharacterIndex]);
    }

    private void StartSubFase()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= StartSubFase;

        hitCount = 0;

        if(characters[currentCharacterIndex].GetInputHandler()==null)
        {
            tutorialManager.inputHandlers[0].SetCharacter(characters[currentCharacterIndex].CharacterClass.Character);
        }

        characters[currentCharacterIndex].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        characters[currentCharacterIndex].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("Attack").Enable();

        comboHitCount = 0;
        tutorialManager.tutorialEnemy.OnHit += EnemyHitted;
        

    }

    Coroutine hitCounterTimer;

    private void EnemyHitted()
    {
        if(characters[currentCharacterIndex].CharacterClass is DPS)
        {
            if(hitCounterTimer != null)
                tutorialManager.StopCoroutine(hitCounterTimer);

            comboHitCount++;

            if(comboHitCount == 3)
            {
                comboHitCount = 0;
                hitCount++;
            }
            else
                hitCounterTimer = tutorialManager.StartCoroutine(ResetComboHitCounterTimer());
        }
        else
        {
            hitCount++;
        }

        Debug.Log(hitCount);
    }

    int comboHitCount = 0;

    public override void Update()
    {
        base.Update();

        if(hitCount >= 3)
        {
            hitCount = 0;
            tutorialManager.tutorialEnemy.OnHit -= EnemyHitted;

            characters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Move").Disable();
            characters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Attack").Disable();

            if (currentCharacterIndex < 3)
            {
                //sottofase successiva
                tutorialManager.Fade();
                SetupNextCharacter();
            }
            else
            {
                //fase successiva
                tutorialManager.blockFaseChange = false;
                stateMachine.SetState(new IntermediateTutorialFase(tutorialManager));
            }
        }


    }


    public override void Exit()
    {
        base.Exit();

        tutorialManager.dialogueBox.OnDialogueEnded += tutorialManager.EndCurrentFase;

        tutorialManager.PlayDialogue(faseData.faseEndDialogue);
    }

    public IEnumerator ResetComboHitCounterTimer()
    {
        yield return new WaitForSeconds(3);

        comboHitCount = 0;

        tutorialManager.StopCoroutine(hitCounterTimer);
    }

    public IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);

        SetupNextCharacter();
    }
}
