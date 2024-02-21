using UnityEngine;

public class AggroCondition : Condition
{
    private CharacterClass player;
    private Character target;
    private float duration;
    private bool started;
    private float timer;

    public override void AddCondition(Character parent)
    {       
        transform.parent = parent.transform;
        target = parent;
       //base.AddCondition(parent);
        Debug.Log(transform.parent.name + " sono sotto AGGRO per " + duration + " secondi");
        started = true;
        parent.underAggro = true;
    }

    public override void RemoveCondition(Character parent)
    {
        Debug.Log(parent.name + " non sono più sotto AGGRO");
        target = null;
        parent.underAggro = false;
        base.RemoveCondition(parent);
        
    }

    public void SetVariable(CharacterClass player, float duration)
    {
        this.player = player;
        this.duration = duration;
          
    }

    private void Update()
    {
       if(player != null)
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

}
