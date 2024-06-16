using System;
using UnityEngine;

[Serializable]
public class RumbleData
{
    public string rumbleName = "";

    [Range(0f, 1f)]
    public float lowFreqency;

    [Range(0f, 1f)]
    public float highFreqency;

    public float duration;

    public int priority = 0;

}
