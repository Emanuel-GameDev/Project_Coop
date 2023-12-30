using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermediateTutorialFase : State
{
    TutorialManager tutorialManager;

    public IntermediateTutorialFase(TutorialManager tutorialManager)
    {
        this.tutorialManager = tutorialManager;
    }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Update()
    {
        base.Enter();
        
    }


    public override void Exit()
    {
        base.Enter();
    }
}
