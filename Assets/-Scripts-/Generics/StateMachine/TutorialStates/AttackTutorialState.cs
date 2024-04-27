using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackTutorialState : TutorialFase
{
    TutorialManager tutorialManager;

    AttackTutorialFaseData faseData;


    PlayerCharacter[] currentFaseCharacters;
    Dialogue[] charactersPreTutorialDialogue;
    int currentCharacterIndex;

    int hitCount = 0;

    int inputHandlerIndex = 0;

    public AttackTutorialState(TutorialManager tutorialManager)
    {
        this.tutorialManager = tutorialManager;
    }

    public override void Enter()
    {
        base.Enter();

        faseData = (AttackTutorialFaseData)tutorialManager.fases[tutorialManager.faseCount].faseData;


        tutorialManager.objectiveText.enabled = true;
        tutorialManager.objectiveText.text = faseData.faseObjectiveBrutus.GetLocalizedString();
        tutorialManager.objectiveNumbersGroup.SetActive(true);


        tutorialManager.blockFaseChange = true;

        currentCharacterIndex = -1;

        currentFaseCharacters = new PlayerCharacter[4] { tutorialManager.dps, tutorialManager.tank, tutorialManager.ranged, tutorialManager.healer };
        charactersPreTutorialDialogue = new Dialogue[4] { faseData.dpsDialogue, faseData.tankDialogue, faseData.rangedDialogue, faseData.healerDialogue };


        tutorialManager.DeactivateAllPlayerInputs();

        tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
        tutorialManager.PlayDialogue(faseData.faseStartDialogue);
    }


    public void WaitAfterDialogue()
    {
        tutorialManager.DeactivateAllPlayerInputs();
        tutorialManager.StartCoroutine(Wait(0.1f));
    }

    private void SetupNextCharacter()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= WaitAfterDialogue;
        tutorialManager.DeactivateAllPlayerInputs();

        currentCharacterIndex++;
        // DA RIVEDERE #MODIFICATO
        tutorialManager.inputBindings[currentFaseCharacters[currentCharacterIndex]].SetPlayerCharacter(currentFaseCharacters[currentCharacterIndex]);

        tutorialManager.dialogueBox.OnDialogueEnded += StartSubFase;
        tutorialManager.PlayDialogue(charactersPreTutorialDialogue[currentCharacterIndex]);

        if (currentFaseCharacters[currentCharacterIndex] is DPS)
            return;

        tutorialManager.objectiveText.text = faseData.faseObjective.GetLocalizedString();
    }

    private void StartSubFase()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= StartSubFase;

        hitCount = 0;
        comboHitCount = 0;

        tutorialManager.objectiveNumberToReach.text = hitCount.ToString();

        tutorialManager.DeactivateAllPlayerInputs();
        foreach (PlayerInputHandler ih in CoopManager.Instance.GetActiveHandlers())
        {
            ih.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        }
        tutorialManager.inputBindings[currentFaseCharacters[currentCharacterIndex]].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("Attack").Enable();


        tutorialManager.tutorialEnemy.OnHitAction += EnemyHitted;


    }

    Coroutine hitCounterTimer;

    private void EnemyHitted()
    {
        if (currentFaseCharacters[currentCharacterIndex] is DPS)
        {
            if (hitCounterTimer != null)
                tutorialManager.StopCoroutine(hitCounterTimer);

            comboHitCount++;

            if (comboHitCount == 3)
            {
                comboHitCount = 0;
                hitCount++;
                tutorialManager.objectiveNumberToReach.text = hitCount.ToString();
            }
            else
                hitCounterTimer = tutorialManager.StartCoroutine(ResetComboHitCounterTimer());
        }
        else
        {
            hitCount++;
            tutorialManager.objectiveNumberToReach.text = hitCount.ToString();
        }

        Debug.Log(hitCount);
    }

    int comboHitCount = 0;

    public override void Update()
    {
        base.Update();

        if (hitCount >= 3)
        {
            hitCount = 0;
            tutorialManager.tutorialEnemy.OnHitAction -= EnemyHitted;

            foreach (PlayerInputHandler ih in CoopManager.Instance.GetActiveHandlers())
            {
                ih.GetComponent<PlayerInput>().actions.FindAction("Move").Disable();
            }

            tutorialManager.inputBindings[currentFaseCharacters[currentCharacterIndex]].GetInputHandler().GetComponent<PlayerInput>().actions.FindAction("Attack").Disable();

            if (currentCharacterIndex < currentFaseCharacters.Length-1)
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
