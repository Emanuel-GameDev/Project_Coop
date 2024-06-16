using System;
using UnityEngine;

[Serializable]
public class RumbleData
{
    [Range(0f, 1f)]
    public float lowFreqency;

    [Range(0f, 1f)]
    public float highFreqency;

    public float duration;

    public int priority = 0;

    public string rumbleName = "";

}
