using MBT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Custom Taks/Attacco Carica ")]
    public class T_Attack : Leaf
    {
        public TransformReference targetTransform;
        public GameObjectReference parentGameObject;
        public float chargeTimer = 2;        
        public float minDistance = 0.1f;

        private BossCharacter character;
        private bool canCharge;
        private float speed;

        public override void OnEnter()
        {
          character = parentGameObject.Value.GetComponent<BossCharacter>();
            Debug.Log("Start Charge Timer");
            Invoke(nameof(SetChargeTrue),chargeTimer);
        }

        private void SetChargeTrue()
        {
            Debug.Log("StartCharge");
           canCharge = true;
        }

        public override NodeResult Execute()
        {
            Vector3 target = targetTransform.Value.position;
            GameObject obj = parentGameObject.Value;
            speed = character.MoveSpeed;

            if (canCharge)
            {

                float dist = Vector3.Distance(target, obj.transform.position);

                if (dist > minDistance)
                {
                    // Move towards target
                    obj.transform.position = Vector3.MoveTowards(obj.transform.position, target,
                        (speed > dist) ? dist : speed
                    );

                    return NodeResult.running;
                }
                else
                {
                    return NodeResult.success;
                }
            }

            return NodeResult.running;
        }
    }
}