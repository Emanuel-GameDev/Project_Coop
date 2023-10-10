using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Move")]
public class Move : Ability
{
    [SerializeField]  public float movingSpeed = 5f;
    
    public override Ability GetData(MonoBehaviour parent)
    {
        return base.GetData(parent);
    }
}
