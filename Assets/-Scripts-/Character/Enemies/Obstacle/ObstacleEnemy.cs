using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleEnemy : MonoBehaviour,IDamager
{
    [Header("Variabili Ostacolo base")]

    [SerializeField,Tooltip("Il danno inflitto dall'ostacolo")]
    [Min(0)]
    protected float damage=5;
    protected float staminaDamage = 1;
    [SerializeField,Tooltip("La forza di spinta dell'ostacolo")]
    [Min(0)]
    protected float pushStrength=10000;
    [SerializeField, Tooltip("Il raggio del trigger dell'ostacolo")]
    [Min(1)]
    protected float triggerArea = 1;

    Animator animator;
    public Transform dealerTransform => transform;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public DamageData GetDamageData()
    {
        return new DamageData(damage, staminaDamage, this, false);
    }

    
}
  