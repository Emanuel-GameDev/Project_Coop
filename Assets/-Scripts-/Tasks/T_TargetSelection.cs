using MBT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Custom Taks/Target Selection ")]
    public class T_TargetSelection : Leaf
    {
        public GameObjectReference parentGameObject;
        public TransformReference blackboardVariable = new TransformReference(VarRefMode.DisableConstant);
        private BossCharacter bossCharacter;

        public override void OnEnter()
        {          
            bossCharacter =  parentGameObject.Value.GetComponent<BossCharacter>();
            bossCharacter.TargetSelection();
            
            blackboardVariable.Value = bossCharacter.Target;
            Debug.LogWarning(bossCharacter.target.name + "palle" + blackboardVariable.Value.name);
        }

        public override NodeResult Execute()
        {
            if(bossCharacter.Target != null)
            {
                return NodeResult.success;
            }
            else
            {
               
                return NodeResult.failure;
            }
            
        }
    }
}