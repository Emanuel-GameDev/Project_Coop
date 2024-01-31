using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    private void UpdateCounter(object obj)
    {
        guardExecuted++;
    }

    public override void Update()
    {
        base.Update();

        if(guardExecuted >= faseData.numberOfBlockToPass)
        {
            //ora perfetti
        }

    }


    public override void Exit()
    {
        base.Exit();
        PubSub.Instance.UnregisterFunction(EMessageType.guardExecuted, UpdateCounter);
    }
}
