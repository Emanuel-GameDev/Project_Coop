using MBT;
using System.Collections.Generic;
using UnityEngine;

public enum AttackPhase
{
    slam,
    damageable,
    follow
}

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("PrisonerEros/PioggiaDiNatiche")]
    public class T_RainOfAss : Leaf
    {
        public TransformReference targetTransform;
        public GameObjectReference parentGameObject;

        private PrisonErosBossCharacter bossCharacter;
        private Vector3 targetPosition;
        private bool mustStop = false;
        private AttackPhase phase;

       
        private int attackCount;
        private float tempTimerDamageable;
        private float tempTimerFollow;
        private bool canFollow;
        private bool canTakeDamage = false;
        private bool isShadow;
        

        public override void OnEnter()
        {
            bossCharacter = parentGameObject.Value.GetComponent<PrisonErosBossCharacter>();
            mustStop = false;
            bossCharacter.Agent.isStopped = false;
            attackCount = 0;
            tempTimerDamageable = 0;
            tempTimerFollow = 0;
           

            //sale su con animazione          
        }
        public override NodeResult Execute()
        {
            if (attackCount <= bossCharacter.slamsQuantity)
            {
                attackCount++;

                //Inizio inseguimento player
                if (canFollow && bossCharacter.followDuration >= tempTimerFollow)
                {
                   if(!isShadow)
                    {
                        SetShadowForm(true);                        
                    }
                    tempTimerFollow += Time.deltaTime;

                    //Follow target                    
                    targetPosition = targetTransform.Value.position;
                    bossCharacter.Agent.speed = bossCharacter.walkSpeed;
                    bossCharacter.Agent.SetDestination(targetPosition);
                    
                }

                //inizio slam
                else if (!canFollow && !canTakeDamage)
                {                   
                    if (isShadow)
                    {
                        bossCharacter.Agent.isStopped = true;
                        SetShadowForm(false);
                        //Inizio schianto
                        //anim play schianto, con chiamata che crea onda urto e setto canTakeDamage a true

                    }

                }
                else if(canTakeDamage && bossCharacter.timerDamageable >= tempTimerDamageable)
                {                    
                    tempTimerFollow += Time.deltaTime;



                }



            }
            else
            {
                return NodeResult.success;
            }

            
            return NodeResult.running;

        }

        private void SetShadowForm(bool value)
        {
            if(value)
            {
                bossCharacter.shadowMesh.enabled = true;
                bossCharacter.spriteRenderer.enabled = false;
                isShadow = true;
            }
            else
            {
                bossCharacter.shadowMesh.enabled = false;
                bossCharacter.spriteRenderer.enabled = true;
                isShadow=false;
            }
        }
        

    }

    
}
