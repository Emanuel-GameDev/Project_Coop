using MBT;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("PrisonerEros/ColpoDiFrusta")]
    public class T_Flick : Leaf
    {
        public TransformReference targetTransform;
        public GameObjectReference parentGameObject;

        private PrisonErosBossCharacter bossCharacter;
        private Vector3 targetPosition;
        

       
        public override void OnEnter()
        {
            bossCharacter = parentGameObject.Value.GetComponent<PrisonErosBossCharacter>();
            bossCharacter.flickDone = false;
            targetPosition = targetTransform.Value.position;
            
            Vector3 direction = (targetTransform.Value.position - bossCharacter.transform.position).normalized;
            targetPosition = new Vector3((direction.x * bossCharacter.flickDistance), 0, (direction.z * bossCharacter.flickDistance)) + bossCharacter.transform.position;
            bossCharacter.Agent.SetDestination(targetPosition);

            //PlayAnimazione attacco frusta a fine animazione setto flickDone

        }

        public override NodeResult Execute()
        {
           
            float dist = Vector3.Distance(targetPosition, bossCharacter.transform.position);
            if (bossCharacter.flickDone || dist <= bossCharacter.minDistance)
            {

                bossCharacter.Agent.isStopped = true;
                return NodeResult.success;
            }
            return NodeResult.running;
        }

    }

}
