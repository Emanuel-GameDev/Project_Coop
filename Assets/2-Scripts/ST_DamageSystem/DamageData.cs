using System.Numerics;

public class DamageData
{
    public float damage;
    public float staminaDamage = 0;
    public IDamager dealer;
    public Condition condition = null;
    public bool bossAttack = false;  
    public bool blockedByTank;

    public DamageData(float damage, IDamager dealer)
    {
        this.damage = damage;
        this.dealer = dealer; 
    }

    public DamageData(float damage, IDamager dealer, Condition condition)
    {
        this.damage = damage;
        this.dealer = dealer;
        this.condition = condition;
    }
    public DamageData(float damage,float staminaDamage, IDamager dealer, bool bossAttack)
    {
        this.damage = damage;
        this.staminaDamage = staminaDamage;
        this.dealer = dealer;
        this.bossAttack = bossAttack;
    }
    public DamageData(float damage, float staminaDamage, IDamager dealer, Condition condition,bool bossAttack)
    {
        this.damage = damage;
        this.staminaDamage = staminaDamage;
        this.dealer = dealer;
        this.condition = condition;
        this.bossAttack = bossAttack;
    }


}
