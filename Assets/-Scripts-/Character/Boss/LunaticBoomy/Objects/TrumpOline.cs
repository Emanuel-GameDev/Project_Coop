using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrumpOline : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float maxHp = 3f;

    // variabile che si vede in inspector, per i designer
    public float currHp;
    private float _currHp;

    private void Awake()
    {
        _currHp = maxHp;
        currHp = _currHp;
    }

    private void UpdateCurrHp(float hp)
    {
        _currHp = hp;
        currHp = _currHp;
    }

    public void TakeDamage(DamageData data)
    {
        float newHp = _currHp - data.damage;

        if (newHp <= 0)
        {
            // Morto
        }
        else
        {
            UpdateCurrHp(newHp);
        }
    }

    private void ResetTrumpOline()
    {
        UpdateCurrHp(maxHp);
    }
}
