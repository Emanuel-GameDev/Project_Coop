using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    CinemachineTargetGroup targetGroup;

    public void AddTarget(PlayerInput target)
    {
        targetGroup.AddMember(target.transform, 1, 5);
    }
    
    public void RemoveTarget(PlayerInput target)
    {
        targetGroup.RemoveMember(target.transform);
    }
}
