using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveObstacle : ObstacleEnemy
{

    [Header("Variabili escusive Ostacolo esplosivo")]

    [SerializeField, Tooltip("Il raggio dell'esplosione")]
    [Min(1)]
    protected float explosionArea = 1;
}
