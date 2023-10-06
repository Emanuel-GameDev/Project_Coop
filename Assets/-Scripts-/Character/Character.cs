using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour
{

    [SerializeField] protected float maxHp;
    [SerializeField] protected float currentHp;
    [SerializeField] protected float speed;
    [SerializeField] protected SkillTree skillTree;
    [HideInInspector] public List<PowerUp> powerPool;
    protected Rigidbody rb;

    //Lo uso per chimare tutte le funzioni iniziali
    protected virtual void Start()
    {
        InitialSetup();
    }

    protected virtual void Attack()
    {

    } 
    protected virtual void Defend()
    { 
    }

    //dati x e z chiama Move col Vector2
    protected virtual void Move(float x, float z)
    {
        Move(new Vector2(x, z));
    }
    
    // Dato un vector2 chiama move col Vector3
    protected virtual void Move(Vector2 direction)
    {
        Move(new Vector3(direction.x, transform.position.y, direction.y).normalized);
    }

    //dato un vector 3 setta la velocità del rigidBody in quella direzione, se il vettore non è normalizzato lo normalizza
    protected virtual void Move(Vector3 direction)
    {
        //skillTree.GetMoveData(this);

        if (!direction.normalized.Equals(direction))
            direction = direction.normalized;

        rb.velocity = new Vector3(direction.x * speed, direction.y, direction.z * speed);
    }

    //Tutto ciò che va fatto nello ad inizio
    protected virtual void InitialSetup()
    {
        rb = GetComponent<Rigidbody>();
    }


}
