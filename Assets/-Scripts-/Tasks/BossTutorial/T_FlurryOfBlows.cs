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
        private bool attackStarted = false;

        public override void OnEnter()
        {
            bossCharacter = parentGameObject.Value.GetComponent<TutorialBossCharacter>();
            bossCharacter.Agent.isStopped = false;
            attackStarted = false;
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
                    attackStarted = false;
                    bossCharacter.anim.SetTrigger("FlurryOfBlows" + attackCount);
                    
                    bossCharacter.Agent.SetDestination(targetPosition);

                    return NodeResult.running;
                }

                //preview attacco
                else if(!attackStarted)
                {
                    SetUpBoss();
                    AttackPreview();
                    bossCharacter.StartCoroutine(bossCharacter.StartAttackPunch());
                    
                }


            }


            return NodeResult.running;
        }

        private void SetUpBoss()
        {
            attackStarted = true;
            Vector3.RotateTowards(bossCharacter.transform.rotation.eulerAngles, targetPosition,360f,360f);
        }

        public void AttackPreview()
        {
            
            //mostra preview attacco
            Debug.Log("Mostra preview");
            Vector3.RotateTowards(bossCharacter.previewArrow.transform.rotation.eulerAngles, targetPosition, 360f, 360f);
            bossCharacter.previewArrow.SetActive(true);




        }
    }


}
