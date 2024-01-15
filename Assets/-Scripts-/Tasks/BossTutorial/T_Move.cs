using MBT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Custom Taks/Muovi ")]
    public class T_Move : Leaf
    {
        
        public override void OnEnter()
        {
           
        }

        public override NodeResult Execute()
        {

            return NodeResult.success;
        }

    }
}