using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class GuardTutorialState : TutorialFase
{
    TutorialManager tutorialManager;

    GuardTutorialFaseData faseData;

    int guardExecuted = 0;

    public GuardTutorialState(TutorialManager tutorialManager)
    {
        this.tutorialManager = tutorialManager;
    }

    public override void Enter()
    {
        base.Enter();

        guardExecuted = 0;

        faseData = (GuardTutorialFaseData) tutorialManager.fases[tutorialManager.faseCount].faseData;
        PubSub.Instance.RegisterFunction(EMessageType.guardExecuted, UpdateCounter);

        tutorialManager.DeactivatePlayerInput(tutorialManager.dps);
        tutorialManager.DeactivatePlayerInput(tutorialManager.healer);
        tutorialManager.DeactivatePlayerInput(tutorialManager.ranged);
        tutorialManager.DeactivatePlayerInput(tutorialManager.tank);

        tutorialManager.dialogueBox.OnDialogueEnded += WaitAfterDialogue;
        tutorialManager.PlayDialogue(faseData.faseStartDialogue);

        var firstPlayer = tutorialManager.dps.GetComponent<PlayerInput>().user;
        var firstControl = tutorialManager.dps.GetComponent<PlayerInput>().currentControlScheme;
        var secondPlayer = tutorialManager.tank.GetComponent<PlayerInput>().user;

        InputDevice[] firstDevices = firstPlayer.pairedDevices.ToArray();

        tutorialManager.tank.GetComponent<PlayerInput>().SwitchCurrentControlScheme(firstControl);
        
        foreach (InputDevice device in firstDevices )
        InputUser.PerformPairingWithDevice(device,
            tutorialManager.tank.GetComponent<PlayerInput>().user);

        firstPlayer.UnpairDevices();
    }

    private void WaitAfterDialogue()
    {
        tutorialManager.dialogueBox.OnDialogueEnded -= WaitAfterDialogue;

        tutorialManager.tank.GetComponent<PlayerInput>().actions.FindAction("Move").Enable();
        tutorialManager.tank.GetComponent<PlayerInput>().actions.FindAction("Defense").Enable();

        tutorialManager.ActivateEnemyAI();
    }

    private void UpdateCounter(object obj)
    {
        guardExecuted++;
        Debug.Log("conta");
    }

    public override void Update()
    {
        base.Update();

        if(guardExecuted >= faseData.numberOfBlockToPass)
        {
            //ora perfetti
            Debug.Log("3");
        }

    }


    public override void Exit()
    {
        base.Exit();
        PubSub.Instance.UnregisterFunction(EMessageType.guardExecuted, UpdateCounter);
    }
}
