using UnityEngine;

public class Condition : MonoBehaviour
{
    CharacterClass parent;
    public virtual void AddCondition(CharacterClass parent)
    {
        this.parent = parent;
        transform.parent = parent.transform;
    }
    public virtual void RemoveCondition(CharacterClass parent)
    {
        parent.RemoveFromConditions(this);
        parent = null; 
        transform.parent = null;
        Destroy(this.gameObject);
    }

}
