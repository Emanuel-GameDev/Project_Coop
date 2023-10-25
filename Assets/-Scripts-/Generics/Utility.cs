using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static bool IsInLayerMask(this LayerMask targetMask, int layer)
    {
        int layerbit = 1 << layer;
        return (targetMask & layerbit) != 0;
    }
}
