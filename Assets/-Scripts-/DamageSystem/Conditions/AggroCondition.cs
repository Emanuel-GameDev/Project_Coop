using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroCondition : MonoBehaviour,ICondition
{
    private CharacterClass target;
    private float duration;

    public AggroCondition(CharacterClass target, float duration)
    {
        this.target = target;
        this.duration = duration;
    }


    public void AddCondition(CharacterClass parent)
    {
        parent.gameObject.AddComponent<AggroCondition>();
        Invoke(nameof(RemoveCondition), duration);
        Debug.Log(parent.name + " sono sotto aggro per " + duration + " secondi");
    }

    public  void RemoveCondition(CharacterClass parent)
    {
        Destroy(parent.GetComponent<AggroCondition>());
        Debug.Log(parent.name + " non sono più sotto aggro");
    }

    private void Update()
    {
        if (target != null)
        {
            Debug.Log($" {this.transform.gameObject.name } aggro contro {target.name}");
        }
    }

}
