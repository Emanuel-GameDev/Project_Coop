using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RumbleData : MonoBehaviour
{
    [Range(0f, 1f)]
    public float lowFreqency;

    [Range(0f, 1f)]
    public float highFreqency;

    public float duration;

    public int priority = 0;

    public string rumbleName = "";

}
