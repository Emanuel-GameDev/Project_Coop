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
    public override void Attack()
    {
        skillTree.Attack();
        //Play animazione attacco
    }
    public override void Defend()
    {
       skillTree.Defend();
    }
    public override void Move()
    {
        skillTree.Move();
    }
}
