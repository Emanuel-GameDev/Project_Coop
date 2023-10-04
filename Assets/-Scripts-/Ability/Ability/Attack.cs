using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(menuName = "Ability/Attack")]
public class Attack : Ability
{
    public float damage;
    public float velocity;
    public bool ranged;

    public override void Use(MonoBehaviour parent)
    {
       
    }
    private void MeleeAttack() 
    { 
        
    }
    private void RangedAttack() 
    { 

    }





}
