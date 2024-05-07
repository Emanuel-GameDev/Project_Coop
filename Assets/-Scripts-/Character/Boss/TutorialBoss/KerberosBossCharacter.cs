using MBT;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class KerberosBossCharacter : BossCharacter
{
    [Header("Generics")]
    public float lowHpPhaseTreshold;
    public float minDistance;
    public float followDuration;
    public float walkSpeed;
    public float attackPreviewTimer;
    public GameObject pivotPreviewArrow;
    public GameObject visual;
    public GameObject _pivot;
   
    
   
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
    public float chargeSpeed;
    public float chargeDistance;
    public float ChargeStaminaDamage => chargeStaminaDamage + powerUpData.DamageIncrease;
    public float ChargeOnParryPushForce = 3;
    public float ChargeOnParryDuration = 1;

    [Header("Schianto")]
    public float crashDirectDamage;
    public float crashDirectStaminaDamage;
    public float crashWaveDamage;
    public float crashWaveStaminaDamage;
    public float crashTimer;
    public float crashPushForce = 2;
    public float crashPushDuration = 1;
    public GameObject crashwaveObject;
    public Transform crashwaveTransform;
    
    

    [Header("IfParried")]
    public float parryStunTimer = 3;


    [HideInInspector] public DamageData damageData;
    [HideInInspector] public bool canAttackAnim;
    [HideInInspector] public bool canShowPreview;
    [HideInInspector] public bool previewStarted;
    [HideInInspector] public bool canLastAttackPunch;
    [HideInInspector] private bool canRotateInAnim;   
    [HideInInspector] private Vector2 direction;
    public bool parried = false;
    [HideInInspector] public Character whoParried;
    [HideInInspector] public Animator anim => animator;
    [HideInInspector] public bool waveSpawned;




    #region Crash

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
    public void InstantiateCrashWave()
    {
        if (parried == false)
        {
            GameObject instantiatedWave = Instantiate(crashwaveObject, crashwaveTransform.position, Quaternion.identity, transform);
            instantiatedWave.GetComponentInChildren<CrashWave>().SetVariables(crashWaveDamage, crashWaveStaminaDamage, this);
            waveSpawned = true;
           
        }
    }
    
    #endregion
    #region flurryOfBlows
    public void SetFlurryOfBlowsDamageData(int attackNumber)
    {

        if (attackNumber >= punchQuantity)
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
    public void SetCanLastAttackPunch()
    {
        canLastAttackPunch = true;
    }
   
    #endregion


    private void Update()
    {
        
        if (target != null)
        {
            direction = new Vector2(target.gameObject.transform.position.x - transform.position.x, target.gameObject.transform.position.y - transform.position.y);

            if (canRotateInAnim)
            {               
                SetSpriteDirection(direction);
               
            }

            if (canShowPreview)
            {               
                pivotPreviewArrow.transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
    public void SetChargeDamageData()
    {
        staminaDamage = chargeStaminaDamage;
        damage = chargeDamage;
        attackCondition = null;
        

    }

    public override void TakeDamage(DamageData data)
    {
        if (!isDead)
        {
            base.TakeDamage(data);
            if(currentHp <=0)
                gameObject.GetComponentInChildren<Blackboard>().GetVariable<BoolVariable>("isDead").Value = true;

        }

        float currentPercentage = (currentHp/maxHp) * 100;
        if(currentPercentage <= lowHpPhaseTreshold)
        {
            gameObject.GetComponentInChildren<Blackboard>().GetVariable<BoolVariable>("lowHp").Value = true;
        }

       
    }
    protected override void SetSpriteDirection(Vector2 direction)
    {
        if (direction.y != 0)
            anim.SetFloat("Y", direction.y);

        Vector3 scale = _pivot.gameObject.transform.localScale;

        if ((direction.x > 0.5 && scale.x > 0) || (direction.x < -0.5 && scale.x < 0))
            scale.x *= -1;

        _pivot.gameObject.transform.localScale = scale;
    }
    public IEnumerator StartAttackPunch()
    {
      
        yield return new WaitForSeconds(attackPreviewTimer);
        Debug.Log("fine preview");
        pivotPreviewArrow.SetActive(false);
        
        canShowPreview = false;
        canAttackAnim = true;
        
        
    }
    public void SetCanShowPreview()
    {
        previewStarted = false;
        canShowPreview = true;
        
    }
    public void SetCanRotateInAnim(int value)
    {
       
        if (value == 0)
        {
            canRotateInAnim = false;
            SetSpriteDirection(direction);
        }
        if(value == 3)
        {
            canRotateInAnim = false;
        }
        else
        {
            canRotateInAnim = true;
        }
    }  
    public override void OnParryNotify(Character whoParried)
    {
        parried = true;        
        this.whoParried = whoParried;
    }
    public IEnumerator UnstunFromParry()
    {
        yield return new WaitForSeconds(parryStunTimer);
        anim.SetTrigger("Return");
        yield return new WaitForSeconds(1f);
        gameObject.GetComponentInChildren<Blackboard>().GetVariable<BoolVariable>("parried").Value = false;
    }
    public override void Death()
    {
        base.Death();
        target = null;

    }

}
