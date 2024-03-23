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

    public event System.Action<Collider2D> OnTriggerEnterEvent;
    public event System.Action<Collider2D> OnTriggerStayEvent;

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
        if (OnTriggerEnterEvent != null)
        {
            OnTriggerEnterEvent(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (OnTriggerStayEvent != null)
            OnTriggerStayEvent(collision);
    }
}
