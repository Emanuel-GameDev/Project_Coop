using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DotEffect : StatusEffectBehaviour
{
    public float DOTDamage = 1;
    public float countdown = 1;
    float DOTTimer = 0;




    public void ApplyDOT(float damagePerTik, float tikPerSecond)
    {
        DOTDamage = damagePerTik;
        countdown = 1 / tikPerSecond;
    }


    public override void RemoveDOT()
    {
        Destroy(this);
    }


    private void Update()
    {
        //ElapseDOTTimer();
    }

    private void ElapseDOTTimer()
    {
        if (DOTTimer >= countdown)
        {
            gameObject.GetComponent<Character>().TakeDamage(new DamageData(DOTDamage, null));
            DOTTimer = 0;
        }

        DOTTimer += Time.deltaTime;
    }
}
