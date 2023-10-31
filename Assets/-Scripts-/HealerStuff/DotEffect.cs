using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DotEffect : StatusEffectBehaviour
{
    public float DOTDamage = 1;
    public float countdown = 1;
    float timer = 0;

    Coroutine cor;


    public void ApplyDOT(Character characterToDamage, float damagePerTik, float tikPerSecond)
    {
        DOTDamage = damagePerTik;
        countdown = 1 / tikPerSecond;
    }

    public void RemoveDOT()
    {
        Destroy(this);
    }


    private void Update()
    {
        if (timer >= countdown)
        {
            gameObject.GetComponent<Character>().TakeDamage(DOTDamage, null);
            timer = 0;
        }

        timer += Time.deltaTime;
    }

}
