using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Ability : ScriptableObject
{
    public List<AbilityUpgrade> obtainedUpgrades;

    public virtual void Use(MonoBehaviour parent) 
    {
        //Dovrebbe ciclare la lista degli upgrade quando viene chiamata per applicarli
    }

    public virtual Ability GetData(MonoBehaviour parent)
    {
        return this;
    }

    

}
