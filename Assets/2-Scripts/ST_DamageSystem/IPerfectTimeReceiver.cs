using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPerfectTimeReceiver
{
    void SetPerfectTimingHandler(PerfectTimingHandler handler);

     void  PerfectTimeStarted(IDamager damager);
    

    void PerfectTimeEnded();
}
