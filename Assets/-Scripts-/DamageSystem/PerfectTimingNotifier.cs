using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerfectTimingNotifier : MonoBehaviour
{
    public IDamager damager;
    private void OnEnable()
    {
        damager = GetComponentInParent<IDamager>();
    }

}
