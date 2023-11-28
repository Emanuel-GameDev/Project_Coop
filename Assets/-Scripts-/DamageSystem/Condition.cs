using UnityEngine;

public abstract class Condition : MonoBehaviour
{
    CharacterClass characterClass;
    public abstract void AddCondition(CharacterClass parent);
    public abstract void RemoveCondition(CharacterClass parent);
}
