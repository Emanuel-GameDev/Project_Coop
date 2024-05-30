using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrumpOline : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float maxHp = 3f;

    // variabile che si vede in inspector, per i designer
    public float currHp;
    private float _currHp;

    // Temporanea
    public bool destroyed = false;

    public UnityEvent OnHit { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public UnityEvent OnDeath { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public UnityEvent OnDefenceAbility { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public event UnityAction<Collider2D> OnTriggerEnterEvent;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerEnterEvent?.Invoke(collision);
    }

    public void ClearEventData()
    {
        OnTriggerEnterEvent = null;
    }
}
