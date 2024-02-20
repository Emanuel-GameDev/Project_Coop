using MBT;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    public bool canAttackAnim;
   

   
   
    private void Update()
    {
        if (target != null)
        {
            Vector2 direction = new Vector2(target.gameObject.transform.position.x - transform.position.x, target.gameObject.transform.position.z - transform.position.z);
            SetSpriteDirection(direction);
            
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
        canAttackAnim = true;
        previewArrow.SetActive(false);
        Debug.Log("Fine preview");
    }
 

}
