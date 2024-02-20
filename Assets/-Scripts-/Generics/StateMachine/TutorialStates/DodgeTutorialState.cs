using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DodgeTutorialState : TutorialFase
{
    TutorialManager tutorialManager;
    DodgeTutorialFaseData faseData;

    PlayerCharacter[] characters;
    Dialogue[] charactersPreTutorialDialogue;
    Dialogue[] charactersPerfectTutorialDialogue;
    int currentCharacterIndex;

    public DodgeTutorialState(TutorialManager tutorialManager)
    {
        this.tutorialManager = tutorialManager;
    }

    public override void Enter()
    {
        base.Enter();
        faseData = (DodgeTutorialFaseData)tutorialManager.fases[tutorialManager.faseCount].faseData;

        PubSub.Instance.RegisterFunction(EMessageType.dodgeExecuted, UpdateDodgeCounter);
        PubSub.Instance.RegisterFunction(EMessageType.perfectDodgeExecuted, UpdatePerfectDodgeCounter);

        tutorialManager.blockFaseChange = true;
        currentCharacterIndex = -1;

        characters = new PlayerCharacter[2] { tutorialManager.dps, tutorialManager.ranged };
        charactersPreTutorialDialogue = new Dialogue[2] { faseData.dpsDodgeDialogue, faseData.rangedDodgeDialogue };
        charactersPerfectTutorialDialogue = new Dialogue[2] { faseData.dpsPerfectDodgeDialogue, faseData.rangedPerfectDodgeDialogue };


        tutorialManager.DeactivateAllPlayerInputs();


        characterChange = true;

        tutorialManager.DeactivateEnemyAI();

        tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
        tutorialManager.PlayDialogue(faseData.faseStartDialogue);

        perfectDodgeCountAllowed = false;
    }
    bool characterChange = true;
    bool perfectDodgeCountAllowed = false;
    private void UpdatePerfectDodgeCounter(object obj)
    {
        if (!perfectDodgeCountAllowed)
            return;


        perfectDodgeCount++;

        Debug.Log("schiva perfect");

        if ( perfectDodgeCount == 3)
        {
            if (currentCharacterIndex < 1)
            {
                //sottofase successiva
                characterChange = true;
                tutorialManager.Fade();
                characters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Move").Disable();
                characters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Defense").Disable();
                SetupNextCharacter();
            }
            else
            {
                //fase successiva
                tutorialManager.blockFaseChange = false;
                tutorialManager.DeactivateEnemyAI();
                stateMachine.SetState(new IntermediateTutorialFase(tutorialManager));
            }

            perfectDodgeCountAllowed = false;
            //characters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Move").Disable();
            //characters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Defense").Disable();

            //da rimettere dopo

            //if (currentCharacterIndex < 2)
            //{
            //    //sottofase successiva
            //    tutorialManager.Fade();
            //    SetupNextCharacter();
            //}
            //else
            //{
            //    //fase successiva
            //    tutorialManager.blockFaseChange = false;
            //    stateMachine.SetState(new IntermediateTutorialFase(tutorialManager));
            //}

        }
    }

    private void UpdateDodgeCounter(object obj)
    {
        dodgeCount++;
        Debug.Log("schiva");
        if(dodgeCount == 3)
        {
            
            perfectDodgeCount = 0;

            tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;

            if (characters[currentCharacterIndex].CharacterClass is DPS)
                tutorialManager.PlayDialogue(faseData.dpsPerfectDodgeDialogue);
            else if(characters[currentCharacterIndex].CharacterClass is Ranged)
                tutorialManager.PlayDialogue(faseData.rangedPerfectDodgeDialogue);

            //PubSub.Instance.RegisterFunction(EMessageType.dodgeExecuted, UpdatePerfectDodgeCounter);
            //PubSub.Instance.UnregisterFunction(EMessageType.dodgeExecuted, UpdateDodgeCounter);
            characters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Move").Disable();
            characters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Defense").Disable();
            tutorialManager.DeactivateEnemyAI();

            perfectDodgeCountAllowed = true;
        }
    }

    public void WaitAfterDialogue()
    {
        tutorialManager.StartCoroutine(Wait(0.5f));
    }

    private void SetupNextCharacter()
    {
        characterChange = false;
        tutorialManager.dialogueBox.OnDialogueEnded -= WaitAfterDialogue;
        currentCharacterIndex++;
        tutorialManager.tutorialEnemy.SetTarget(characters[currentCharacterIndex].transform);
        //tutorialManager.DeactivateEnemyAI();
        dodgeCount = 0;
        perfectDodgeCount = 0;

        tutorialManager.dialogueBox.OnDialogueEnded += StartSubFase;
        tutorialManager.PlayDialogue(charactersPreTutorialDialogue[currentCharacterIndex]);

        //PubSub.Instance.UnregisterFunction(EMessageType.perfectDodgeExecuted, UpdatePerfectDodgeCounter);
    }

    private void SetupPerfectDodgeTutorial()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= WaitAfterDialogue;

        characters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        characters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Defense").Enable();


        tutorialManager.tutorialEnemy.SetTarget(characters[currentCharacterIndex].transform);
        tutorialManager.ActivateEnemyAI();

        perfectDodgeCount = 0;

        
    }

    int dodgeCount = 0;
    int perfectDodgeCount = 0;

    private void StartSubFase()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= StartSubFase;


        characters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        characters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Defense").Enable();

        tutorialManager.ActivateEnemyAI();


    }

   

    public IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);

        if (characterChange)
            SetupNextCharacter();
        else
            SetupPerfectDodgeTutorial();



    }

    public override void Update()
    {
        base.Update();


    }


    public override void Exit()
    {
        base.Exit();
        tutorialManager.EndCurrentFase();
        //PubSub.Instance.UnregisterFunction(EMessageType.dodgeExecuted, UpdatePerfectDodgeCounter);
    }
}
