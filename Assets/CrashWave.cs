using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashWave : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Animation>().Play();
    }
}
