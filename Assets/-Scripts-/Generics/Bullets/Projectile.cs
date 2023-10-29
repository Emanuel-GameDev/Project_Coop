using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 travelDirection;
    [SerializeField] private float speed;
    [SerializeField] private float lifetime=5f;
    private float timer;

    private void Start()
    {
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
        transform.position += (Vector3)travelDirection * speed * Time.deltaTime;
    }

    public void SetTravelDirection(Vector2 direction)
    {
        travelDirection= direction;
    }
}
