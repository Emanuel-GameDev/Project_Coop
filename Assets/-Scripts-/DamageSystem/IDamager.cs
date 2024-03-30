using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IDamager
{
    
    //modifiche

    //float GetDamage();
    DamageData GetDamageData();

    
    Transform dealerTransform { get; }


    //aaaaaa non so
    void OnParryNotify(Character whoParried);

    //fine modifiche
}
