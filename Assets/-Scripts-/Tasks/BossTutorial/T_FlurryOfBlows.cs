using MBT;
using System;
using System.Collections;
using UnityEngine;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Kerberos/Raffica Di Colpi")]
    public class T_FlurryOfBlows : Leaf
    {
        public TransformReference targetTransform;
        public GameObjectReference parentGameObject;

        private TutorialBossCharacter bossCharacter;
        private Vector3 targetPosition;

        private int attackCount;
        

        public override void OnEnter()
        {
            bossCharacter = parentGameObject.Value.GetComponent<TutorialBossCharacter>();
            bossCharacter.Agent.isStopped = false;

           
            bossCharacter.canShowPreview = true;
            attackCount = 0;
            bossCharacter.SetFlurryOfBlowsDamageData(attackCount);
            bossCharacter.Agent.speed = bossCharacter.flurrySpeed;
            bossCharacter.canAttackAnim = false;


            Vector3 direction = (targetTransform.Value.position - bossCharacter.transform.position).normalized;
            targetPosition = new Vector3((direction.x * bossCharacter.flurryDistance), (direction.y * bossCharacter.flurryDistance), 0) + bossCharacter.transform.position;
           
        }

        public override NodeResult Execute()
        {

            float dist = Vector2.Distance(targetPosition, bossCharacter.transform.position);

            //esci da attacco
            if (attackCount >= bossCharacter.punchQuantity)
            {
                bossCharacter.anim.SetTrigger("Return");
                return NodeResult.success;
            }

            else
            {
                //attacco
                if (bossCharacter.canAttackAnim)
                {
                   
                    attackCount++;
                    bossCharacter.SetFlurryOfBlowsDamageData(attackCount);

                    Debug.Log("inizio attacco " + attackCount);

                    Vector3 direction = (targetTransform.Value.position - bossCharacter.transform.position).normalized;

                    targetPosition = new Vector3((direction.x * bossCharacter.flurryDistance), (direction.y * bossCharacter.flurryDistance), 0) + bossCharacter.transform.position;

                    bossCharacter.canAttackAnim = false;
                   
                    bossCharacter.anim.SetTrigger("FlurryOfBlows" + attackCount);
                    
                    bossCharacter.Agent.SetDestination(targetPosition);

                    return NodeResult.running;
                }

                //preview attacco
                else
                {
                    if(bossCharacter.canShowPreview)
                    {
                        NextAttackPreview();                                              
                    }
                   
                    parentGameObject.Value.transform.LookAt(targetTransform.Value);
                    
                }


            }


            return NodeResult.running;
        }

        public void NextAttackPreview()
        {
           
            
            bossCharacter.canShowPreview = false;
            Debug.Log("Mostra preview");
            bossCharacter.previewArrow.SetActive(true);
            StartCoroutine(bossCharacter.StartAttackPunch());
        }



    }


}
