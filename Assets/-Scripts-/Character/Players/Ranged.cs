using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged : CharacterClass
{
    // inserire riferimento a una futura projectilePoll,
    // il prefab è solo per convenienza

    [SerializeField] GameObject prefabBullet;

    public override void Attack(Character parent)
    {
        BasicFireProjectile(parent.GetComponent<PlayerCharacter>().ReadLook());
    }

    public override void Defence(Character parent)
    {
        base.Defence(parent);
    }

    public override void UseExtraAbility(Character parent)
    {
        base.UseExtraAbility(parent);
    } 

    public override void UseUniqueAbility(Character parent)
    {
        base.UseUniqueAbility(parent);
    }

    private void BasicFireProjectile(Vector2 direction)
    {
        GameObject bullet= Instantiate(prefabBullet);
    }
}
