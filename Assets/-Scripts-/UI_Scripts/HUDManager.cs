using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    private static HUDManager _instance;
    public static HUDManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HUDManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("HUDManager");
                    _instance = singletonObject.AddComponent<HUDManager>();
                }
            }

            return _instance;
        }
    }
}
