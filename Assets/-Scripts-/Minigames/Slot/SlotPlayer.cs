using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlotPlayer : InputReceiver
{
    [SerializeField] Slotmachine slotmachine;

    private void Awake()
    {
        slotmachine = FindAnyObjectByType<Slotmachine>();
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
