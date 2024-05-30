using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

public class LodonTrident : MonoBehaviour
{
    LodonBoss boss;
    Transform parent;
    float velocity;
    Vector3 destination;
    Damager damager;

    public void Inizialize(LodonBoss boss)
    {
        this.boss = boss;
        this.damager = GetComponent<Damager>();
        damager.SetSource(boss);
        parent = boss.transform;
        this.gameObject.SetActive(false);
    }

    internal void Throw(Vector3 destination, Vector3 startPosition, float tridentVelocity)
    {
        transform.parent = null;
        this.velocity = tridentVelocity;
        this.destination = destination;
        transform.position = startPosition;
        RotateToDestination();
        this.gameObject.SetActive(true);
    }

    private void RotateToDestination()
    {
        Vector3 direction = destination - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        TridentFlyDirection();
    }

    private void TridentFlyDirection()
    {
        transform.position = Vector2.MoveTowards(transform.position, destination, velocity * Time.deltaTime);
        if (Vector2.Distance(transform.position, destination) < 0.1f)
            ReturnTrident();
    }

    public void ReturnTrident()
    {
        gameObject.SetActive(false);
        transform.parent = parent;
        transform.localPosition = Vector3.zero;
        boss.ReturnTrident();
    }

}
