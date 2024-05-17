using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedCondition : Condition
{

    private PlayerCharacter target;
    private BossCharacter bleedDealer;
    private float duration;
    private bool started;
    private float timer;
    private float checkInterval = 2f;

    public override void AddCondition(Character parent)
    {
        transform.parent = parent.transform;
        //Da mettere?? target = parent;
        //base.AddCondition(parent);
        Debug.Log(transform.parent.name + " sono sotto Sanguinamento");
        parent.bleeding = true;

    }

    public override void RemoveCondition(Character parent)
    {
        Debug.Log(parent.name + " non sono più sotto Sanguinamento");
        target = null;
        parent.bleeding = false;
        base.RemoveCondition(parent);

    }

    public void SetVariable(float duration, BossCharacter dealer)
    {

        this.duration = duration;
        this.bleedDealer = dealer;

    }

    private void Update()
    {
        if (bleedDealer != null)
        {
            if (started)
            {
                if (timer >= duration)
                {
                    RemoveCondition(target);
                }
                else
                {
                    timer += Time.deltaTime;
                }

                if (timer % checkInterval < Time.deltaTime)
                {
                    target.TakeDamage(new DamageData(target.MaxHp * 0.03f, bleedDealer));
                }
            }
        }

    }
}
