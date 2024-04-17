using System;
using System.Collections.Generic;
using UnityEngine;

public class CrashWave : MonoBehaviour, IDamager
{

   
    [SerializeField] ParticleSystem particle1;
    [SerializeField] ParticleSystem particle2;
    float damage;
    float staminaDamage;
    Character dealer;

  
    ParticleSystem.ShapeModule sh1;
    ParticleSystem.ShapeModule sh2;



    public Transform dealerTransform => throw new NotImplementedException();

    private void OnEnable()
    {
        sh1 = particle1.shape;
        sh2 = particle2.shape;
      
    }
    private void Update()
    {
       
        sh1.scale = transform.localScale;
        sh2.scale = transform.localScale;
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
        Debug.Log("AAAAAAAAAAAAAAAA");
    }
}