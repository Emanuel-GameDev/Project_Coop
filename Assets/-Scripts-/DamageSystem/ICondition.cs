using UnityEngine;

public interface ICondition
{
   
    public abstract void AddCondition(CharacterClass parent);
    public abstract void RemoveCondition(CharacterClass parent);

}
