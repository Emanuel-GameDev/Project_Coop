using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HealTutorialState : TutorialFase
{
    TutorialManager tutorialManager;
    Dictionary<Character, bool> playerHealed;

    HealTutorialFaseData faseData;

    public HealTutorialState(TutorialManager tutorialManager)
    {
        this.tutorialManager = tutorialManager;
    }

    public override void Enter()
    {
        base.Enter();

        faseData = (HealTutorialFaseData)tutorialManager.fases[tutorialManager.faseCount].faseData;

        playerHealed = new Dictionary<Character, bool>();
        playerHealed.Add(tutorialManager.dps, false);
        playerHealed.Add(tutorialManager.ranged, false);
        playerHealed.Add(tutorialManager.tank, false);
        playerHealed.Add(tutorialManager.tutorialEnemy, false);

        DamageData damageData = new DamageData(1, null);

        tutorialManager.dps.CharacterClass.TakeDamage(damageData);
        tutorialManager.ranged.CharacterClass.TakeDamage(damageData);
        tutorialManager.tank.CharacterClass.TakeDamage(damageData);
        tutorialManager.tutorialEnemy.TakeDamage(damageData);

        PubSub.Instance.RegisterFunction(EMessageType.characterHealed, CharacterHealed);

        tutorialManager.DeactivatePlayerInput(tutorialManager.dps);
        tutorialManager.DeactivatePlayerInput(tutorialManager.healer);
        tutorialManager.DeactivatePlayerInput(tutorialManager.ranged);
        tutorialManager.DeactivatePlayerInput(tutorialManager.tank);

        tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
        tutorialManager.PlayDialogue(faseData.faseStartDialogue);

    }

    private void WaitAfterDialogue()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= WaitAfterDialogue;

        tutorialManager.healer.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        tutorialManager.healer.GetComponent<PlayerInput>().actions.FindAction("Defence").Enable();
    }

    private void CharacterHealed(object obj)
    {
        if(obj is PlayerCharacter) 
        { 
            PlayerCharacter character = (PlayerCharacter)obj;
            
        }
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
