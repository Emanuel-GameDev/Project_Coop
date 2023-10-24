using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector2 travelDirection;
    [SerializeField] private float speed;
    private void Start()
    {
        transform.LookAt(travelDirection);
    }

    private void FixedUpdate()
    {
        BulletFlyDirection();
    }

    private void BulletFlyDirection()
    {
        transform.position=transform.forward*speed;
    }
}
