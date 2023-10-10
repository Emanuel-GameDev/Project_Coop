using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(menuName = "Ability/Attack")]
public class Attack : Ability
{
    public float damage;
    public float cooldown;
    public bool ranged;
    public GameObject prefabBullet;
    public float range;


    public override void Use(MonoBehaviour parent)
    {
        if (ranged)
        {
            RangedAttack();
        }
        else
        {
            MeleeAttack();
        }

    }
    private void MeleeAttack() 
    { 
        
    }
    private void RangedAttack() 
    { 

    }





}
