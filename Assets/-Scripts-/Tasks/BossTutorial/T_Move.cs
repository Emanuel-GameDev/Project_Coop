using MBT;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Custom Taks/Muovi ")]
    public class T_Move : Leaf
    {
        public TransformReference targetTransform;
        public GameObjectReference parentGameObject;
        public FloatReference distanceToCheckforPlayer;
        public BoolReference playerFound = new BoolReference(VarRefMode.DisableConstant);
        
        private TutorialBossCharacter bossCharacter;
        private Vector3 targetPosition;
        private float tempTimer;
        private bool started;
        private bool mustStop;
        private List<PlayerCharacter> activePlayers;
        public override void OnEnter()
        {
            bossCharacter = parentGameObject.Value.GetComponent<TutorialBossCharacter>();
            targetPosition = targetTransform.Value.position;
            activePlayers = GameManager.Instance.coopManager.activePlayers;
            mustStop = false;
            tempTimer = 0;
            playerFound.Value = false;
        }

        public override NodeResult Execute()
        {
           
            if (bossCharacter.followDuration >= tempTimer)
            {
               
                tempTimer += Time.deltaTime;

                //Follow target
                targetPosition = targetTransform.Value.position;
                bossCharacter.Agent.speed = bossCharacter.walkSpeed;
                bossCharacter.Agent.SetDestination(targetPosition);

                //Check if a player enters in small range;
                if (CheckForNearPlayer())
                {
                    playerFound.Value = true;
                    return NodeResult.success;
                }


                float dist = Vector3.Distance(targetPosition, bossCharacter.transform.position);

                if (mustStop || dist <= bossCharacter.minDistance)
                {
                    bossCharacter.Agent.isStopped = true;
                    return NodeResult.success;
                }

            }
            else
            {
                //start charge
                playerFound.Value = false;
                return NodeResult.failure;
                
            }



            return NodeResult.running;
        }

        private bool CheckForNearPlayer()
        {
            foreach (PlayerCharacter player in activePlayers)
            {
                
                bool isNear = Utility.DistanceV3toV2(player.transform.position, bossCharacter.transform.position) < distanceToCheckforPlayer.Value;
                if (isNear)
                {
                    bossCharacter.target = player.transform;
                    targetTransform.Value = player.transform;
                    return true;
                    
                }
            }
            return false;
        }





    }
}
