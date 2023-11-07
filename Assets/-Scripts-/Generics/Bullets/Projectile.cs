using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 travelDirection;
    [SerializeField] private float speed;
    [SerializeField] private float lifetime=5f;
    private float timer;

    Rigidbody rb;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        transform.LookAt(travelDirection);
    }

    private void OnEnable()
    {
        timer = lifetime;
    }

    private void FixedUpdate()
    {
        ProjectileFlyDirection();
        ProjectileLiveTimer();
    }

    private void ProjectileLiveTimer()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            ProjectilePool.Instance.ReturnProjectile(this);
        }
    }

    private void ProjectileFlyDirection()
    {
        //transform.position += travelDirection.normalized * speed * Time.deltaTime;

        
        transform.position=Vector3.MoveTowards(transform.position,travelDirection,speed*Time.deltaTime);

    }

    public void SetTravelDirection(Vector3 direction)
    {                
            travelDirection = direction*1000;              
    }
}
