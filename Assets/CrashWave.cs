using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashWave : MonoBehaviour, IDamager
{
    public Damager damager;
    float damage;
    float staminaDamage;
    Character dealer;

    public Transform dealerTransform => throw new NotImplementedException();

    private void Start()
    {
        GetComponent<Animation>().Play();
    }

    public void DestroyWave()
    {
        Destroy(gameObject);
    }

    internal void SetVariables(float crashWaveDamage, float crashWaveStaminaDamage, Character dealer)
    {
       damage = crashWaveDamage;
        staminaDamage = crashWaveStaminaDamage;
        this.dealer = dealer;
    }

    public DamageData GetDamageData()
    {
        return new DamageData(damage, staminaDamage, dealer, true);
    }

    public void OnParryNotify(Character whoParried)
    {
        
    }
}
