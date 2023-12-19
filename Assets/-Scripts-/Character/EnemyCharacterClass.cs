using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacterClass : CharacterClass
{
    
    public float staminaDamage;

    public override DamageData GetDamageData()
    {
        return new DamageData(Damage,staminaDamage, character, false);
    }
}
