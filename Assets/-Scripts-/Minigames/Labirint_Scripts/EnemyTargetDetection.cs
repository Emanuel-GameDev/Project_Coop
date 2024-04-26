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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<LabirintPlayer>())
        {
            labirintEnemy.SetTarget(other.transform);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(transform.localScale.x, transform.localScale.y, 0));
    }
}
