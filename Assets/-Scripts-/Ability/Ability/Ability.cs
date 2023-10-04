using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Ability : ScriptableObject
{
    public virtual void Use(MonoBehaviour parent) { }

    public virtual Ability GetData(MonoBehaviour parent)
    {
        return this;
    }
}
