using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBossCharacter : BossCharacter
{
    public float chargeTimer;
    public float chargeDuration;
    public float chargeSpeed;
    public float chargeDistance;

    StateMachine<TutorialBossState> stateMachine = new();


    private void Start()
    {
        stateMachine.SetState(new FlurryOfBlows(this));
        TargetSection();
    }

}
