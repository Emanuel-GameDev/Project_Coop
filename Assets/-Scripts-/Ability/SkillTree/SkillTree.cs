using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName ="Ability/SkillTree")]
public class SkillTree : ScriptableObject
{
    public Ability attackAbility;

    public Ability defenseAbility;

    public Ability moveAbility;

    public Ability uniqueAbility;

    public virtual void GetAttackData(MonoBehaviour parent)
    {
        attackAbility.Use(parent);
    }
    public virtual void GetDefendData(MonoBehaviour parent)
    {
       defenseAbility.Use(parent);
    }
    public virtual void GetMoveData(MonoBehaviour parent)
    {
        moveAbility.Use(parent);
    }
    public virtual void GetUniqueData(MonoBehaviour parent)
    {
        uniqueAbility.Use(parent);
    }

}
