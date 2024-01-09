using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTutorialState : TutorialFase
{
    TutorialManager tutorialManager;

    public AttackTutorialState(TutorialManager tutorialManager)
    {
        this.tutorialManager = tutorialManager;
    }

    public override void Enter()
    {
        base.Enter();

        PubSub.Instance.RegisterFunction(EMessageType.dpsCombo, AttackCount);

        if (tutorialManager.current is DPS) 
        {
            PubSub.Instance.RegisterFunction(EMessageType.dpsCombo, AttackCount);
        }

        if(tutorialManager.current is Healer)
        {
            PubSub.Instance.RegisterFunction(EMessageType.healerCombo, AttackCount);
        }

        if (tutorialManager.current is Healer)
        {
            PubSub.Instance.RegisterFunction(EMessageType.healerCombo, AttackCount);
        }

        if (tutorialManager.current is Healer)
        {
            PubSub.Instance.RegisterFunction(EMessageType.healerCombo, AttackCount);
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
    int i = 0;
    private void AttackCount(object obj)
    {
        i++;
        Debug.Log(i);
    }
}
