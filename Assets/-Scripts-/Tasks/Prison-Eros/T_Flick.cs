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
            bossCharacter.Agent.SetDestination(targetPosition);
            bossCharacter.Agent.SetDestination(targetPosition);
            //PlayAnimazione attacco frusta

        }

        public override NodeResult Execute()
        {
            if (bossCharacter.flickDone)
                return NodeResult.success;
            return NodeResult.running;
        }

    }

}
