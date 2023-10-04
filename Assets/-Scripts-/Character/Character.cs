using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    [SerializeField] private float maxHp;
    [SerializeField] private float currentHp;
    [SerializeField] protected float speed;
    [SerializeField] protected SkillTree skillTree;
    [HideInInspector] public List<PowerUp> powerPool;

    public virtual void Move()
    {

    }
    public virtual void Attack()
    {

    } 
    public virtual void Defend()
    { 
    }

}
