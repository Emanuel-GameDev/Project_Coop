using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DodgeTutorialState : TutorialFase
{
    //le parti commentate potrebbero essere rimosse 

    TutorialManager tutorialManager;
    DodgeTutorialFaseData faseData;

    PlayerCharacter[] currentFaseCharacters;

    Dialogue[] charactersPreTutorialDialogue;
    //Dialogue[] charactersPerfectTutorialDialogue;

    int currentCharacterIndex;
    int dodgeCount = 0;

    public DodgeTutorialState(TutorialManager tutorialManager)
    {
        this.tutorialManager = tutorialManager;
    }

    public override void Enter()
    {
        base.Enter();
        faseData = (DodgeTutorialFaseData)tutorialManager.standardFases[tutorialManager.standardFaseCount].faseData;

        tutorialManager.objectiveText.enabled = true;
        tutorialManager.objectiveText.text = faseData.faseObjective.GetLocalizedString();
        tutorialManager.objectiveNumbersGroup.SetActive(true);
        tutorialManager.objectiveNumberToReach.text = faseData.timesToDodge.ToString();


        tutorialManager.blockFaseChange = true;
        currentCharacterIndex = -1;

        currentFaseCharacters = new PlayerCharacter[2] { tutorialManager.dps, tutorialManager.ranged };
        charactersPreTutorialDialogue = new Dialogue[2] { faseData.dpsDodgeDialogue, faseData.rangedDodgeDialogue };
        //charactersPerfectTutorialDialogue = new Dialogue[2] { faseData.dpsPerfectDodgeDialogue, faseData.rangedPerfectDodgeDialogue };


        tutorialManager.DeactivateAllPlayerInputs();


        //characterChange = true;

        tutorialManager.DeactivateEnemyAI();

        tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
        tutorialManager.PlayDialogue(faseData.faseStartDialogue);

        //perfectDodgeCountAllowed = false;
        PubSub.Instance.RegisterFunction(EMessageType.dodgeExecuted, UpdateDodgeCounter);
    }

    //bool characterChange = true;
    //bool perfectDodgeCountAllowed = false;

    //private void UpdatePerfectDodgeCounter(object obj)
    //{
    //    if (!perfectDodgeCountAllowed)
    //        return;


    //    perfectDodgeCount++;
    //    tutorialManager.objectiveNumberReached.text = perfectDodgeCount.ToString();


    //    if ( perfectDodgeCount == 3)
    //    {
    //        if (currentCharacterIndex < 1)
    //        {
    //            dodgeCount = 0;
    //            tutorialManager.objectiveNumberReached.text = dodgeCount.ToString();
    //            //sottofase successiva
    //            characterChange = true;
    //            tutorialManager.Fade();
    //            //currentFaseCharacters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Move").Disable();
    //            //currentFaseCharacters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Defense").Disable();
    //            SetupNextCharacter();
    //        }
    //        else
    //        {
    //            //fase successiva
    //            tutorialManager.blockFaseChange = false;
    //            tutorialManager.DeactivateEnemyAI();
    //            stateMachine.SetState(new IntermediateTutorialFase(tutorialManager));
    //        }
    //        PubSub.Instance.UnregisterFunction(EMessageType.perfectDodgeExecuted, UpdatePerfectDodgeCounter);
    //        perfectDodgeCountAllowed = false;
            

    //    }
    //}

    private void UpdateDodgeCounter(object obj)
    {
        dodgeCount++;

        if (dodgeCount < faseData.timesToDodge)
        {
            tutorialManager.objectiveNumberReached.text = dodgeCount.ToString();
        }

        if(dodgeCount == faseData.timesToDodge)
        {
            dodgeCount = 0;
            //perfectDodgeCount = 0;

            //tutorialManager.objectiveNumberReached.text = perfectDodgeCount.ToString();

            //tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
            //tutorialManager.objectiveText.text = faseData.faseObjectivePerfect.GetLocalizedString();

            //if (currentFaseCharacters[currentCharacterIndex] is DPS)
            //    tutorialManager.PlayDialogue(faseData.dpsPerfectDodgeDialogue);
            //else if(currentFaseCharacters[currentCharacterIndex] is Ranged)
            //    tutorialManager.PlayDialogue(faseData.rangedPerfectDodgeDialogue);

            foreach (PlayerInputHandler ih in CoopManager.Instance.GetActiveHandlers())
            {
                ih.GetComponent<PlayerInput>().actions.FindAction("Move").Disable();
            }

            tutorialManager.inputBindings[currentFaseCharacters[currentCharacterIndex]].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("Defense").Disable();
            //tutorialManager.inputBindings[currentFaseCharacters[currentCharacterIndex]].GetInputHandler().GetComponent<PlayerInput>().actions.Enable();

            tutorialManager.DeactivateEnemyAI();

            //perfectDodgeCountAllowed = true;


            if (currentCharacterIndex < currentFaseCharacters.Length - 1)
            {
                dodgeCount = 0;
                tutorialManager.objectiveNumberReached.text = dodgeCount.ToString();
                //sottofase successiva
                //characterChange = true;
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

            //PubSub.Instance.RegisterFunction(EMessageType.perfectDodgeExecuted, UpdatePerfectDodgeCounter);
        }
    }

    public void WaitAfterDialogue()
    {
        tutorialManager.DeactivateAllPlayerInputs();
        tutorialManager.StartCoroutine(Wait(0.1f));
    }

    private void SetupNextCharacter()
    {
        //characterChange = false;
        tutorialManager.dialogueBox.OnDialogueEnded -= WaitAfterDialogue;
        currentCharacterIndex++;


        tutorialManager.ResetStartingCharacterAssosiacion();

        tutorialManager.inputBindings[currentFaseCharacters[currentCharacterIndex]].SetPlayerCharacter(currentFaseCharacters[currentCharacterIndex]);
        tutorialManager.ChangeAndActivateCurrentCharacterImage(currentFaseCharacters[currentCharacterIndex]);
        tutorialManager.DeactivateAllPlayerInputs();

        foreach (PlayerInputHandler ih in CoopManager.Instance.GetActiveHandlers())
        {
            ih.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        }

        //tutorialManager.inputBindings[currentFaseCharacters[currentCharacterIndex]].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("Defense").Enable();
        tutorialManager.inputBindings[currentFaseCharacters[currentCharacterIndex]].GetInputHandler().GetComponent<PlayerInput>().actions.Enable();
        tutorialManager.objectiveText.text = faseData.faseObjective.GetLocalizedString();
        //tutorialManager.DeactivateEnemyAI();
        //tutorialManager.tutorialEnemy.focus = false;
        //tutorialManager.tutorialEnemy.SetTarget(currentFaseCharacters[currentCharacterIndex].transform);
        //tutorialManager.tutorialEnemy.focus = true;
        
        dodgeCount = 0;
        //perfectDodgeCount = 0;

        tutorialManager.objectiveNumberReached.text = dodgeCount.ToString();

        tutorialManager.dialogueBox.OnDialogueEnded += StartSubFase;
        tutorialManager.PlayDialogue(charactersPreTutorialDialogue[currentCharacterIndex]);

       
        //PubSub.Instance.UnregisterFunction(EMessageType.perfectDodgeExecuted, UpdatePerfectDodgeCounter);
    }

    //private void SetupPerfectDodgeTutorial()
    //{
    //    tutorialManager.dialogueBox.OnDialogueEnded -= WaitAfterDialogue;
    //    tutorialManager.DeactivateAllPlayerInputs();

    //    foreach (PlayerInputHandler ih in CoopManager.Instance.GetActiveHandlers())
    //    {
    //        ih.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
    //    }

    //    PubSub.Instance.UnregisterFunction(EMessageType.dodgeExecuted, UpdateDodgeCounter);

    //   // tutorialManager.inputBindings[currentFaseCharacters[currentCharacterIndex]].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("Defense").Enable();
    //    tutorialManager.inputBindings[currentFaseCharacters[currentCharacterIndex]].GetInputHandler().GetComponent<PlayerInput>().actions.Enable();


    //    tutorialManager.tutorialEnemy.focus = false;
    //    tutorialManager.tutorialEnemy.SetTarget(currentFaseCharacters[currentCharacterIndex].transform);
    //    tutorialManager.tutorialEnemy.focus = true;

    //    tutorialManager.ActivateEnemyAI();

    //    perfectDodgeCount = 0;

    //}

    //int perfectDodgeCount = 0;

    private void StartSubFase()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= StartSubFase;
        dodgeCount = 0;
        tutorialManager.objectiveNumberReached.text = dodgeCount.ToString();
        tutorialManager.DeactivateAllPlayerInputs();

        foreach (PlayerInputHandler ih in CoopManager.Instance.GetActiveHandlers())
        {
            ih.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        }

        tutorialManager.inputBindings[currentFaseCharacters[currentCharacterIndex]].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("Defense").Enable();

        tutorialManager.ActivateEnemyAI();
        tutorialManager.tutorialEnemy.focus = false;
        tutorialManager.tutorialEnemy.SetTarget(currentFaseCharacters[currentCharacterIndex].transform);
        tutorialManager.tutorialEnemy.focus = true;

    }

   

    public override void Update()
    {
        base.Update();
    }


    public override void Exit()
    {
        base.Exit();
        tutorialManager.EndCurrentFase();
        PubSub.Instance.UnregisterFunction(EMessageType.dodgeExecuted, UpdateDodgeCounter);
    }


    public IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);

        //if (characterChange)
            SetupNextCharacter();
        //else
        //    SetupPerfectDodgeTutorial();

    }
}
