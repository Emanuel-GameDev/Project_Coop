using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlotPlayer : DefaultInputReceiver
{
    [SerializeField] Slotmachine slotmachine;

    private void Awake()
    {
        slotmachine= FindAnyObjectByType(typeof(Slotmachine)).GetComponent<Slotmachine>();
        slotmachine.listOfCurrentPlayer.Add(this);

    }

    private void Start()
    {
        
    }


    public override void ButtonSouth(InputAction.CallbackContext context)
    {
        slotmachine.InputStop(this);

        
    }


}
