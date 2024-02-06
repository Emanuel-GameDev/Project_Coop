using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlotPlayer : MonoBehaviour
{
    [SerializeField] Slotmachine slotmachine;

    private PlayerInput input;


    private void Start()
    {
        slotmachine=FindObjectOfType(typeof(Slotmachine)) as Slotmachine;

    }

    private void OnEnable()
    {
        input = GetComponent<PlayerInput>();

    }

    public void Sium(InputAction.CallbackContext context)
    {
        Debug.Log("kjdshf");
    }

    public void SiumDialogue(InputAction.CallbackContext context)
    {
        Debug.Log("kjdshf si, ma dialoigue");
    }
}
