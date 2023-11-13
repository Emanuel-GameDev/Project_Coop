using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damager : MonoBehaviour
{
    [SerializeField]
    LayerMask targetLayers;

    IDamager source;

    [SerializeField]
    UnityEvent<Collider> onTrigger = new();

    private void OnTriggerEnter(Collider other)
    {
        onTrigger.Invoke(other);

        if (Utility.IsInLayerMask(other.gameObject.layer, targetLayers))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(source.GetDamage(), source);
            }
        }
    }

    private void Awake()
    {
        source ??= GetComponentInParent<IDamager>();
        source ??= GetComponent<IDamager>();
        source ??= GetComponentInChildren<IDamager>();
    }

    public void AssignFunctionToOnTrigger(UnityAction<Collider> action)
    {
        onTrigger.AddListener(action);
    }

    public void RemoveFunctionFromOnTrigger(UnityAction<Collider> action)
    {
        onTrigger.RemoveListener(action);
    }

    public void SetSource(IDamager character)
    {
        source = character;
    }
}
