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
        
        private KerberosBossCharacter bossCharacter;
        private Vector3 targetPosition;
        private float tempTimer;
        private bool started;
        private bool mustStop;
        private List<PlayerCharacter> activePlayers;
        public override void OnEnter()
        {
            
            bossCharacter = parentGameObject.Value.GetComponent<KerberosBossCharacter>();
            targetPosition = targetTransform.Value.position;
            activePlayers = PlayerCharacterPoolManager.Instance.ActivePlayerCharacters;
            bossCharacter.Agent.isStopped = false;
            mustStop = false;
            tempTimer = 0;
            playerFound.Value = false;
            bossCharacter.anim.SetTrigger("Move");
           
            bossCharacter.anim.ResetTrigger("Return");
        }

        public override NodeResult Execute()
        {
            if (!bossCharacter.isDead)
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
                        bossCharacter.Agent.isStopped = true;
                        bossCharacter.Agent.SetDestination(bossCharacter.transform.position);
                        bossCharacter.anim.SetTrigger("Return");
                        return NodeResult.success;
                    }


                    float dist = Vector3.Distance(targetPosition, bossCharacter.transform.position);


                    if (mustStop || dist <= bossCharacter.minDistance)
                    {
                        bossCharacter.Agent.isStopped = true;
                        bossCharacter.Agent.SetDestination(bossCharacter.transform.position);
                        bossCharacter.anim.SetTrigger("Return");
                        return NodeResult.success;
                    }

                }
                else
                {

                    playerFound.Value = false;
                    bossCharacter.Agent.isStopped = true;
                    bossCharacter.Agent.SetDestination(bossCharacter.transform.position);
                    bossCharacter.anim.SetTrigger("Return");
                    return NodeResult.failure;

                }



                return NodeResult.running;
            }
            return NodeResult.running;
        }

        private bool CheckForNearPlayer()
        {
            foreach (PlayerCharacter player in activePlayers)
            {
                
                bool isNear = Vector2.Distance(player.transform.position, bossCharacter.transform.position) < distanceToCheckforPlayer.Value;
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
