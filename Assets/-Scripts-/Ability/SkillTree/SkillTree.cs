using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Ability/SkillTree")]
public class SkillTree : ScriptableObject
{
    public Ability attackAbility;

    public Ability defenseAbility;

    public Ability moveAbility;
    
    public virtual void Attack()
    {
        attackAbility.Use();
    }
    public virtual void Defend()
    {
       defenseAbility.Use();
    }
    public virtual void Move()
    {
        moveAbility.Use();
    }
}
