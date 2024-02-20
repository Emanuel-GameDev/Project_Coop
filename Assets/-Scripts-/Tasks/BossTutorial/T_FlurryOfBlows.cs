using MBT;
using System.Collections;
using System.Threading;
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
        private bool mustStop = false;

        private int attackCount;
        private bool canAttack;

        public override void OnEnter()
        {
            bossCharacter = parentGameObject.Value.GetComponent<TutorialBossCharacter>();            
            mustStop = false;
            bossCharacter.Agent.isStopped = false;
            attackCount = 0;


            Vector3 direction = (targetTransform.Value.position - bossCharacter.transform.position).normalized;
            targetPosition = new Vector3((direction.x * bossCharacter.flurryDistance), 0, (direction.z * bossCharacter.flurryDistance)) + bossCharacter.transform.position;
            bossCharacter.SetFlurryOfBlowsDamageData(attackCount);
            Debug.Log("inizio attacco " + attackCount);


            bossCharacter.Agent.speed = bossCharacter.flurrySpeed;
            bossCharacter.Agent.SetDestination(targetPosition);
            bossCharacter.anim.SetTrigger("FlurryOfBlows");



        }

        public override NodeResult Execute()
        {
            
            float dist = Vector3.Distance(targetPosition, bossCharacter.transform.position);

            if (mustStop || dist <= bossCharacter.minDistance)
            {
                attackCount++;
                bossCharacter.SetFlurryOfBlowsDamageData(attackCount);


                if (attackCount >= bossCharacter.punchQuantity)
                {
                    bossCharacter.Agent.isStopped = true;
                    bossCharacter.anim.SetTrigger("Return");
                    return NodeResult.success;
                }

                else
                {
                    
                    Debug.Log("inizio attacco " + attackCount);
                    
                    Vector3 direction = (targetTransform.Value.position - bossCharacter.transform.position).normalized;
                    targetPosition = new Vector3((direction.x * bossCharacter.flurryDistance), 0, (direction.z * bossCharacter.flurryDistance)) + bossCharacter.transform.position;

                    if (canAttack)
                    {
                        bossCharacter.anim.SetTrigger("FlurryOfBlows" + attackCount);
                        bossCharacter.Agent.speed = bossCharacter.flurrySpeed;
                        bossCharacter.Agent.SetDestination(targetPosition);

                    }

                    return NodeResult.running;

                }

            }


            return NodeResult.running;
        }
        
        public IEnumerator AttackPreview(float Timer)
        {
            yield return new WaitForSeconds(Timer);
            canAttack = true;

        }
    }

    
}
