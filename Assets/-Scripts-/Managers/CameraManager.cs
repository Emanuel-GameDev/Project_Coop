using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    CinemachineTargetGroup targetGroup;
    [SerializeField]
    float cameraDistance = 6.5f;

    private static CameraManager _instance;
    public static CameraManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CameraManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("CameraManager");
                    _instance = singletonObject.AddComponent<CameraManager>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void AddAllPlayers()
    {
        List<PlayerCharacter> players = GameManager.Instance.coopManager.ActivePlayers;

        if(players.Count == 0)  
            return;

        foreach (PlayerCharacter player in players)
        {
            AddTarget(player.transform);
        }

    }


    public void AddTarget(Transform target)
    {
        if(targetGroup != null) 
        targetGroup.AddMember(target.transform, 1, cameraDistance);
    }
    
    public void RemoveTarget(Transform target)
    {
        if (targetGroup != null)
            targetGroup.RemoveMember(target.transform);
    }
}
