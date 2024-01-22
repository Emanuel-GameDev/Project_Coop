using UnityEngine;

public class Condition : MonoBehaviour
{
    Character parent;
    public virtual void AddCondition(Character parent)
    {
        this.parent = parent;
        transform.parent = parent.transform;
    }
    public virtual void RemoveCondition(Character parent)
    {
        parent.RemoveFromConditions(this);
        parent = null; 
        transform.parent = null;
        Destroy(this.gameObject);
    }

}
