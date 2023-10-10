using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    [SerializeField] protected float maxHp;
    [SerializeField] protected float currentHp;
    [SerializeField] private float _speed;

    protected float Speed 
    {
        get { return _speed + powerUpData.speedIncrease; } 
        set { _speed = value; }
    }

   
    [SerializeField] protected SkillTree skillTree;
    [HideInInspector] protected PowerUpData powerUpData;
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
        Move(new Vector3(direction.x, 0, direction.y).normalized);
    }

    //dato un vector 3 setta la velocità del rigidBody in quella direzione, se il vettore non è normalizzato lo normalizza
    protected virtual void Move(Vector3 direction)
    {
        //skillTree.GetMoveData(this);

        if (!direction.normalized.Equals(direction))
            direction = direction.normalized;

        rb.velocity = new Vector3(direction.x * Speed, direction.y, direction.z * Speed);
    }

    //Tutto ciò che va fatto nello ad inizio
    protected virtual void InitialSetup()
    {
        rb = GetComponent<Rigidbody>();
        powerUpData = new();
    }

    public void AddPowerUp(PowerUp powerUp)
    {
        powerUpData.Add(powerUp);
    }
    public void RemovePowerUp(PowerUp powerUp)
    {
        powerUpData.Remove(powerUp);
    }
    public List<PowerUp> GetPowerUpList()
    {
        return powerUpData._powerUpData;
    }



}
