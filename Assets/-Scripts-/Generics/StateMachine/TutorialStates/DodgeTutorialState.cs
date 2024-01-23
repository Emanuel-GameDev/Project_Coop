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


        tutorialManager.blockFaseChange = true;
        currentCharacterIndex = -1;

        characters = new PlayerCharacter[2] { tutorialManager.dps, tutorialManager.ranged };
        charactersPreTutorialDialogue = new Dialogue[2] { faseData.dpsDodgeDialogue, faseData.rangedDodgeDialogue };
        charactersPerfectTutorialDialogue = new Dialogue[2] { faseData.dpsPerfectDodgeDialogue, faseData.rangedPerfectDodgeDialogue };

        for (int i = 0; i < 2; i++)
        {
            tutorialManager.DeactivatePlayerInput(characters[i]);
        }

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
    int dodgeCount = 0;

    private void StartSubFase()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= StartSubFase;

        dodgeCount = 0;

        characters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        characters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Attack").Enable();
        characters[currentCharacterIndex].GetComponent<PlayerInput>().actions.FindAction("Defense").Enable();

        //tutorialManager.tutorialEnemy.OnHit += EnemyHitted;


    }

    public IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);

        SetupNextCharacter();
    }

    public override void Update()
    {
        base.Update();


    }


    public override void Exit()
    {
        base.Exit();
    }
}
