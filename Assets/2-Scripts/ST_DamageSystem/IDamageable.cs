using UnityEngine.Events;

public interface IDamageable
{
    public void TakeDamage(DamageData data);
    public UnityEvent OnHit
    {
        get;
        set;
    }
    public UnityEvent OnDeath
    {
        get;
        set;
    }
    public UnityEvent OnDefenceAbility
    {
        get;
        set;
    }
    
}