using UnityEngine;

public class StunCondition : Condition
{
    private Character characterClass;
    private Character target;
    private float duration;
    private bool started;
    private float timer;

    public override void AddCondition(Character parent)
    {
        transform.parent = parent.transform;
        target = parent;
        //base.AddCondition(parent);  
        Debug.Log(transform.parent.name + " sono sotto STUN per " + duration + " secondi");
        started = true;
        parent.stunned = true;
    }

    public override void RemoveCondition(Character parent)
    {

        Debug.Log(parent.name + " non sono più sotto STUN");
        target = null;
        parent.stunned = false;
        base.RemoveCondition(parent);

    }


    public void SetVariable(float duration)
    {
        this.duration = duration;

    }

    private void Update()
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
        }


    }

}
