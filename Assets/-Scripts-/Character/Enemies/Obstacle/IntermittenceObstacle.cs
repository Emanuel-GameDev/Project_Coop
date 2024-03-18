using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermittenceObstacle : ObstacleEnemy
{
    [Header("Variabili esclusive Ostacolo intermittente")]

    [SerializeField, Tooltip("Intervallo di tempo di idle")]
    [Min(0)]
    protected float idleTime = 5;
    float idleTimer;
    [SerializeField, Tooltip("Intervallo di tempo di action")]
    [Min(0)]
    protected float actionTime = 5;
    float actionTimer;
}
