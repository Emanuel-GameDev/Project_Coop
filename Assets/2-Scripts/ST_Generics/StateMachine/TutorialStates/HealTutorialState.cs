using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HealTutorialState : TutorialFase
{
    TutorialManager tutorialManager;
    //List<PlayerCharacter> playerHealed;
    int numberOfPlayerHealed = 0;
    HealTutorialFaseData faseData;

    bool tankhealed = false;
    bool dpshealed = false;
    bool rangedhealed = false;
    bool enemyhealed = false;

    public HealTutorialState(TutorialManager tutorialManager)
    {
        this.tutorialManager = tutorialManager;
    }

    public override void Enter()
    {
        base.Enter();

        faseData = (HealTutorialFaseData)tutorialManager.standardFases[tutorialManager.standardFaseCount].faseData;

        tutorialManager.objectiveText.enabled = true;
        tutorialManager.objectiveText.text = faseData.faseObjective.GetLocalizedString();

        //playerHealed = new List<PlayerCharacter> { tutorialManager.dps, tutorialManager.ranged, tutorialManager.tank };

        DamageData damageData = new DamageData(1, null);

        tutorialManager.ChangeAndActivateCurrentCharacterImage(tutorialManager.healer, null, null);

       

        PubSub.Instance.RegisterFunction(EMessageType.characterHealed, CharacterHealed);

        tutorialManager.DeactivateAllPlayerInputs();

        tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
        tutorialManager.PlayDialogue(faseData.faseStartDialogue);

        tutorialManager.healer.GetComponent<Healer>().canHealEnemies = true;
        tutorialManager.healer.GetComponent<Healer>().smallHealTrigger.ClearList();

        numberOfPlayerHealed = 0;
        tutorialManager.objectiveNumberReached.text = numberOfPlayerHealed.ToString();
        tutorialManager.objectiveNumberToReach.text = "3";

        tutorialManager.ResetPlayerReminders(new PlayerCharacter[1] { tutorialManager.healer });

    }
    bool dialoguePlaying = false;
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

        //tutorialManager.inputBindings[tutorialManager.healer].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("Defense").Enable();
        tutorialManager.inputBindings[tutorialManager.healer].GetInputHandler().GetComponent<PlayerInput>().actions.Enable();

        dialoguePlaying = false;

       
    }

    private void CharacterHealed(object obj)
    {
        if (obj is PlayerCharacter)
        {
            PlayerCharacter character = (PlayerCharacter)obj;

            switch (character)
            {
                case DPS:

                    if (dpshealed || dialoguePlaying)
                        break;

                    tutorialManager.DeactivatePlayerInput(tutorialManager.healer.GetInputHandler());

                    numberOfPlayerHealed++;
                    tutorialManager.objectiveNumberReached.text = numberOfPlayerHealed.ToString();

                    tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
                    tutorialManager.PlayDialogue(faseData.DPSDialogue);
                    dialoguePlaying = true;
                    dpshealed = true;
                    break;

                case Ranged:

                    if (rangedhealed || dialoguePlaying)
                        break;


                    tutorialManager.DeactivatePlayerInput(tutorialManager.healer.GetInputHandler());

                    numberOfPlayerHealed++;
                    tutorialManager.objectiveNumberReached.text = numberOfPlayerHealed.ToString();

                    tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
                    tutorialManager.PlayDialogue(faseData.rangedDialogue);
                    dialoguePlaying = true;

                    rangedhealed = true;
                    break;

                case Tank:

                    if (tankhealed || dialoguePlaying)
                        break;

                    tutorialManager.DeactivatePlayerInput(tutorialManager.healer.GetInputHandler());

                    numberOfPlayerHealed++;
                    tutorialManager.objectiveNumberReached.text = numberOfPlayerHealed.ToString();

                    tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
                    tutorialManager.PlayDialogue(faseData.tankDialogue);
                    dialoguePlaying = true;

                    tankhealed = true;
                    break;

            }
        }
        else if(obj is TutorialEnemy)
        {
            if (enemyhealed || dialoguePlaying)
                return;

            TutorialEnemy character = (TutorialEnemy)obj;

            tutorialManager.DeactivatePlayerInput(tutorialManager.healer.GetInputHandler());
            tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
            tutorialManager.PlayDialogue(faseData.dumpyDialogue);
            dialoguePlaying = true;

            enemyhealed = true;
        }
    }


    public override void Update()
    {
        base.Update();
         
        if (dpshealed && tankhealed && rangedhealed && !dialoguePlaying)
        {
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

        tutorialManager.currentFaseObjective.gameObject.SetActive(false);

        tutorialManager.healer.GetComponent<Healer>().canHealEnemies = false;

        tutorialManager.dialogueBox.OnDialogueEnded += tutorialManager.EndCurrentFase;
        tutorialManager.DeactivateAllPlayerInputs();
        tutorialManager.PlayDialogue(faseData.faseEndDialogue);
    }
}
