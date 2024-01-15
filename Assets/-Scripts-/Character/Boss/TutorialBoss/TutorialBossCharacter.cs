using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBossCharacter : BossCharacter
{
    [Header("Generics")]
    public float minDistance;

    [Header("Raffica Di Pugni")]
    public float flurryDistance;
    public float flurrySpeed;
    public int punchQuantity;
    public int normalPunchDamage;
    public int normalPunchStaminaDamage;
    public int lastPunchDamage;
    public int lastPunchStaminaDamage;

    [Header("Carica")]
    public float chargeDamage;
    public float chargeStaminaDamage;
    public float chargeTimer;
    public float chargeDuration;
    public float chargeSpeed;
    public float chargeDistance;
    public float ChargeStaminaDamage => chargeStaminaDamage + powerUpData.damageIncrease;

    [Header("Schianto")]
    public float crashDirectDamage;
    public float crashDirectStaminaDamage;
    public float crashWaveDamage;
    public float crashWaveStaminaDamage;
    public float crashTimer;

    public DamageData damageData;

    //StateMachine<TutorialBossState> stateMachine = new();


    private void Start()
    {
        //stateMachine.SetState(new FlurryOfBlows(this));
        
    }


    public void SetChargeDamageData()
    {
        staminaDamage = chargeStaminaDamage;
        damage = chargeDamage;
        attackCondition = null;

    }
    public void SetCrashDirectDamageData()
    {
        staminaDamage = crashDirectStaminaDamage;
        damage = crashDirectDamage;
        attackCondition = null;

    }
    public void SetCrashWaveDamageData()
    {
        staminaDamage = crashWaveStaminaDamage;
        damage = crashWaveDamage;
        attackCondition = null;
    }
    public void SetFlurryOfBlowsDamageData(int attackNumber)
    {

        if(attackNumber >= punchQuantity) 
        {
            staminaDamage = normalPunchStaminaDamage;
            damage = normalPunchDamage;
            attackCondition = null;
        }
        else
        {
            staminaDamage = lastPunchStaminaDamage;
            damage = lastPunchDamage;
            attackCondition = null;
        }
    }

}
