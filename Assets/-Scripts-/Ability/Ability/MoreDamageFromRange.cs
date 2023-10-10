using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Active/MoreDamageFromRange")]
public class MoreDamageFromRange : Ability
{
    private float range;
    private float damageIncreaser;
    public override void Use(MonoBehaviour parent)
    {
        CalcolaCose();
    }

    private void CalcolaCose()
    {

    }

}
