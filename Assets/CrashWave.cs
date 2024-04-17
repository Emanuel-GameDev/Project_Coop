using System;
using UnityEngine;

public class CrashWave : MonoBehaviour, IDamager
{

    [SerializeField] ParticleSystem particle;
    float damage;
    float staminaDamage;
    Character dealer;
    ParticleSystem.ShapeModule sh;

    public Transform dealerTransform => throw new NotImplementedException();

    private void OnEnable()
    {
        particle = GetComponentInChildren<ParticleSystem>();
        sh = particle.shape;
    }
    private void Update()
    {
        sh.scale = transform.localScale;
    }

    public void DestroyWave()
    {
        particle.Stop();
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