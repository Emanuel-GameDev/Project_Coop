using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonErosBossCharacter : BossCharacter
{
    [Header("Generics")]
    public float minDistance;
    public float followDuration;
    public float walkSpeed;
    public SpriteRenderer spriteRenderer;
    public MeshRenderer shadowMesh;

    [HideInInspector] public DamageData damageData;

    [Header("Pioggia Di Natiche")]  
    public int slamsQuantity;
    public int timerDamageable;
    public int timerSlamFollow;
    public int slamNormalDamage;
    public int slamNormalStaminaDamage;
    public int slamAreaDamage;
    public int slamAreaStaminaDamage;

    
    
    

    public void SetRainOfAssDamageData()
    {
        
    }
    public void WhipAttackDamageData()
    {
        
    }
    public void CrossbowDamageData()
    {
        
    }
    
}
