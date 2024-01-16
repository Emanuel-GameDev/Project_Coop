using MBT;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Custom Taks/Muovi ")]
    public class T_Move : Leaf
    {
        public TransformReference targetTransform;
        public GameObjectReference parentGameObject;

        private TutorialBossCharacter bossCharacter;
        private float tempTimer;
        public override void OnEnter()
        {
            bossCharacter = parentGameObject.Value.GetComponent<TutorialBossCharacter>();
            tempTimer = 0;
        }

        public override NodeResult Execute()
        {
            return NodeResult.running;
            FollowTarget();
            CheckIfTargetInRange();
        }

        private void CheckIfTargetInRange()
        {
            throw new NotImplementedException();
        }

        private void FollowTarget()
        {
            bossCharacter.Agent.speed = bossCharacter.flurrySpeed;
            bossCharacter.Agent.SetDestination(targetPosition);

        }


    }
    }
}