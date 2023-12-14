using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damager : MonoBehaviour
{
    [SerializeField] public
    LayerMask targetLayers;

    IDamager source;
    Condition conditionToApply=null;
    bool oneTimeCondition;

    [SerializeField]
    UnityEvent<Collider> onTrigger = new();

    //decidere se tenere ConditionToApply

    private void OnTriggerEnter(Collider other)
    {
        onTrigger.Invoke(other);

        if (Utility.IsInLayerMask(other.gameObject.layer, targetLayers))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
               
                //modifiche

                // damageable.TakeDamage(new DamageData(source.GetDamage(),source, conditionToApply));
                DamageData newData = source.GetDamageData();

                if(newData.condition==null && conditionToApply != null)
                    newData.condition = conditionToApply;

                damageable.TakeDamage(newData);

                //fine modifica

                if (oneTimeCondition)
                    conditionToApply = null;
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

  
    public void SetCondition(Condition condition, bool oneTime)
    {
        condition.transform.parent = transform;
        conditionToApply = condition;
        oneTimeCondition = oneTime;
    }

    public void RemoveCondition()
    {
        conditionToApply = null;
    }
}
