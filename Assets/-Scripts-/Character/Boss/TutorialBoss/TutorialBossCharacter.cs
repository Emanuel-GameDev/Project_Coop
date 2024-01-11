using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBossCharacter : BossCharacter
{
    [Header("Generics")]
    public float minDistance = 0.1f;
    [Header("Raffica Di Pugni")]
    public float flurryDistance;
    public float flurrySpeed;
    public int punchQuantity;
    [Header("Carica")]
    public float chargeTimer;
    public float chargeDuration;
    public float chargeSpeed;
    public float chargeDistance;
   

    StateMachine<TutorialBossState> stateMachine = new();


    private void Start()
    {
        stateMachine.SetState(new TBCharge(this));
        
    }

    private void Update()
    {
        stateMachine.StateUpdate();
    }

}
