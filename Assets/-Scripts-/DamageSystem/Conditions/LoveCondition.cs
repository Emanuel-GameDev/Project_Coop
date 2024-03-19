using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoveCondition : Condition
{
    private PlayerCharacter player;
    private Character target;
    private float duration;
    public bool started;
    private float timer;

    public override void AddCondition(Character parent)
    {
        transform.parent = parent.transform;
        target = parent;
        //base.AddCondition(parent);
        Debug.Log(transform.parent.name + " sono sotto Innamoramento");       
        parent.inLove = true;
        foreach(PlayerCharacter p in GameManager.Instance.coopManager.ActivePlayers) 
        { 
            if(player != p)
            {
                if(p.inLove)
                {
                    started = true;
                    p.GetComponentInChildren<LoveCondition>().started = true;
                    Debug.Log(p.name + "inizio durata innamoramento di " + duration);

                }
                
                
            }
        }
    }

    public override void RemoveCondition(Character parent)
    {
        Debug.Log(parent.name + " non sono più sotto Innamoramento");
        target = null;
        parent.inLove = false;
        base.RemoveCondition(parent);

    }

    public void SetVariable(PlayerCharacter player, float duration)
    {
        this.player = player;
        this.duration = duration;

    }

    private void Update()
    {
        if (player != null)
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
