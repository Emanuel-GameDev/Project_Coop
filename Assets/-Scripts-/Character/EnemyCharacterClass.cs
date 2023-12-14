using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacterClass : CharacterClass
{
    //danno stamina
    public float staminaDamage;

    //Conditions??
    bool underAggro;

    public override DamageData GetDamageData()
    {
        return new DamageData(Damage,staminaDamage, character, false);
    }
}
