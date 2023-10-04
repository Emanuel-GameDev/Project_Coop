using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClassCharacter
{
    tank,
    dps,
    ranged,
    healer
}
public class PlayerCharacter : Character
{
    [SerializeField] private ClassCharacter _class;
    [SerializeField] private SkillTree skillTree;

    public Ability activeability;

    public override void Attack()
    {
        
       skillTree.GetAttackData(this);
        //Play animazione attacco
    }
    public override void Defend()
    {
       skillTree.GetDefendData(this);
    }
    public override void Move()
    {
        skillTree.GetMoveData(this);
    }

    public void UniqueAbility()
    {
        skillTree.GetUniqueData(this);
    }
  






}
