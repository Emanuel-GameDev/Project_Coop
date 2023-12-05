using UnityEngine;

public class AggroCondition : Condition
{
    private CharacterClass player;
    private float duration;

    public override void AddCondition(CharacterClass parent)
    {       
        transform.parent = parent.transform;        
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
       
    }

}
