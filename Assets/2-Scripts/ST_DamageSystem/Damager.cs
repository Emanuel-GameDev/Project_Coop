using UnityEngine;
using UnityEngine.Events;

public class Damager : MonoBehaviour
{
    [SerializeField] GameObject HitVFX;

    [SerializeField]
    public LayerMask targetLayers;

    public IDamager source;
    Condition conditionToApply = null;
    bool oneTimeCondition;

    [SerializeField]
    UnityEvent<Collider2D> onTrigger = new();

    

    //decidere se tenere ConditionToApply


    private void OnTriggerEnter2D(Collider2D other)
    {
        onTrigger.Invoke(other);

        if (Utility.IsInLayerMask(other.gameObject.layer, targetLayers))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                DamageData newData = source.GetDamageData();

                if (newData.condition == null && conditionToApply != null)
                    newData.condition = conditionToApply;

                damageable.TakeDamage(newData);

                if (oneTimeCondition)
                    conditionToApply = null;

                if(HitVFX != null)
                {
                    //Collider2D[] contacts = new Collider2D[1];
                    //other.GetContacts(contacts);

                    //if (contacts[0]!=null)
                    //{
                        GameManager.Instance.SpawnVFXObject(HitVFX, other.ClosestPoint(transform.position));
                   // }
                }
            }
        }
    }

    private void Awake()
    {
        source ??= GetComponentInParent<IDamager>();
        source ??= GetComponent<IDamager>();
        source ??= GetComponentInChildren<IDamager>();
    }

    public void AssignFunctionToOnTrigger(UnityAction<Collider2D> action)
    {
        onTrigger.AddListener(action);
    }

    public void RemoveFunctionFromOnTrigger(UnityAction<Collider2D> action)
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
