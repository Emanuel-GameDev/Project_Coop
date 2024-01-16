using System.Collections.Generic;
using UnityEngine;

public class TutorialBossCharacter : BossCharacter
{
    [Header("Generics")]
    public float minDistance = 0.1f;
    [Header("Target & Move Selection")]
    public float shortDistanceRange = 15f;
    [Range(0, 1f)]
    public float shortDistanceChancePercentage = 0.65f;
    [Range(0, 1f)]
    public float moveRepeatPercentage = 0.25f;
    [Header("Raffica Di Pugni")]
    public float flurryDistance;
    public float flurrySpeed;
    public int punchQuantity;
    [Header("Carica")]
    public float chargeTimer;
    public float chargeDuration;
    public float chargeSpeed;
    public float chargeDistance;
    [Header("Mvoimento")]
    public float moveDuration;


}
