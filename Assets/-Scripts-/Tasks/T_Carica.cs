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

        private TutorialBossCharacter bossCharacter;
        private bool started = false;
        private bool mustStop = false;
        private float tempTimer;        
        private Vector3 targetPosition;

        public override void OnEnter()
        {
            bossCharacter = parentGameObject.Value.GetComponent<TutorialBossCharacter>();
            targetPosition = targetTransform.Value.position;

            started = false;
            mustStop = false;

            Vector3 direction = (targetPosition-bossCharacter.transform.position).normalized;
            targetPosition = new Vector3((direction.x * bossCharacter.chargeDistance), 0,(direction.z * bossCharacter.chargeDistance)) + bossCharacter.transform.position; 

            Debug.Log("Start Charge Timer");
            tempTimer = 0;
            
        }
      
        public override NodeResult Execute()
        {           
                  
            if(tempTimer > chargeTimer)
            {
                if (!started)
                {
                    Debug.Log("partito");
                    bossCharacter.Agent.isStopped = false;
                    bossCharacter.Agent.speed = bossCharacter.chargeSpeed;
                    bossCharacter.Agent.SetDestination(targetPosition);
                    started = true;

                }

                float dist = Vector3.Distance(targetPosition, bossCharacter.transform.position);
                Debug.Log(dist);

               
                
                if(mustStop || dist <= minDistance)
                {
                    
                    bossCharacter.Agent.isStopped = true;
                    return NodeResult.success;
                }
            }

            else
            {
                tempTimer += Time.deltaTime;               
            }

            return NodeResult.running;
        }
    }
}