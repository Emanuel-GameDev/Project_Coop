using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoResetter : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(ResetSave());
    }

    IEnumerator ResetSave()
    {
        yield return new WaitForSeconds(.5f);
        SaveManager.Instance.ClearSaveData();
    }
}
