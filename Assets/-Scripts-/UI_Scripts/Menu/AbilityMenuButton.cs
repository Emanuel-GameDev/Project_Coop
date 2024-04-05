using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMenuButton : MonoBehaviour
{
    [SerializeField]
    private GameObject abilityTent;


    public void Activate()
    {
        abilityTent.SetActive(true);
    }

    public void Deactivate()
    {
        abilityTent.SetActive(false);
    }

}
