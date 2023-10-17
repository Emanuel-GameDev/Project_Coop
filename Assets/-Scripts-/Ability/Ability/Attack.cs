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
            RangedAttack(parent);
        }
        else
        {
            MeleeAttack();
        }

    }
    private void MeleeAttack() 
    { 
        
    }
    private void RangedAttack(MonoBehaviour parent) 
    {
        if(parent.GetComponent<PlayerCharacter>() != null)
        {
            Vector2 AttackDirection = parent.GetComponent<PlayerCharacter>().GetReadLook();
        }
        GameObject bullet = Instantiate(prefabBullet);
        bullet.transform.position=parent.gameObject.transform.position;

        

        
    }





}
