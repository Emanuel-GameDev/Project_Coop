public class DamageData
{
    public float damage;
    public float staminaDamage;
    public IDamager dealer;
    public Condition condition;

    public DamageData(float damage, IDamager dealer)
    {
        this.damage = damage;
        this.staminaDamage = 0;
        this.dealer = dealer;
        this.condition = null;
    }

    public DamageData(float damage, IDamager dealer, Condition condition)
    {
        this.damage = damage;
        this.staminaDamage = 0;
        this.dealer = dealer;
        this.condition = condition;
    }
    public DamageData(float damage,float staminaDamage, IDamager dealer)
    {
        this.damage = damage;
        this.staminaDamage = staminaDamage;
        this.dealer = dealer;
        this.condition = null;
    }
    public DamageData(float damage, float staminaDamage, IDamager dealer, Condition condition)
    {
        this.damage = damage;
        this.staminaDamage = staminaDamage;
        this.dealer = dealer;
        this.condition = condition;
    }
}
