public class DamageData
{
    public float damage;
    public IDamager dealer;
    public Condition condition;

    public DamageData(float damage, IDamager dealer)
    {
        this.damage = damage;
        this.dealer = dealer;
        this.condition = null;
    }

    public DamageData(float damage, IDamager dealer, Condition condition)
    {
        this.damage = damage;
        this.dealer = dealer;
        this.condition = condition;
    }
}
