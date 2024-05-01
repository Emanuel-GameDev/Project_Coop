using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    private static RewardManager _instance;
    public static RewardManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RewardManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("RewardManager");
                    _instance = singletonObject.AddComponent<RewardManager>();
                }
            }

            return _instance;
        }
    }
    [SerializeField] public GameObject rightPrefabRewards;
    [SerializeField] public GameObject leftPrefabRewards;
    [SerializeField] public float popUpDuration;
    [SerializeField] public float moveDuration;
}
