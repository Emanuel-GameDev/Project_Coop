using MBT;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TutorialBossCharacter : BossCharacter
{
    [Header("Generics")]
    public float minDistance;
    public float followDuration;
    public float walkSpeed;
    public float attackPreviewTimer;
    public GameObject previewArrow;
    public GameObject visual;
    public GameObject _pivot;
    public UnityEvent OnDie;
    public Animator anim => animator;
    

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
    public float ChargeStaminaDamage => chargeStaminaDamage + powerUpData.damageIncrease;

    [Header("Schianto")]
    public float crashDirectDamage;
    public float crashDirectStaminaDamage;
    public float crashWaveDamage;
    public float crashWaveStaminaDamage;
    public float crashTimer;

    [HideInInspector] public DamageData damageData;
    [HideInInspector] public bool canAttackAnim;
    [HideInInspector] public bool canShowPreview;
    [HideInInspector] public bool previewStarted;
    [HideInInspector] public bool canLastAttackPunch;
    [HideInInspector] private bool canRotateInAnim;
    [HideInInspector] private Vector2 direction;




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
                previewArrow.transform.rotation = Quaternion.LookRotation(direction);
            }
        }
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
    public override void TakeDamage(DamageData data)
    {
        base.TakeDamage(data);

        if (isDead)
        {
            gameObject.GetComponentInChildren<Blackboard>().GetVariable<BoolVariable>("isDead").Value = true;
            OnDie?.Invoke();

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
        previewArrow.SetActive(false);
        
        canShowPreview = false;
        canAttackAnim = true;
        
        
    }
    public void SetCanShowPreview()
    {
        previewStarted = false;
        canShowPreview = true;
        Debug.Log("inizio preview");
    }
    public void SetCanRotateInAnim(int value)
    {
        SetSpriteDirection(direction);

        if (value == 0)
        {
            canRotateInAnim = false;
        }
        else
        {
            canRotateInAnim = true;
        }
    }
    public void SetCanLastAttackPunch()
    {
        canLastAttackPunch = true;
    }
    
}
