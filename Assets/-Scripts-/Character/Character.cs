using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private float maxHp;
    [SerializeField] private float currentHp;
    [SerializeField] private float speed;



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
