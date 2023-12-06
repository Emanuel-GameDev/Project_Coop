using UnityEngine;

public class Condition : MonoBehaviour
{
    CharacterClass parent;
    public virtual void AddCondition(CharacterClass parent)
    {
        this.parent = parent;
        parent.AddCondition(this);
    }
    public virtual void RemoveCondition(CharacterClass parent)
    {
        this.parent = null;
        parent.RemoveCondition(this);
    }

}
