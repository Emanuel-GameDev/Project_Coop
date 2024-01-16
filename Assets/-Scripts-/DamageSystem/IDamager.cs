using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IDamager
{
    //modifiche

    //float GetDamage();
    DamageData GetDamageData();


    //aaaaaa non so
    virtual void OnParryNotify() { }

    //fine modifiche
}
