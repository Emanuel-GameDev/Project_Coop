using UnityEngine;

public abstract class Condition : MonoBehaviour
{
    Character parent;
    public virtual void AddCondition(Character parent)
    {
        //DA guardare funziona se non chiamata questa funzione hahaha
        Condition condition = Utility.InstantiateCondition<Condition>();
        condition.parent = parent;
        condition.transform.parent = parent.transform;
        
    }
    public virtual void RemoveCondition(Character parent)
    {
        parent.RemoveFromConditions(this);
        parent = null; 
        transform.parent = null;
        Destroy(this.gameObject);
    }

}
