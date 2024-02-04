using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRotation : MonoBehaviour
{
    [SerializeField] private bool shouldUpdate;
    public void RotateToCamera()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 cameraRotation = cam.transform.eulerAngles;
            transform.eulerAngles = cameraRotation;
        }
    }

    private void Awake()
    {
        RotateToCamera();
    }
    private void Update()
    {
        if (shouldUpdate)
        {
            RotateToCamera();
        }
    }
}
