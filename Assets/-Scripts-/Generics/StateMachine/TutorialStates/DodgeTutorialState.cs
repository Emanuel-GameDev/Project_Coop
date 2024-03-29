using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DodgeTutorialState : TutorialFase
{
    TutorialManager tutorialManager;
    DodgeTutorialFaseData faseData;

    PlayerCharacter[] currentFaseCharacters;
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

        tutorialManager.objectiveText.enabled = true;
        tutorialManager.objectiveText.text = faseData.faseObjective.GetLocalizedString();
        tutorialManager.objectiveNumbersGroup.SetActive(true);

        PubSub.Instance.RegisterFunction(EMessageType.dodgeExecuted, UpdateDodgeCounter);
        PubSub.Instance.RegisterFunction(EMessageType.perfectDodgeExecuted, UpdatePerfectDodgeCounter);

        tutorialManager.blockFaseChange = true;
        currentCharacterIndex = -1;

        currentFaseCharacters = new PlayerCharacter[2] { tutorialManager.dps, tutorialManager.ranged };
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
        tutorialManager.objectiveNumberToReach.text = perfectDodgeCount.ToString();


        if ( perfectDodgeCount == 3)
        {
            if (currentCharacterIndex < 1)
            {
                //sottofase successiva
                characterChange = true;
                tutorialManager.Fade();
                //currentFaseCharacters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Move").Disable();
                //currentFaseCharacters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Defense").Disable();
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
            

        }
    }

    private void UpdateDodgeCounter(object obj)
    {
        dodgeCount++;

        if (dodgeCount < 3)
        {
            tutorialManager.objectiveNumberToReach.text = dodgeCount.ToString();
        }

        if(dodgeCount == 3)
        {
            
            perfectDodgeCount = 0;

            tutorialManager.objectiveNumberToReach.text = perfectDodgeCount.ToString();

            tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
            tutorialManager.objectiveText.text = faseData.faseObjectivePerfect.GetLocalizedString();

            if (currentFaseCharacters[currentCharacterIndex] is DPS)
                tutorialManager.PlayDialogue(faseData.dpsPerfectDodgeDialogue);
            else if(currentFaseCharacters[currentCharacterIndex] is Ranged)
                tutorialManager.PlayDialogue(faseData.rangedPerfectDodgeDialogue);

            tutorialManager.inputBindings[currentFaseCharacters[currentCharacterIndex]].GetComponent<PlayerInput>().actions.FindAction("Move").Disable();
            tutorialManager.inputBindings[currentFaseCharacters[currentCharacterIndex]].GetComponent<PlayerInput>().actions.FindAction("Defense").Disable();

            tutorialManager.DeactivateEnemyAI();

            perfectDodgeCountAllowed = true;
        }
    }

    public void WaitAfterDialogue()
    {
        tutorialManager.DeactivateAllPlayerInputs();
        tutorialManager.StartCoroutine(Wait(0.1f));
    }

    private void SetupNextCharacter()
    {
        characterChange = false;
        tutorialManager.dialogueBox.OnDialogueEnded -= WaitAfterDialogue;
        currentCharacterIndex++;

        // DA RIVEDERE #MODIFICATO
        //tutorialManager.inputBindings[currentFaseCharacters[currentCharacterIndex]].SetReceiver(currentFaseCharacters[currentCharacterIndex]);

        tutorialManager.DeactivateAllPlayerInputs();
        tutorialManager.inputBindings[currentFaseCharacters[currentCharacterIndex]].GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        tutorialManager.inputBindings[currentFaseCharacters[currentCharacterIndex]].GetComponent<PlayerInput>().actions.FindAction("Defense").Enable();

        tutorialManager.objectiveText.text = faseData.faseObjective.GetLocalizedString();
        //tutorialManager.DeactivateEnemyAI();
        //tutorialManager.tutorialEnemy.focus = false;
        //tutorialManager.tutorialEnemy.SetTarget(currentFaseCharacters[currentCharacterIndex].transform);
        //tutorialManager.tutorialEnemy.focus = true;
        
        dodgeCount = 0;
        perfectDodgeCount = 0;

        tutorialManager.objectiveNumberToReach.text = dodgeCount.ToString();

        tutorialManager.dialogueBox.OnDialogueEnded += StartSubFase;
        tutorialManager.PlayDialogue(charactersPreTutorialDialogue[currentCharacterIndex]);

        //PubSub.Instance.UnregisterFunction(EMessageType.perfectDodgeExecuted, UpdatePerfectDodgeCounter);
    }

    private void SetupPerfectDodgeTutorial()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= WaitAfterDialogue;
        tutorialManager.DeactivateAllPlayerInputs();
        tutorialManager.inputBindings[currentFaseCharacters[currentCharacterIndex]].GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        tutorialManager.inputBindings[currentFaseCharacters[currentCharacterIndex]].GetComponent<PlayerInput>().actions.FindAction("Defense").Enable();


        tutorialManager.tutorialEnemy.focus = false;
        tutorialManager.tutorialEnemy.SetTarget(currentFaseCharacters[currentCharacterIndex].transform);
        tutorialManager.tutorialEnemy.focus = true;

        tutorialManager.ActivateEnemyAI();

        perfectDodgeCount = 0;

        
    }

    int dodgeCount = 0;
    int perfectDodgeCount = 0;

    private void StartSubFase()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= StartSubFase;

        tutorialManager.DeactivateAllPlayerInputs();
        tutorialManager.inputBindings[currentFaseCharacters[currentCharacterIndex]].GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        tutorialManager.inputBindings[currentFaseCharacters[currentCharacterIndex]].GetComponent<PlayerInput>().actions.FindAction("Defense").Enable();

        tutorialManager.ActivateEnemyAI();
        tutorialManager.tutorialEnemy.focus = false;
        tutorialManager.tutorialEnemy.SetTarget(currentFaseCharacters[currentCharacterIndex].transform);
        tutorialManager.tutorialEnemy.focus = true;

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
