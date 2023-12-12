using UnityEngine;

public class AggroCondition : Condition
{
    private CharacterClass player;
    private float duration;
    private bool started;
    private float timer;

    public override void AddCondition(CharacterClass parent)
    {       
        transform.parent = parent.transform;
        base.AddCondition(parent);
        Debug.Log(transform.parent.name + " sono sotto aggro per " + duration + " secondi");
    }

    public override void RemoveCondition(CharacterClass parent)
    {
        Debug.Log(parent.name + " non sono più sotto aggro");
        Destroy(this.gameObject);
        
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
            if (!started)
            {
                started = true;
                timer = duration;
                
            }
                
        }
    }

}
