using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCharacter : EnemyCharacter
{
    protected Condition attackCondition;

    public override void TargetSelection()
    {
        base.TargetSelection();
    }
    

    public override DamageData GetDamageData()
    {
        return new DamageData(damage, staminaDamage, this, attackCondition, true);
    }

    

}
