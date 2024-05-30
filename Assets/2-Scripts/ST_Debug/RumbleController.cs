using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RumbleController : MonoBehaviour
{
    [Range(0f, 1f)]
    public float lowFreqency;

    [Range(0f, 1f)]
    public float highFreqency;

    public float duration;
}
