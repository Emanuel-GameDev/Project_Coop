using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardTutorialState : TutorialFase
{
    TutorialManager tutorialManager;

    GuardTutorialFaseData faseData;

    public GuardTutorialState(TutorialManager tutorialManager)
    {
        this.tutorialManager = tutorialManager;
    }

    public override void Enter()
    {
        base.Enter();

        faseData = (GuardTutorialFaseData) tutorialManager.fases[tutorialManager.faseCount].faseData;

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
