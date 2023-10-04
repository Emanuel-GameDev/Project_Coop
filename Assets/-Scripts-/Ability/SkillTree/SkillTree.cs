using UnityEngine;

[CreateAssetMenu(menuName = "Ability/SkillTree")]
public class SkillTree : ScriptableObject
{
    public Ability attackAbility;

    public Ability defenseAbility;

    public Ability moveAbility;

    public Ability uniqueAbility;

    public virtual Ability GetAttackData(MonoBehaviour parent)
    {
        return attackAbility.GetData(parent);
    }
    public virtual Ability GetDefendData(MonoBehaviour parent)
    {
        return defenseAbility.GetData(parent);
    }
    public virtual Ability GetMoveData(MonoBehaviour parent)
    {
        return moveAbility.GetData(parent);
    }
    public virtual void UseUniqueData(MonoBehaviour parent)
    {
        uniqueAbility.Use(parent);
    }

}
