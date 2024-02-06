using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetDetection : MonoBehaviour
{
    LabirintEnemy labirintEnemy;

    private void Awake()
    {
        labirintEnemy = GetComponentInParent<LabirintEnemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<LabirintPlayer>())
        {
            labirintEnemy.SetTarget(other.transform);
        }
    }
}
