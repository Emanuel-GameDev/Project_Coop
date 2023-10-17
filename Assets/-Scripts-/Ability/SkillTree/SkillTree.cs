using UnityEngine;

[CreateAssetMenu(menuName = "Ability/SkillTree")]
public class SkillTree : ScriptableObject
{
    public Attack attackAbility;

    public Defence defenseAbility;

    public Move moveAbility;

    public Ability uniqueAbility;

    public Ability extraAbility;

    public virtual Attack GetAttackData(MonoBehaviour parent)
    {
        return (Attack)attackAbility.GetData(parent);
    }
    public virtual Defence GetDefendData(MonoBehaviour parent)
    {
        return (Defence)defenseAbility.GetData(parent);
    }
    public virtual Move GetMoveData(MonoBehaviour parent)
    {
        return (Move)moveAbility.GetData(parent);
    }
    public virtual void UseUniqueData(MonoBehaviour parent)
    {
        uniqueAbility.Use(parent);
    }


    //Abilità sbloccabile con upgrade
    public virtual void UseExtraAbility(MonoBehaviour parent)
    {
        extraAbility.Use(parent);
    }

}
